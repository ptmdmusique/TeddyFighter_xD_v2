using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class PlayerStatJson : JsonManager {

	[Header("Json getter")]
	private string playerPath;
	private string globalPath;
	private string statName = "PlayerStat";
	private string globalstatName = "PlayerGlobalStat";

	[Header("Member to get")]
	private TempPlayer newPlayer;
	private GlobalStat newGlobalStat;

	#region Default
	protected override void Awake()
	{
		base.Awake();
		shortPath = "Database/";

		//Set the path
		playerPath = "Assets/Resources/Database/" + statName + ".json";
		globalPath = "Assets/Resources/Database/" + globalstatName + ".json";
	}
	#endregion

	#region Action
	public override void Load()
	{
		//Get stats
		newPlayer = JsonConvert.DeserializeObject<TempPlayer>(Resources.Load<TextAsset>(shortPath + statName).ToString());

		player.maxHealth = newPlayer.maxHealth;
		player.maxRage = newPlayer.maxRage;

		player.mySpeed = newPlayer.mySpeed;
		player.isInvincible = newPlayer.isInvincible;
		player.myDamage = newPlayer.damage;
		player.canAttack = newPlayer.canAttack;

		player.shieldRageRate = newPlayer.shieldRageRate;
		player.shieldDepleteTime = newPlayer.shieldDepleteTime;

		player.dataFragment = newPlayer.dataFragment;
	}
	public override void Save()
	{
		newPlayer.maxHealth = player.maxHealth;
		newPlayer.maxRage = player.maxRage;

		newPlayer.mySpeed = player.mySpeed;
		newPlayer.isInvincible = player.isInvincible;
		newPlayer.damage = player.myDamage;
		newPlayer.canAttack = player.canAttack;

		newPlayer.shieldRageRate = player.shieldRageRate;
		newPlayer.shieldDepleteTime = player.shieldDepleteTime;

		newPlayer.dataFragment += player.dataFragment;

		using (StreamWriter file = File.CreateText(playerPath)) {
			JsonSerializer serializer = new JsonSerializer();
			serializer.Serialize(file, newPlayer);
		}
	}
	public override void OverwriteSave()
	{
		//Get the object info from the original file 
		newPlayer = JsonConvert.DeserializeObject<TempPlayer>(Resources.Load<TextAsset>(shortPath + statName + "Start").ToString());

		//Overwrite it to the current save file
		using (StreamWriter file = File.CreateText(playerPath))
		{
			JsonSerializer serializer = new JsonSerializer();
			serializer.Serialize(file, newPlayer);
		}
	}
	#endregion

	public override void SaveGlobalStat()
	{
		newGlobalStat = JsonConvert.DeserializeObject<GlobalStat>(Resources.Load<TextAsset>("Database/PlayerGlobalStat").ToString());
		newGlobalStat.totalScore += player.score;
		newGlobalStat.totalPlaytime += Manager.timer;
		newGlobalStat.totalEnemiesKilled += Manager.enemyKilled;

		using (StreamWriter file = File.CreateText(globalPath))
		{
			JsonSerializer serializer = new JsonSerializer();
			serializer.Serialize(file, newGlobalStat);
		}
	}
	public override void LoadGlobalStat()
	{
		newGlobalStat = JsonConvert.DeserializeObject<GlobalStat>(Resources.Load<TextAsset>("Database/GlobalPlayerStat").ToString());
		
	}

	class TempPlayer
	{
		public float mySpeed;
		public bool isInvincible;
		public float damage;
		public bool canAttack;

		public float maxHealth;
		public float maxRage;

		public float shieldRageRate;
		public float shieldDepleteTime;

		public float dataFragment;

		[Newtonsoft.Json.JsonConstructor]
		TempPlayer(float _mySpeed, bool _isInvincible, float _damage,
			bool _canAttack, float _maxHealth, float _curHealth,
			float _maxRage, float _curRage, float _shieldRageRate,
			float _shieldDepleteTime, float _dataFragment, float _score
			)
		{
			mySpeed = _mySpeed;
			isInvincible = _isInvincible;
			damage = _damage;
			canAttack = _canAttack;

			maxHealth = _maxHealth;
			maxRage = _maxRage;

			shieldRageRate = _shieldRageRate;
			shieldDepleteTime = _shieldDepleteTime;

			dataFragment = _dataFragment;
		}

		TempPlayer()
		{
			mySpeed = 15;
			isInvincible = false;
			damage = 500;
			canAttack = true;

			maxHealth = 200;
			maxRage = 200;

			shieldRageRate = 5;
			shieldDepleteTime = 0.5f;

			dataFragment = 0;
		}
	}
	class GlobalStat
	{
		public float totalScore;
		public float totalPlaytime;
		public float totalEnemiesKilled;

		GlobalStat()
		{
			totalScore = 0;
			totalPlaytime = 0;
			totalEnemiesKilled = 0;
		}
	}
}
