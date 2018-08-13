using System.Collections;
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
	public float minSpeed = 1;
	public float pathTime = 3;
	private List<Transform> spawnedList;
	[Header("Path info")]
	public Vector3[] wayPoints;
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
	}
	public void Start()
	{
		RebuildList();
		if (pathOrientation == Orientation.LookAtTarget && lookAtTarget == null) {
			lookAtTarget = GameObject.Find("Player").transform;
		}
	}

	#region Resetting stuff
	public void SetZToZero()
	{
		if (wayPoints.Length <= 0) {
			return;
		}

		for (int indx = 0; indx < wayPoints.Length; indx++) {
			wayPoints[indx].z = 0;
		}
	}
	public void RebuildList()
	{
		FindMinSpeed();
	}
	public void FindMinSpeed()
	{
		//Find the minimum speed of the objects in the current list
		foreach(Minion minion in minionList) {
			float speed = minion.minion.GetComponent<GeneralObject>().mySpeed;
			if (speed < minSpeed) {
				minSpeed = speed;
			}
		}
	}
	#endregion

	#region Formation stuff
	public void SummonFormation(Vector3 center)
	{
		if (wayPoints.Length <= 0) {
			return;
		}

		Vector3[] truePath = new Vector3[wayPoints.Length];
		Vector3[] mirroredPath = null;

		if (pathMirrorType != MirrorType.None) {
			mirroredPath = new Vector3[wayPoints.Length];
		}

		//Translate all the path upward
		for (int indx = 0; indx < wayPoints.Length; indx++) {
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
		foreach(Minion minion in minionList) {
			for(int indx = 0; indx < minion.numberOfMinion; indx++) {
				Transform spawnedObject = Instantiate(minion.minion, path[0], Quaternion.identity);
				objectCollector.AddChild(spawnedObject);

				Tween tween2;
				switch (pathOrientation) {
					case Orientation.ToPath:
						tween2 = spawnedObject.DOPath(path, pathTime, pathType, PathMode.TopDown2D)
						.SetOptions(closedPath)
						.SetEase(Ease.Linear)
						.SetLookAt(lookAhead, Vector3.forward, Vector3.right)
						;
						if (loop == true) {
							//Loop to inifinity and beyond!
							tween2.SetLoops(-1, LoopType.Yoyo);
						}
						break;

					case Orientation.LookAtPosition:
						tween2 = spawnedObject.DOPath(path, pathTime, pathType, PathMode.TopDown2D)
							.SetOptions(closedPath)
							.SetEase(Ease.Linear)
							.SetLookAt(lookAtPosition)
							;
						if (loop == true) {
							//Loop to inifinity and beyond!
							tween2.SetLoops(-1, LoopType.Yoyo);
						}
						break;

					case Orientation.LookAtTarget:
						tween2 = spawnedObject.DOPath(path, pathTime, pathType, PathMode.TopDown2D)
							.SetOptions(closedPath)
							.SetEase(Ease.Linear)
							.SetLookAt(lookAtTarget)
							;
						if (loop == true) {
							//Loop to inifinity and beyond!
							tween2.SetLoops(-1, LoopType.Yoyo);
						}
						break;

					case Orientation.None:
						Vector3 targetRotation = spawnedObject.eulerAngles;
						targetRotation.z = 0;
						if (downward == true) {
							targetRotation.z = 180;
						}

						tween2 = spawnedObject.DOPath(path, pathTime, pathType, PathMode.TopDown2D)
							.SetOptions(closedPath)
							.SetEase(Ease.Linear)
							;
						if (loop == true) {
							//Loop to inifinity and beyond!
							tween2.SetLoops(-1, LoopType.Yoyo);
						}

						//Rotate afterward  -   Add a delay before rotation to override the look rotation of the tween
						StartCoroutine(RotateObjectWithDelay(spawnedObject, targetRotation, 0.00001f));
						break;
				}

				//Add the object to the spawned list
				spawnedList.Add(spawnedObject);

				yield return new WaitForSeconds(waitBetweenSpawn);
			}
		}

		//Afterward, launch all the object
		LaunchObjects();
	}
	#endregion

	#region Minion stuff
	public IEnumerator RotateObjectWithDelay(Transform target, Vector3 targetRotation, float delay = 0)
	{
		yield return new WaitForSeconds(delay);

		target.eulerAngles = targetRotation;
	}
	public void LaunchObjects()
	{
		foreach (Transform minion in spawnedList) {
			minion.GetComponent<GeneralAI>().StartObject();
		}
	}
	#endregion
}
