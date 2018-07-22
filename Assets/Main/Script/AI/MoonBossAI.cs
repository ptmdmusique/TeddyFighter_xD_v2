using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoonBossAI : GeneralAI
{
	//Main script
	private MoonBoss script;

	//Position stuff
	private Vector3 bossInitialPosition;
	private Vector3 bossSpawnPosition;

	//AI stuff			---------------- TO DO: Link with database
	private bool startAIing = false;
	private float curMileStone = 1;             //Boss checkpoint
	private float percentPerMileStone;
	//Boss stuff
	private float speedMultiplier;
	//Minion stuff
	public float minSpawnRadius = 10;
	public float maxSpawnRadius = 10;
	private int numberOfMinionSpawning = 3;
	private float waitBetweenSpawning = 1f;
	private int spawningMilestone;
	private float minionTimeMilestone;
	//Path
	private float pathSpeed = 50;
	private float pathMilestone = 0.9f;
	private DOTweenPath pathScript;	

	//Coroutine
	private Coroutine spawningCoroutine;
	private Coroutine launchingCoroutine;
	private bool isMoving = false;
	private float moveIdle = 1f;
	private float minionIdle = 1.5f;

	#region Default
	void Awake()
	{
		script = GetComponent<MoonBoss>();

		//Spawn position
		bossSpawnPosition = transform.position;

		CircleCollider2D temp = GetComponent<CircleCollider2D>();
		minSpawnRadius = temp.radius * 1.5f * transform.localScale.x;       //Since it is a circle, the .x and .y scales should be the same
		maxSpawnRadius = minSpawnRadius * 1.7f;

		//Get DOTween
		pathScript = GetComponent<DOTweenPath>();

		//TODO: Link with database
		//Changing the info based on the difficulty
		switch (myType) {
			case AIType.Simple:
				//AI
				curMileStone = 0.6f;
				percentPerMileStone = 0.3f;
				//Boss
				speedMultiplier = 1.2f;
				//Minion
				spawningMilestone = 2;
				minionTimeMilestone = 1.2f;
				//Path
				pathMilestone = 1.2f;
				break;
			case AIType.Intermediate:
				//AI
				curMileStone = 0.8f;
				percentPerMileStone = 0.25f;
				//Boss
				speedMultiplier = 1.4f;
				//Minion
				spawningMilestone = 2;
				minionTimeMilestone = 1.4f;
				//Path
				pathMilestone = 2f;
				break;
			case AIType.Complex:
				//AI
				curMileStone = 0.9f;
				percentPerMileStone = 0.2f;
				//Boss
				speedMultiplier = 1.5f;
				//Minion
				spawningMilestone = 3;
				minionTimeMilestone = 1.45f;
				pathMilestone = 3f;
				break;
		}
	}
	private void Start()
	{
		bossInitialPosition = script.sceneManager.bossInitialLocation.position;     //Get the correct position to move to

		//Start AIing
		MoveToInitial();
	}
	void Update()
	{
		if (startAIing == true) {
			//Start doing AI stuff
			//Updating the milestone as it takes the hit
			if (script.curHealth <= curMileStone * script.maxHealth) {
				script.mySpeed *= speedMultiplier;          //Increase the boss' speed
				numberOfMinionSpawning += spawningMilestone;//Increase the number of minion spawn
				waitBetweenSpawning *= minionTimeMilestone; //Decrease the interval between each spawn

				curMileStone -= percentPerMileStone;        //Reduce the milestone
				pathSpeed *= pathMilestone;                  //Reduce the time to travel the path
			}
		}

		if (Input.GetKeyDown(KeyCode.L) == true) {
			UsePath();
		}
	}
	#endregion

	#region Action -- States
	//Decision
	public void MoveDecide()
	{
		if (isMoving == true) {
			return;
		}

		//Decide what to do next
		int chance = Random.Range(0, 100);
		if (chance < 70) {
			//Keep moving
			isMoving = true;
			MoveToRandom();
		} else if (chance < 90) {
			//Scaling as a indicator
			transform.DOScale(1f, 0.5f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutElastic).
				OnComplete(
				() => {
					//Move along the path
					isMoving = true;
					UsePath();
				}
				);
		} else {
			//20% that we will do nothing and wait for a little bit
			Invoke("MoveDecide", moveIdle);
		}
	}
	public void MinionDecide()
	{
		if (spawningCoroutine != null || launchingCoroutine != null) {
			return;
		}

		int chance = Random.Range(0, 100);
		if (chance < 80) {
			//80% that we will do something
			if (script.minionSpawnedList.Count > 0) {
				//We already spawned something
					//Changing color as an indicator
				GetComponent<SpriteRenderer>().DOColor(Color.red, 0.2f).SetLoops(2, LoopType.Yoyo).
					SetEase(Ease.InOutQuad).
					OnComplete(() => launchingCoroutine = StartCoroutine(LaunchMinion()));				
			} else {
				spawningCoroutine = StartCoroutine(SpawningCoroutine());
			}
		} else {
			//if we are idling, then wait a little bit and try again
			Invoke("MinionDecide", minionIdle);
		}
	}
	//Minion
	public Transform SpawnMinion()
	{
		Vector2 spawnPosition = Random.insideUnitCircle.normalized * Random.Range(minSpawnRadius, maxSpawnRadius) + (Vector2)transform.position;
		return script.SpawnMinion(0, spawnPosition);
	}
	public IEnumerator LaunchMinion()
	{
		//Decide what to do next
		MoveDecide();

		Transform player = StaticGlobal.GetPlayer();
		List<Transform> temp = new List<Transform>();
		for (int indx = 0; indx < script.minionSpawnedList.Count; indx++) {
			Transform minion = script.minionSpawnedList[indx];
			if (minion != null) {
				script.objectMaster.AddChild(minion);
				GeneralObject minionScript = minion.GetComponent<GeneralObject>();
				script.MoveMinion(minionScript, player.position - minion.position); //Launch the minion
				temp.Add(minion);       //Temporarily add the minion to delete list
				yield return new WaitForSeconds(0.3f);
			}
		}

		//Delete 
		foreach (Transform minion in temp) {
			script.minionSpawnedList.Remove(minion);                            //Remove from the original list
		}
		launchingCoroutine = null;
	}				//State
	public IEnumerator SpawningCoroutine()
	{
		//Decide what to do next
		MoveDecide();

		//Spawning all the minion
		for (int indx = 0; indx < numberOfMinionSpawning; indx++) {
			SpawnMinion();
			yield return new WaitForSeconds(waitBetweenSpawning / numberOfMinionSpawning);   //Wait a little bit
		}

		spawningCoroutine = null;
	}			//State					
	//Path
	public void UsePath()
	{
		MinionDecide();       //Also decide what to do

		pathScript.GetTween().timeScale = pathSpeed;
		pathScript.DORestart(true);
	}							//State
	//Move around
	public void MoveToRandom()
	{
		//Decide to spawn or launching something
		MinionDecide();

		//Decide where to go next
		Vector2 nextPos = new Vector2(StaticGlobal.GetCameraXBound(Camera.main).x * 0.7f * Random.Range(-1f, 1f),
								transform.position.y);
		transform.DOMove(nextPos, Vector2.Distance(transform.position, nextPos) / script.mySpeed).SetEase(Ease.OutBack).
			OnComplete(() => {
				isMoving = false;
				MoveDecide();
			});
	}						//State
	public void MoveToInitial()
	{
		transform.position = bossSpawnPosition;
		transform.DOMove(bossInitialPosition, 2f).SetEase(Ease.InOutQuad)
			.OnComplete(() => {
				startAIing = true;
				//Decide after move to the correct position
				isMoving = false;

				MoveDecide();
			});    //Move to the correct position first
	}                      //State
	#endregion
}
