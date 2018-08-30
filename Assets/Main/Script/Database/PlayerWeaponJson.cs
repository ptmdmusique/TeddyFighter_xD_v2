using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class PlayerWeaponJson : JsonManager
{

	[Header("Json getter")]
	private string bulletPath;
	private string weaponPath;
	private string bulletName = "PlayerBullet";
	private string weaponName = "PlayerWeapon";

	[Header("Member to get")]
	private List<Bullet> bulletList;
	private List<Active> weaponActiveList;

	// Use this for initialization
	#region Default
	protected override void Awake () {
		base.Awake();

		//Set the path
		bulletPath = "Assets/Resources/Database/" + bulletName + ".json";
		weaponPath = "Assets/Resources/Database/" + weaponName + ".json";

		//Get the list from the database
		bulletList = JsonConvert.DeserializeObject<List<Bullet>>(Resources.Load<TextAsset>(shortPath + bulletName).ToString());
		weaponActiveList = JsonConvert.DeserializeObject<List<Active>>(Resources.Load<TextAsset>(shortPath + weaponName).ToString());
	}
	#endregion

	#region Action
	public override void Load()
	{
		for(int indx = Player.singleMin; indx <= Player.singleMax; indx++) {
			List<Weapon.BulletInfo> curBulletList = player.weaponList[indx].myWeapon.bulletList;
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

			player.weaponList[indx].isActive = weaponActiveList[indx].isActive;
		}

		//TODO: Work on pattern and ultimate weapon
	}
	public override void Save()
	{
		for (int indx = Player.singleMin; indx <= Player.singleMax; indx++) {
			List<Weapon.BulletInfo> curBulletList = player.weaponList[indx].myWeapon.bulletList;
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

			weaponActiveList[indx].isActive = player.weaponList[indx].isActive;
		}

		using (StreamWriter file = File.CreateText(bulletPath)) {
			JsonSerializer serializer = new JsonSerializer();
			serializer.Serialize(file, bulletList);
		}

		using (StreamWriter file = File.CreateText(weaponPath)) {
			JsonSerializer serializer = new JsonSerializer();
			serializer.Serialize(file, weaponActiveList);
		}
	}
	public override void OverwriteSave()
	{
		//Get the objects from the original file 
		bulletList = JsonConvert.DeserializeObject<List<Bullet>>(Resources.Load<TextAsset>(shortPath + bulletName + "Start").ToString());
		weaponActiveList = JsonConvert.DeserializeObject<List<Active>>(Resources.Load<TextAsset>(shortPath + weaponName + "Start").ToString());

		//Then write it to the current save file
		using (StreamWriter file = File.CreateText(bulletPath))
		{
			JsonSerializer serializer = new JsonSerializer();
			serializer.Serialize(file, bulletList);
		}

		using (StreamWriter file = File.CreateText(weaponPath))
		{
			JsonSerializer serializer = new JsonSerializer();
			serializer.Serialize(file, weaponActiveList);
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
	public class Active
	{
		public bool isActive;
	}
}

