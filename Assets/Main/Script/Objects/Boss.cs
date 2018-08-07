using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Boss : Shooter {
	
	[Header("Important UI")]
	protected StatusIndicator healthSI;
	public Sprite icon;
	public Manager sceneManager;

	[Header("More Info")]
	public Transform parentToDestroy;
	
	[System.Serializable]
	public class MinionInfo
	{
		public Transform minion;
		public bool launch = false; //Else move
		public float minionSpeed = 5;
	}
	[Header("Minion")]
	public List<MinionInfo> minionList;
	public List<Transform> minionSpawnedList = new List<Transform>();
	public Transform minionHolder;

	#region Default
	protected override void Awake()
	{
		base.Awake();

		//Find crucial objects
		sceneManager = GameObject.Find("Manager").GetComponent<Manager>();
		healthSI = sceneManager.bossSI;
		healthSI.gameObject.SetActive(true);
		healthSI.ChangeIcon(icon);

		//Set SI
		healthSI.SetValue(curHealth, maxHealth);

		//Holder
		minionHolder = transform.parent.Find("MinionHolder");
	}
	#endregion

	#region Action
	public virtual Transform SpawnMinion(int indx, Vector3 position)
	{
		Transform spawned = Instantiate(minionList[indx].minion, position, Quaternion.identity);
		spawned.parent = minionHolder;			//Set the parent
		GeneralObject script = spawned.GetComponent<GeneralObject>();
		script.SetTag(tag);
		script.mySpeed = minionList[indx].minionSpeed;
		return spawned;
	}
	
	public virtual void MoveMinion(GeneralObject script, Vector2 direction, float speed = -1)
	{
		OrbitAround orbitScript = script.GetComponent<OrbitAround>();
		if (orbitScript != null) {
			//Turn off the orbiting
			orbitScript.enabled = false;
		}
		script.Move(direction, speed == -1 ? script.mySpeed : speed);
	}
	public virtual void MoveMinionTo(GeneralObject script, Vector2 position, float time)
	{
		OrbitAround orbitScript = script.GetComponent<OrbitAround>();
		if (orbitScript != null) {
			//Turn off the orbiting
			orbitScript.enabled = false;
		}
		script.MoveToPosition(position, time);
	}
	public override void ChangeHealth(float amount, int option = 0)
	{
		base.ChangeHealth(amount, option);
		healthSI.SetValue(curHealth, maxHealth);
	}
	public override void Die()
	{
		CreateExplosion();
		if (tag == "Enemy" && GetComponent<Projectile>() == null) {
			StaticGlobal.GetPlayer().GetComponent<Player>().ChangeScore(myScore);
		}

		if (aoeRadius > 0) {
			Collider2D[] withinRadius = Physics2D.OverlapCircleAll(transform.position, aoeRadius);
			foreach (Collider2D collider in withinRadius) {
				GeneralObject otherScript = collider.GetComponent<GeneralObject>();
				if (((tag == "Ally" || tag == "Player") && (collider.tag == "enemy")) &&
					((collider.tag == "Ally" || collider.tag == "Player") && (tag == "enemy"))) {
					if (otherScript.isInvincible == false) {
						otherScript.ChangeHealth(-myDamage * aoePercentage);
					}
				}
			}
		}
		SpawnCollectible(collectibleOption);
		healthSI.gameObject.SetActive(false);

		//Destroy each minion in the spawned list
		foreach(Transform minion in minionSpawnedList) {
			if (minion != null) {
				minion.GetComponent<GeneralObject>().Die();
			}
		}

		Destroy(parentToDestroy == null ? gameObject : parentToDestroy.gameObject);
	}
	#endregion
}
