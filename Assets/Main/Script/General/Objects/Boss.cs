using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Boss : Shooter {

	[Header("Important UI")]
	public StatusIndicator healthSI;
	public Sprite icon;
	public Manager sceneManager;

	[Header("Minion")]
	public List<Transform> minionList;


	#region Default
	protected override void Awake()
	{
		base.Awake();

		//Find crucial objects
		sceneManager = GameObject.Find("Manager").GetComponent<Manager>();
		healthSI = sceneManager.bossSI; 
	}
	#endregion

	#region Action
	public virtual Transform SpawnMinion(int indx, Vector3 position)
	{
		Transform spawned = Instantiate(minionList[indx], position, Quaternion.identity);
		objectMaster.AddChild(spawned);
		return spawned;
	}
	public virtual void MoveToPosition(Vector3 target, float time, Ease easeType = Ease.Linear)
	{
		transform.DOMove(target, time).SetEase(easeType);
	}
	#endregion
}
