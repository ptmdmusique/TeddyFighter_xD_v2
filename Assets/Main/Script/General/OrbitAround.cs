using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitAround : MonoBehaviour {

    public Transform target;
    public float speed = 1;

    // Update is called once per frame
    void Update () {
        transform.RotateAround(target.position, Vector3.forward, speed * Time.deltaTime);
    }
}
