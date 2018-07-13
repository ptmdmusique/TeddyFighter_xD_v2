using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToVector : MonoBehaviour {

	public float speed = 999;
	public Vector3 targetVector;

	// Update is called once per frame
	void Update () {
		Quaternion rotation = Quaternion.Euler(targetVector);
		transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * speed);
		if (Quaternion.Angle(Quaternion.Euler(targetVector), transform.rotation) < 0.01f) {
			transform.rotation = rotation;
		}
	}
}
