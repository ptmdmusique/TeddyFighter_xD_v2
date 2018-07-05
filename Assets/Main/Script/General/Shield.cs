using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {

    public class ShieldList
    {
        public ParticleSystem myPS;
        public OrbitAround myOrbitingScript;
    }
    private List<ShieldList> childShield;

	// Use this for initialization
	void Awake () {
        childShield = new List<ShieldList>();

        foreach(Transform child in transform) {
            ShieldList temp = new ShieldList();
            temp.myPS = child.GetComponent<ParticleSystem>();
            temp.myOrbitingScript = child.GetComponent<OrbitAround>();
            childShield.Add(temp);
        }
    }

    public void UpdateSpeed(float newSpeed)     //Update the speed of the shield
    {
        foreach(ShieldList child in childShield) {
            child.myOrbitingScript.speed = newSpeed;
        }
    }
    public void SetActive(bool target = false)
    {
        foreach(ShieldList child in childShield) {
            child.myPS.gameObject.SetActive(target);
        }
    }
}
