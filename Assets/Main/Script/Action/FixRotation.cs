using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixRotation : MonoBehaviour {

	private Vector3 initialRotation;
	private void Awake()
	{
		initialRotation = transform.eulerAngles;
	}	
	void Update () {
		transform.eulerAngles = initialRotation;	
	}
}
