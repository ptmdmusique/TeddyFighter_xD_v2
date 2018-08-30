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
	public float pathTime = 3;
	public bool minionFollowVShape = false;
	public float vDistance = 4;

	[Header("Path info")]
	public bool closedPath = false;
	public MirrorType pathMirrorType = MirrorType.None;
	public bool loop = false;
	public PathType pathType = PathType.CatmullRom;
	public bool downward = true;
	protected DOTweenPath pathScript;
	[HideInInspector] public List<Vector3> wayPoints;

	[Header("Target stuff")]
	public Orientation pathOrientation;
	public Transform lookAtTarget = null;
	public float lookAhead = 0.001f;
	public Vector3 lookAtPosition = Vector3.zero;

	[Header("Transform info")]
	protected Collector objectCollector;

	#region Default
	private void Awake()
	{
		objectCollector = GameObject.Find("Object_Collector").GetComponent<Collector>();

		pathScript = GetComponent<DOTweenPath>();
		wayPoints = pathScript.wps;
		//wayPoints.Insert(0, transform.position);
	}
	public void Start()
	{
		if (pathOrientation == Orientation.LookAtTarget && lookAtTarget == null) {
			lookAtTarget = GameObject.Find("Player").transform;
		}
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.K) == true) {
			SummonFormation(transform.position);
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

					DoPath(spawnedObject, SetupPath(center, path));
					yield return new WaitForSeconds(waitBetweenSpawn);
				}
			}
		} else {
			//If summon in formation first then there is no delay between each summon

			//Summon the boss first
			Transform spawnedObject = Instantiate(minionList[0].minion, center, Quaternion.identity);
			objectCollector.AddChild(spawnedObject);
			spawnedObject.GetComponent<GeneralAI>().followFormation = true;

			DoPath(spawnedObject, SetupPath(center, path));
			//yield return new WaitForSeconds(waitBetweenSpawn);

			float rotation = spawnedObject.eulerAngles.z;			//Get the rotation vector of the boss
			Vector3 dirVector = Vector3.down;                       //This is the original translational vector that we will use to locate the spawn line (beneath the boss)
			//We then need to rotate it to find the true vector
			dirVector = StaticGlobal.RotateVectorByAmount_2D(rotation, dirVector) * vDistance;
			//This is the vector that determines distance from the center of the spawning line, act as the speration of each minion
			Vector3 distanceVector = Vector3.left * vDistance;      //Also act as a step for each increment 
			//Rotate that to find the true vector too
			distanceVector = StaticGlobal.RotateVectorByAmount_2D(rotation, distanceVector);

			//The center of the first lane
			Vector3 laneCenter = center + dirVector;

			//Then summon the rest of them
			for(int indx = 1; indx < minionList.Count; indx++) {
				Minion minion = minionList[indx];
				Vector3 spawnPos = laneCenter - distanceVector * (minion.numberOfMinion - 1) / 2.0f;	

				for(int indx2 = 0; indx2 < minion.numberOfMinion; indx2++) {
					spawnedObject = Instantiate(minion.minion, spawnPos, Quaternion.identity);
					objectCollector.AddChild(spawnedObject);
					spawnedObject.GetComponent<GeneralAI>().followFormation = true;
					DoPath(spawnedObject, SetupPath(spawnedObject.position, path));								//Re-set up the path

					//Move on to the next position in lane
					spawnPos += distanceVector;
				}

				//Move on to the next lane
				laneCenter += dirVector;
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
		switch (pathOrientation) {
			case Orientation.ToPath:
				spawnedObject.DOPath(path, pathTime, pathType, PathMode.TopDown2D)
					.SetOptions(closedPath)
					.SetLookAt(lookAhead, Vector3.forward, Vector3.right)
					.OnComplete(() => spawnedObject.GetComponent<GeneralAI>().StartObject(Vector3.zero))
					.SetLoops(loop == true ? -1 : 1, LoopType.Yoyo) //Loop to inifinity and beyond!
					;
				break;

			case Orientation.LookAtPosition:
				spawnedObject.DOPath(path, pathTime, pathType, PathMode.TopDown2D)
					.SetOptions(closedPath)
					.SetLookAt(lookAtPosition)
					.OnComplete(() => spawnedObject.GetComponent<GeneralAI>().StartObject(Vector3.zero))
					.SetLoops(loop == true ? -1 : 1, LoopType.Yoyo) //Loop to inifinity and beyond!
					;
				break;

			case Orientation.LookAtTarget:
				spawnedObject.DOPath(path, pathTime, pathType, PathMode.TopDown2D)
					.SetOptions(closedPath)
					.SetLookAt(lookAtTarget)
					.OnComplete(() => spawnedObject.GetComponent<GeneralAI>().StartObject(Vector3.zero))
					.SetLoops(loop == true ? -1 : 1, LoopType.Yoyo) //Loop to inifinity and beyond!
					;
				break;

			case Orientation.None:
				Vector3 targetRotation = spawnedObject.eulerAngles;
				targetRotation.z = 0;
				if (downward == true) {
					targetRotation.z = 180;
				}

				spawnedObject.DOPath(path, pathTime, pathType, PathMode.TopDown2D)
					.SetOptions(closedPath)
					.OnComplete(() => spawnedObject.GetComponent<GeneralAI>().StartObject(Vector3.zero))
					.SetLoops(loop == true ? -1 : 1, LoopType.Yoyo) //Loop to inifinity and beyond!
				;

				//Rotate afterward  -   Add a delay before rotation to override the look rotation of the tween
				StartCoroutine(RotateObjectWithDelay(spawnedObject, targetRotation, 0.00001f));
				break;
		}
	}
	protected Vector3[] SetupPath(Vector3 center, Vector3[] originalPath)
	{
		//Set up the path relative to the center
		Vector3[] returnPath = new Vector3[originalPath.Length];

		returnPath[0] = center;		//Offset the first waypoint
		for (int indx = 1; indx < returnPath.Length; indx++) {
			//The current waypoint is the previous waypoint translated by the difference among the original wps
			returnPath[indx] = returnPath[indx - 1] + (originalPath[indx] - originalPath[indx - 1]);
		}

		return returnPath;
	}
	#endregion
}
