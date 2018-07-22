using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitAround : MonoBehaviour {

    public Transform target;
    public float speed = 1;
	public bool followTarget = true;		//Recommended to be true

	private Vector2 offset;

	#region Default
	private void Awake()
	{
		UpdateOffset();
	}
	public void LateUpdate () {
		if (followTarget == true) {
			//Move first
			transform.position = target.position + (Vector3)offset; // ---- This will ensure we keep the right distance

			//Then rotate
			transform.RotateAround(target.position, Vector3.forward, speed * Time.deltaTime);

			//And finally update the relative offset
			offset = transform.position - target.position;
		} else {
			transform.RotateAround(target.position, Vector3.forward, speed * Time.deltaTime);
		}
	}
	private void OnEnable()
	{
		UpdateOffset();
	}
	#endregion

	#region Action
	public void UpdateOffset()
	{
		if (target != null) {
			offset = -target.position + transform.position;
		}
	}
	#endregion
}
