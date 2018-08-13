using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public abstract class LevelScript : MonoBehaviour {

	[Header("Level info")]
	public List<MinionInfo> enemyList;
	public PlayableDirector startGame;
	public PlayableDirector endScene;
	public Transform enemySpawnLocation;
	public Transform allySpawnLocation;
	protected float screenHeight;
	protected float screenWidth;
	protected Collector objectCollector;

	[Header("Script into")]
	public Player player;
	public float spawnWait = 0.5f;
	protected int curCheckpoint;
	protected Coroutine levelScript;
	protected Coroutine spawnMinionCoroutine;
	protected Coroutine spawnFormationCoroutine;
	protected bool isWaiting = false;                   //Are we waiting for something?
	protected bool needToUpdateCheckpoint = false;		//Did we reach a checkpoint but didn't update it?

	[System.Serializable]
	public class MinionInfo
	{
		public Transform minion;
		public int chance;
	}

	#region Default
	protected virtual void Awake()
	{
		screenHeight = StaticGlobal.GetCameraHeight(Camera.main);
		screenWidth = StaticGlobal.GetCameraWidth(Camera.main);
		objectCollector = GameObject.Find("Object_Collector").GetComponent<Collector>();
	}
	#endregion

	#region Action
	public void StartLevel()
	{
		if (player == null) {
			player = StaticGlobal.GetPlayer().GetComponent<Player>();
		}
		if (levelScript == null) { 
			levelScript = StartCoroutine(LevelEvent());
		}
		if (spawnMinionCoroutine == null) {
			spawnMinionCoroutine = StartCoroutine(SpawnMinion());
		}
		if (spawnFormationCoroutine == null) {
			spawnMinionCoroutine = StartCoroutine(SpawnFormation());
		}
	}
	public abstract IEnumerator LevelEvent();
	public IEnumerator SpawnMinion()
	{
		yield return new WaitUntil(() => isWaiting == false);            //Temporarily stop the coroutine if necessary
		foreach(MinionInfo minion in enemyList) {
			int chance = Random.Range(0, 100);
			if (chance < minion.chance) {
				//If the randomized chance is correct then spawn it
				Transform spawned = Instantiate(minion.minion, new Vector2(Random.Range(-screenWidth, screenWidth) * 0.8f, enemySpawnLocation.position.y), Quaternion.identity);
				objectCollector.AddChild(spawned);
			}
		}
		yield return new WaitForSeconds(spawnWait);
		spawnMinionCoroutine = StartCoroutine(SpawnMinion());
	}
	public IEnumerator SpawnFormation()
	{
		yield return new WaitUntil(() => isWaiting == false);            //Temporarily stop the coroutine if necessary

		//TODO: Need to work on this
		yield return new WaitForSeconds(spawnWait);
		spawnFormationCoroutine = StartCoroutine(SpawnFormation());
	}
	#endregion

	//START GAME -> EVENTS -> END GAME
}
