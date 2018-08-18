﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Formation : MonoBehaviour {

	[System.Serializable]
	public class Minion
	{
		public Transform minion;
		public int numberOfMinion = 1;
	}

	[System.Serializable]
	public enum Orientation
	{
		None = 0,
		LookAtPosition = 1,
		LookAtTarget = 2,
		ToPath = 3
	}

	[System.Serializable]
	public enum MirrorType
	{
		None = 0,
		xAxis = 1,              //x-axis symmetry
		yAxis = 2,              //y-axis symmetry
		Both = 3                //Symmetrical on both axis
	}

	[Header("Formation info")]
	public List<Minion> minionList;
	public float waitBetweenSpawn = 0.5f;
	public float pathTime = 3;
	public bool minionFollowVShape = false;
	[Header("Path info")]
	public List<Vector3> wayPoints;
	public bool closedPath = false;
	public MirrorType pathMirrorType = MirrorType.None;
	public bool loop = false;
	public PathType pathType = PathType.CatmullRom;
	public bool downward = true;
	[Header("Target stuff")]
	public Orientation pathOrientation;
	public Transform lookAtTarget = null;
	public float lookAhead = 0.001f;
	public Vector3 lookAtPosition = Vector3.zero;

	[Header("Transform info")]
	private Collector objectCollector;

	//private Vector3[] truePath;
	//private Vector3[] mirroredPath;

	// Use this for initialization
	private void Awake()
	{
		objectCollector = GameObject.Find("Object_Collector").GetComponent<Collector>();
		wayPoints = GetComponent<DOTweenPath>().wps;
		wayPoints.Insert(0, transform.position);
	}
	public void Start()
	{
		if (pathOrientation == Orientation.LookAtTarget && lookAtTarget == null) {
			lookAtTarget = GameObject.Find("Player").transform;
		}
	}
	#region Resetting stuff
	public void SetZToZero()
	{
		if (wayPoints.Count <= 0) {
			return;
		}

		for (int indx = 0; indx < wayPoints.Count; indx++) {
			wayPoints[indx] = new Vector3 (wayPoints[indx].x, wayPoints[indx].y, 0);
		}
	}
	#endregion

	#region Formation stuff
	public void SummonFormation(Vector3 center)
	{
		if (wayPoints.Count <= 0) {
			return;
		}

		Vector3[] truePath = new Vector3[wayPoints.Count];
		Vector3[] mirroredPath = null;

		if (pathMirrorType != MirrorType.None) {
			mirroredPath = new Vector3[wayPoints.Count];
		}

		//Translate all the path upward
		for (int indx = 0; indx < wayPoints.Count; indx++) {
			//Set the true path
			truePath[indx] = new Vector3(wayPoints[indx].x, wayPoints[indx].y + center.y, 0);

			switch (pathMirrorType) {
				//Also set the mirror path
				case MirrorType.xAxis:
					mirroredPath[indx] = truePath[indx];
					mirroredPath[indx].y *= -1;
					break;

				case MirrorType.yAxis:
					mirroredPath[indx] = truePath[indx];
					mirroredPath[indx].x *= -1;
					break;

				case MirrorType.Both:
					mirroredPath[indx] = truePath[indx];
					mirroredPath[indx] *= -1;
					break;
			}
		}

		StartCoroutine(SpawnCoroutine(truePath, center));

		if (pathMirrorType != MirrorType.None) {
			StartCoroutine(SpawnCoroutine(mirroredPath, center));
		}
	}
	public IEnumerator SpawnCoroutine(Vector3[] path, Vector3 center)
	{
		if (minionFollowVShape == false) {
			foreach (Minion minion in minionList) {
				for (int indx = 0; indx < minion.numberOfMinion; indx++) {
					Transform spawnedObject = Instantiate(minion.minion, center, Quaternion.identity);
					objectCollector.AddChild(spawnedObject);
					spawnedObject.GetComponent<GeneralAI>().followFormation = true;

					DoPath(spawnedObject, path);
					yield return new WaitForSeconds(waitBetweenSpawn);
				}
			}
		} else {
			//Summon the boss first
			Transform spawnedObject = Instantiate(minionList[0].minion, center, Quaternion.identity);
			objectCollector.AddChild(spawnedObject);
			spawnedObject.GetComponent<GeneralAI>().followFormation = true;

			DoPath(spawnedObject, path);
			yield return new WaitForSeconds(waitBetweenSpawn);

			//Then summon the rest of them
			for(int indx = 1; indx < minionList.Count; indx++) {
				Minion minion = minionList[indx];
				Vector3 spawnPos = center + ;
				spawnedObject = Instantiate(minion.minion, spawnPos, Quaternion.identity);
				objectCollector.AddChild(spawnedObject);
				spawnedObject.GetComponent<GeneralAI>().followFormation = true;
			}
		}
	}
	#endregion

	#region Minion stuff
	public IEnumerator RotateObjectWithDelay(Transform target, Vector3 targetRotation, float delay = 0)
	{
		yield return new WaitForSeconds(delay);

		target.eulerAngles = targetRotation;
	}
	public void DoPath(Transform spawnedObject, Vector3[] path)	{
		DOTweenPath script = spawnedObject.gameObject.AddComponent<DOTweenPath>();
		script.hasOnComplete = true;
		script.onComplete.AddListener(() => spawnedObject.GetComponent<GeneralAI>().StartObject(Vector3.zero));
		script.loops = loop == true ? -1 : 1;
		script.wps.AddRange(path);
		script.isClosedPath = closedPath;

		switch (pathOrientation) {
			case Orientation.ToPath:
				script.orientType = DG.Tweening.Plugins.Options.OrientType.ToPath;
				break;
			case Orientation.LookAtPosition:
				script.orientType = DG.Tweening.Plugins.Options.OrientType.LookAtPosition;
				script.lookAtPosition = lookAtPosition;
				break;
			case Orientation.LookAtTarget:
				script.orientType = DG.Tweening.Plugins.Options.OrientType.LookAtTransform;
				script.lookAtTransform = lookAtTarget;
				break;
			case Orientation.None:
				script.orientType = DG.Tweening.Plugins.Options.OrientType.None;
				break;
		}

		script.DOPlay();
		//switch (pathOrientation) {
		//	case Orientation.ToPath:
		//		spawnedObject.DOPath(path, pathTime, pathType, PathMode.TopDown2D)
		//			.SetOptions(closedPath)
		//			.SetEase(Ease.Linear)
		//			.SetLookAt(lookAhead, Vector3.forward, Vector3.right)
		//			.OnComplete(() => spawnedObject.GetComponent<GeneralAI>().StartObject(Vector3.zero))
		//			.SetLoops(loop == true ? -1 : 1, LoopType.Yoyo) //Loop to inifinity and beyond!
		//			;
		//		break;

		//	case Orientation.LookAtPosition:
		//		spawnedObject.DOPath(path, pathTime, pathType, PathMode.TopDown2D)
		//			.SetOptions(closedPath)
		//			.SetEase(Ease.Linear)
		//			.SetLookAt(lookAtPosition)
		//			.OnComplete(() => spawnedObject.GetComponent<GeneralAI>().StartObject(Vector3.zero))
		//			.SetLoops(loop == true ? -1 : 1, LoopType.Yoyo) //Loop to inifinity and beyond!
		//			;
		//		break;

		//	case Orientation.LookAtTarget:
		//		spawnedObject.DOPath(path, pathTime, pathType, PathMode.TopDown2D)
		//			.SetOptions(closedPath)
		//			.SetEase(Ease.Linear)
		//			.SetLookAt(lookAtTarget)
		//			.OnComplete(() => spawnedObject.GetComponent<GeneralAI>().StartObject(Vector3.zero))
		//			.SetLoops(loop == true ? -1 : 1, LoopType.Yoyo) //Loop to inifinity and beyond!
		//			;
		//		break;

		//	case Orientation.None:
		//		Vector3 targetRotation = spawnedObject.eulerAngles;
		//		targetRotation.z = 0;
		//		if (downward == true) {
		//			targetRotation.z = 180;
		//		}

		//		spawnedObject.DOPath(path, pathTime, pathType, PathMode.TopDown2D)
		//			.SetOptions(closedPath)
		//			.SetEase(Ease.Linear)
		//			.OnComplete(() => spawnedObject.GetComponent<GeneralAI>().StartObject(Vector3.zero))
		//			.SetLoops(loop == true ? -1 : 1, LoopType.Yoyo) //Loop to inifinity and beyond!
		//		;

		//		//Rotate afterward  -   Add a delay before rotation to override the look rotation of the tween
		//		StartCoroutine(RotateObjectWithDelay(spawnedObject, targetRotation, 0.00001f));
		//		break;
		//}
	}
	#endregion
}
