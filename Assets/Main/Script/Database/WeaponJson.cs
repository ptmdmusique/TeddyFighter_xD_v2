using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class WeaponJson : MonoBehaviour {

	[Header("Json getter")]
	private string path;
	private string jsonString;

	[Header("Member to get")]
	private Player player;
	private List<Bullet> bulletList;

	// Use this for initialization
	#region Default
	void Start () {
		//Set the path
		path = "Assets/Resources/Database/PlayerWeapon.json";

		//Get the list from the database
		bulletList = JsonConvert.DeserializeObject<List<Bullet>>(Resources.Load<TextAsset>("Database/PlayerWeapon").ToString());

		//Get the player
		player = StaticGlobal.GetPlayer().GetComponent<Player>();
	}
	private void FixedUpdate()
	{
		if (Input.GetKeyDown(KeyCode.L) == true) {
			Debug.Log("Loaded!");
			LoadToPlayer();
		} else if (Input.GetKeyDown(KeyCode.U) == true) {
			Debug.Log("Unloaded!");
			WriteFromPlayer();
		}
	}
	#endregion

	#region Action
	public void LoadToPlayer()
	{
		for(int indx = Player.singleMin; indx <= Player.singleMax; indx++) {
			List<Weapon.Bullet> curBulletList = player.weaponList[indx].myWeapon.bulletList;
			for (int indx2 = 0; indx2 < curBulletList.Count; indx2++) {
				curBulletList[indx2].fireRate = bulletList[indx2].fireRate;
				curBulletList[indx2].bulletPerWave = bulletList[indx2].bulletPerWave;
				curBulletList[indx2].waitDuringWave = bulletList[indx2].waitDuringWave;
				curBulletList[indx2].waitAfterWave = bulletList[indx2].waitAfterWave;
				curBulletList[indx2].numberOfWave = bulletList[indx2].numberOfWave;
				curBulletList[indx2].bulletBeforeWait = bulletList[indx2].bulletBeforeWait;
				curBulletList[indx2].bulletSpeed = bulletList[indx2].bulletSpeed;
				curBulletList[indx2].damage = bulletList[indx2].damage;
				curBulletList[indx2].rageCost = bulletList[indx2].rageCost;
				curBulletList[indx2].rageReward = bulletList[indx2].rageReward;
				curBulletList[indx2].lifeTime = bulletList[indx2].lifeTime;
				curBulletList[indx2].aoeRadius = bulletList[indx2].aoeRadius;
				curBulletList[indx2].aoePercentage = bulletList[indx2].aoePercentage;
				curBulletList[indx2].fasterMax = bulletList[indx2].fasterMax;
				curBulletList[indx2].fasterDuration = bulletList[indx2].fasterDuration;
			}
		}
	}
	public void WriteFromPlayer()
	{
		for (int indx = Player.singleMin; indx <= Player.singleMax; indx++) {
			List<Weapon.Bullet> curBulletList = player.weaponList[indx].myWeapon.bulletList;
			for (int indx2 = 0; indx2 < curBulletList.Count; indx2++) {
				bulletList[indx2].fireRate = curBulletList[indx2].fireRate;
				bulletList[indx2].bulletPerWave = curBulletList[indx2].bulletPerWave;
				bulletList[indx2].waitDuringWave = curBulletList[indx2].waitDuringWave;
				bulletList[indx2].waitAfterWave = curBulletList[indx2].waitAfterWave;
				bulletList[indx2].numberOfWave = curBulletList[indx2].numberOfWave;
				bulletList[indx2].bulletBeforeWait = curBulletList[indx2].bulletBeforeWait;
				bulletList[indx2].bulletSpeed = curBulletList[indx2].bulletSpeed;
				bulletList[indx2].damage = curBulletList[indx2].damage;
				bulletList[indx2].rageCost = curBulletList[indx2].rageCost;
				bulletList[indx2].rageReward = curBulletList[indx2].rageReward;
				bulletList[indx2].lifeTime = curBulletList[indx2].lifeTime;
				bulletList[indx2].aoeRadius = curBulletList[indx2].aoeRadius;
				bulletList[indx2].aoePercentage = curBulletList[indx2].aoePercentage;
				bulletList[indx2].fasterMax = curBulletList[indx2].fasterMax;
				bulletList[indx2].fasterDuration = curBulletList[indx2].fasterDuration;
			}
		}

		using (StreamWriter file = File.CreateText(path)) {
			JsonSerializer serializer = new JsonSerializer();
			serializer.Serialize(file, bulletList);
		}
	}
	#endregion

	//Temp class
	public class Bullet
	{
		[Header("Gun relating info")]
		public float fireRate = 5;          //Bullet per second: 1/fireRate = reloadTime
		public int bulletPerWave = 3;
		public float waitDuringWave = 3;
		public float waitAfterWave = 0.5f;
		public int numberOfWave = 1;
		public int bulletBeforeWait = -1;
		[Header("Bullet relating info")]
		public float bulletSpeed = -1;
		public float damage = 10;
		public float rageCost = 10;
		public float rageReward = -1;
		public float lifeTime = -1;
		[Header("Additional damage")]
		public float aoeRadius = 0;
		public float aoePercentage = 0;
		[Header("Speed stuff")]
		public float fasterMax = -1;
		public float fasterDuration = -1;

		public Transform bullet;

		[Newtonsoft.Json.JsonConstructor]
		public Bullet(float _fireRate, int _bulletPerWave, float _waitDuringWave,
				float _waitAfterWave, int _numberOfWave, int _bulletBeforeWait,
				float _bulletSpeed, float _damage, float _rageCost, float _rageReward,
				float _lifeTime, float _aoeRadius, float _aoePercentage, float _fasterMax,
				float _fasterDuration
			)
		{
			fireRate = _fireRate;
			bulletPerWave = _bulletPerWave;
			waitDuringWave = _waitDuringWave;
			waitAfterWave = _waitAfterWave;
			numberOfWave = _numberOfWave;
			bulletBeforeWait = _bulletBeforeWait;
			bulletSpeed = _bulletSpeed;
			damage = _damage;
			rageCost = _rageCost;
			rageReward = _rageReward;
			lifeTime = _lifeTime;
			aoeRadius = _aoeRadius;
			aoePercentage = _aoePercentage;
			fasterMax = _fasterMax;
			fasterDuration = _fasterDuration;
		}
	}

}

