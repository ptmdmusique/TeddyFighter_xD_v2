using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Collectible : MonoBehaviour {

	[Header("Info")]
	public float minValue = 10;
	public float maxValue = 10;
	public List<Transform> explosionList;
	public float lifeTime = 3;
	private Collector vfxMaster;

	#region Default
	private void Awake()
	{
		vfxMaster = GameObject.Find("VFX_Collector").GetComponent<Collector>();

		if (lifeTime > 0) {
			StartCoroutine(DestroySelf());
		}
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
	public IEnumerator DestroySelf()
	{
		foreach(SpriteRenderer graphic in GetComponentsInChildren<SpriteRenderer>()) {
			graphic.DOFade(0, lifeTime);
		}
		yield return new WaitForSeconds(lifeTime);

		Destroy(gameObject);
	}
	public void FadeOutParticleSystem(ParticleSystem system)
	{
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[system.particleCount]; 
		system.GetParticles(particles);

		for (int p = 0; p < particles.Length; p++) {
			particles[p].GetCurrentColor().
		}
	}
	#endregion
}
