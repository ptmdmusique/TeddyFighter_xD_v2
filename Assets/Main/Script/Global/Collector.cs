using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{

    [Header("Startup stuff")]
    public bool clearWhenStart = false;

    //Destroy the entire child list
    public void DestroyAll()
    {
        foreach (Transform child in transform) {
            GeneralObject childScript = child.GetComponent<GeneralObject>();
            if (childScript != null) {
                childScript.Die();
            }
        }
    }

    private void Awake()
    {
        if (clearWhenStart == true) {
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
        }
    }

    //Add new child
    public void AddChild(Transform newChild)
    {
        if (newChild.parent != transform) {
            newChild.parent = transform;
        }
    }
}

