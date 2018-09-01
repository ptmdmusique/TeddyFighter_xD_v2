using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class PlayerWeaponJson : JsonManager
{

	[Header("Json getter")]
	private string bulletPath;
	private string patternBulletPath;
	private string patternInfoPath;
	private string weaponPath;

	private string bulletName = "PlayerBullet";
	private string patternBulletName = "PlayerPatternBullet";
	private string patternInfoName = "PlayerPatternInfo";
	private string weaponName = "PlayerWeapon";

	[Header("Member to get")]
	private List<Bullet> bulletList;							//Normal bullet
	private List<Bullet> patternBulletList;						//Pattern bullet
	private List<PatternInfo> patternInfoList;					//Info for the pattern weapon
	private List<Active> weaponActiveList;						//All active list

	// Use this for initialization
	#region Default
	protected override void Awake () {
		base.Awake();

		//Set the path
		patternBulletPath = "Assets/Resources/Database/" + patternBulletName + ".json";
		bulletPath = "Assets/Resources/Database/" + bulletName + ".json";
		patternInfoPath = "Assets/Resources/Database/" + patternInfoName + ".json";
		weaponPath = "Assets/Resources/Database/" + weaponName + ".json";		
	}
	#endregion

	#region Action
	public override void Load()
	{
		//Get the list from the database
		patternBulletList = JsonConvert.DeserializeObject<List<Bullet>>(Resources.Load<TextAsset>(shortPath + patternBulletName).ToString());
		bulletList = JsonConvert.DeserializeObject<List<Bullet>>(Resources.Load<TextAsset>(shortPath + bulletName).ToString());
		patternInfoList = JsonConvert.DeserializeObject<List<PatternInfo>>(Resources.Load<TextAsset>(shortPath + patternInfoName).ToString());
		weaponActiveList = JsonConvert.DeserializeObject<List<Active>>(Resources.Load<TextAsset>(shortPath + weaponName).ToString());

		//Single weapon
		for (int indx = Player.singleMin; indx <= Player.singleMax; indx++) {
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

		//Pattern weapon
		{
			List<Weapon.BulletInfo> curBulletList = player.weaponList[Player.multiGun].myWeapon.bulletList;
			for (int indx = 0; indx < curBulletList.Count; indx++)
			{
				curBulletList[indx].fireRate = patternBulletList[indx].fireRate;
				curBulletList[indx].bulletPerWave = patternBulletList[indx].bulletPerWave;
				curBulletList[indx].waitDuringWave = patternBulletList[indx].waitDuringWave;
				curBulletList[indx].waitAfterWave = patternBulletList[indx].waitAfterWave;
				curBulletList[indx].numberOfWave = patternBulletList[indx].numberOfWave;
				curBulletList[indx].bulletBeforeWait = patternBulletList[indx].bulletBeforeWait;
				curBulletList[indx].bulletSpeed = patternBulletList[indx].bulletSpeed;
				curBulletList[indx].damage = patternBulletList[indx].damage;
				curBulletList[indx].rageCost = patternBulletList[indx].rageCost;
				curBulletList[indx].rageReward = patternBulletList[indx].rageReward;
				curBulletList[indx].lifeTime = patternBulletList[indx].lifeTime;
				curBulletList[indx].aoeRadius = patternBulletList[indx].aoeRadius;
				curBulletList[indx].aoePercentage = patternBulletList[indx].aoePercentage;
				curBulletList[indx].fasterMax = patternBulletList[indx].fasterMax;
				curBulletList[indx].fasterDuration = patternBulletList[indx].fasterDuration;
			}
			player.weaponList[Player.multiGun].isActive = weaponActiveList[Player.multiGun].isActive;
		}

		//Pattern info
		{
			List<PatternWeapon.PatternInfo> curInfoList = ((PatternWeapon) player.weaponList[Player.multiGun].myWeapon).patternInfo;
			for(int indx = 0; indx < curInfoList.Count; indx++)
			{
				switch (patternInfoList[indx].type)
				{
					case 0:
						curInfoList[indx].myType = WeaponPattern.FullArc;
						break;
					case 1:
						curInfoList[indx].myType = WeaponPattern.Random;
						break;
					case 2:
						curInfoList[indx].myType = WeaponPattern.Even;
						break;
					case 3:
						curInfoList[indx].myType = WeaponPattern.Odd;
						break;
				}
				curInfoList[indx].startAngle = patternInfoList[indx].startAngle;
				curInfoList[indx].endAngle = patternInfoList[indx].endAngle;
				curInfoList[indx].rightToLeft = patternInfoList[indx].rightToLeft;
				curInfoList[indx].randomChance = patternInfoList[indx].randomChance;
				curInfoList[indx].angleOffset = patternInfoList[indx].angleOffset;
			}
		}

		//TODO: Work on ultimate weapon
	}
	public override void Save()
	{
		//Single weapon
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

		//Pattern weapon
		{
			List<Weapon.BulletInfo> curBulletList = player.weaponList[Player.multiGun].myWeapon.bulletList;
			for (int indx = 0; indx < curBulletList.Count; indx++)
			{
				patternBulletList[indx].fireRate = curBulletList[indx].fireRate;
				patternBulletList[indx].bulletPerWave = curBulletList[indx].bulletPerWave;
				patternBulletList[indx].waitDuringWave = curBulletList[indx].waitDuringWave;
				patternBulletList[indx].waitAfterWave = curBulletList[indx].waitAfterWave;
				patternBulletList[indx].numberOfWave = curBulletList[indx].numberOfWave;
				patternBulletList[indx].bulletBeforeWait = curBulletList[indx].bulletBeforeWait;
				patternBulletList[indx].bulletSpeed = curBulletList[indx].bulletSpeed;
				patternBulletList[indx].damage = curBulletList[indx].damage;
				patternBulletList[indx].rageCost = curBulletList[indx].rageCost;
				patternBulletList[indx].rageReward = curBulletList[indx].rageReward;
				patternBulletList[indx].lifeTime = curBulletList[indx].lifeTime;
				patternBulletList[indx].aoeRadius = curBulletList[indx].aoeRadius;
				patternBulletList[indx].aoePercentage = curBulletList[indx].aoePercentage;
				patternBulletList[indx].fasterMax = curBulletList[indx].fasterMax;
				patternBulletList[indx].fasterDuration = curBulletList[indx].fasterDuration;
			}
			weaponActiveList[Player.multiGun].isActive = player.weaponList[Player.multiGun].isActive;
		}

		//Pattern info
		{
			List<PatternWeapon.PatternInfo> curInfoList = ((PatternWeapon)player.weaponList[Player.multiGun].myWeapon).patternInfo;
			for (int indx = 0; indx < patternInfoList.Count; indx++)
			{
				switch (curInfoList[indx].myType)
				{
					case WeaponPattern.FullArc:
						patternInfoList[indx].type = 0;
						break;
					case WeaponPattern.Random:
						patternInfoList[indx].type = 1;
						break;
					case WeaponPattern.Even:
						patternInfoList[indx].type = 2;
						break;
					case WeaponPattern.Odd:
						patternInfoList[indx].type = 3;
						break;
				}
				patternInfoList[indx].startAngle = curInfoList[indx].startAngle;
				patternInfoList[indx].endAngle = curInfoList[indx].endAngle;
				patternInfoList[indx].rightToLeft = curInfoList[indx].rightToLeft;
				patternInfoList[indx].randomChance = curInfoList[indx].randomChance;
				patternInfoList[indx].angleOffset = curInfoList[indx].angleOffset;
			}
		}

		using (StreamWriter file = File.CreateText(bulletPath)) {
			JsonSerializer serializer = new JsonSerializer();
			serializer.Serialize(file, bulletList);
		}

		using (StreamWriter file = File.CreateText(patternBulletPath))
		{
			JsonSerializer serializer = new JsonSerializer();
			serializer.Serialize(file, patternBulletList);
		}

		using (StreamWriter file = File.CreateText(patternInfoPath))
		{
			JsonSerializer serializer = new JsonSerializer();
			serializer.Serialize(file, patternInfoList);
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
		patternBulletList = JsonConvert.DeserializeObject<List<Bullet>>(Resources.Load<TextAsset>(shortPath + patternBulletName + "Start").ToString());
		patternInfoList = JsonConvert.DeserializeObject<List<PatternInfo>>(Resources.Load<TextAsset>(shortPath + patternInfoName + "Start").ToString());
		weaponActiveList = JsonConvert.DeserializeObject<List<Active>>(Resources.Load<TextAsset>(shortPath + weaponName + "Start").ToString());

		//Then write it to the current save file
		using (StreamWriter file = File.CreateText(bulletPath))
		{
			JsonSerializer serializer = new JsonSerializer();
			serializer.Serialize(file, bulletList);
		}
		using (StreamWriter file = File.CreateText(patternBulletPath))
		{
			JsonSerializer serializer = new JsonSerializer();
			serializer.Serialize(file, patternBulletList);
		}
		using (StreamWriter file = File.CreateText(patternInfoPath))
		{
			JsonSerializer serializer = new JsonSerializer();
			serializer.Serialize(file, patternInfoList);
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
	public class PatternInfo
	{
		public int type = 0;

		public float startAngle = 0;
		public float endAngle = 0;
		public float angleOffset = 0;

		public bool rightToLeft = false;
		public float randomChance = 0;
	}
	public class Active
	{
		public bool isActive;
	}
}

