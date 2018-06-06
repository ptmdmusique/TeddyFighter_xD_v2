using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralObject : MonoBehaviour {

    public float maxHealth;
    private float curHealth;

    public List<transform> weaponList;
    public List<transform> explosionList;

    private rigidBody myRb;
}
