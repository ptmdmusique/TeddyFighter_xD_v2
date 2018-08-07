using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveToPos : MonoBehaviour {

	public Vector3 targetVector;
	public bool startMoveTo = false;
	public float moveToTime = 1f;

	private void Update()
	{
		if (startMoveTo == true) {
			startMoveTo = false;
			MoveTo();
		}
	}

	public void MoveTo()
	{
		transform.DOMove(targetVector, moveToTime);
	}
}
