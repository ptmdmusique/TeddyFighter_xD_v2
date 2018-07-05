using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {

	[Header("Info")]
	public float myValue = 10;
	public List<Transform> explosionList;
	private Collector vfxMaster;

	#region Default
	private void Awake()
	{
		vfxMaster = GameObject.Find("VFX_Collector").GetComponent<Collector>();
	}
	#endregion

	#region Action
	public void Collected()
	{
		CreateExplosion();
		Destroy(gameObject);
	}
	public virtual void CreateExplosion()
	{
		//Create all 
		foreach (Transform explosion in explosionList) {
			vfxMaster.AddChild(Instantiate(explosion, transform.position, Quaternion.identity));
		}
		return;
	}
	#endregion
}
