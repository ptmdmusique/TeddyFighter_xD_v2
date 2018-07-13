using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {

    public Transform target;
    private Vector3 offset;

	// Use this for initialization
	void Start () {
        offset = target.position - transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (target != null) {
			transform.position = target.position - offset;
		}
	}
}
