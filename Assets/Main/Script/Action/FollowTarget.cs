using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {

    public Transform target;
	public bool destroyWhenNoTarget = false;
    private Vector3 offset;

	// Use this for initialization
	void Start () {
		if (target != null) {
			offset = target.position - transform.position;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (target != null) {
			transform.position = target.position - offset;
		} else {
			if (destroyWhenNoTarget == true) {
				Destroy(gameObject);
			}
		}
	}
}
