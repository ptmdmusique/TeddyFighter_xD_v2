using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public enum FormationPosition
{
	Left = 0,
	Right = 1,
	Top = 2,
	Bottom = 3
}

public abstract class LevelScript : MonoBehaviour {

	[Header("Level info")]
	public List<MinionInfo> enemyList;
	public List<FormationInfo> formationList;
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
	protected bool needToUpdateCheckpoint = false;      //Did we reach a checkpoint but didn't update it?

	[Header("Wave info")]
	public float formationSpawnPositionModifier = 1.1f;
	public float minionSpawnRangeModifier = 0.8f;

	[System.Serializable]
	public class MinionInfo
	{
		public Transform minion;
		public int chance;
	}
	[System.Serializable]
	public class FormationInfo
	{
		[Header("Formation Info")]
		public Formation formation;
		public int chance;
		public FormationPosition sideToSpawn = FormationPosition.Top;
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
				Transform spawned = Instantiate(minion.minion, new Vector2(Random.Range(-screenWidth / 2.0f, screenWidth / 2.0f) * minionSpawnRangeModifier, enemySpawnLocation.position.y), Quaternion.identity);
				objectCollector.AddChild(spawned);
			}
		}
		yield return new WaitForSeconds(spawnWait);
		spawnMinionCoroutine = StartCoroutine(SpawnMinion());
	}
	public IEnumerator SpawnFormation()
	{
		yield return new WaitUntil(() => isWaiting == false);            //Temporarily stop the coroutine if necessary
		foreach(FormationInfo info in formationList) {
			int chance = Random.Range(0, 100);
			Vector3 spawnPosition;

			if (chance < info.chance) {
				//If the randomized chance is correct then spawn it
				switch (info.sideToSpawn) {
					case FormationPosition.Top:
						//10% above the border
						info.formation.SummonFormation(spawnPosition = new Vector3(Random.Range(-screenWidth, screenWidth) * minionSpawnRangeModifier, screenHeight / 2.0f * formationSpawnPositionModifier,0));
						Debug.Log(spawnPosition);
						break;
					case FormationPosition.Bottom:
						//10% bellow the border
						info.formation.SummonFormation(new Vector3(Random.Range(-screenWidth, screenWidth) * minionSpawnRangeModifier, -screenHeight / 2.0f * formationSpawnPositionModifier, 0));
						break;
					case FormationPosition.Left:
						//10% to the left of border
						info.formation.SummonFormation(new Vector3(-screenWidth / 2.0f * formationSpawnPositionModifier, Random.Range(-screenWidth, screenWidth) * minionSpawnRangeModifier, 0));
						break;
					case FormationPosition.Right:
						//10% to the right of border
						info.formation.SummonFormation(new Vector3(screenWidth / 2.0f * formationSpawnPositionModifier, Random.Range(-screenWidth, screenWidth) * minionSpawnRangeModifier, 0));
						break;
				}
			}
		}

		//TODO: Need to work on this
		yield return new WaitForSeconds(spawnWait * 2);		//Wait double the amount of time for formation spawning
		spawnFormationCoroutine = StartCoroutine(SpawnFormation());
	}
	#endregion

	//START GAME -> EVENTS -> END GAME
}
