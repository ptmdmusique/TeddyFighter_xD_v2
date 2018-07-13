using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class PlayerStatJson : JsonManager {

	[Header("Json getter")]
	private string playerPath;

	[Header("Member to get")]
	private TempPlayer newPlayer;

	#region Default
	protected override void Awake()
	{
		base.Awake();

		//Set the path
		playerPath = "Assets/Resources/Database/PlayerStat.json";

		//Get stats
		newPlayer = JsonConvert.DeserializeObject<TempPlayer>(Resources.Load<TextAsset>("Database/PlayerStat").ToString());
	}
	#endregion

	#region Action
	public override void Load()
	{
		player.maxHealth = newPlayer.maxHealth;
		player.curHealth = newPlayer.curHealth;
		player.maxRage = newPlayer.maxRage;
		player.curRage = newPlayer.curRage;

		player.mySpeed = newPlayer.mySpeed;
		player.isInvincible = newPlayer.isInvincible;
		player.myDamage = newPlayer.damage;
		player.canAttack = newPlayer.canAttack;

		player.shieldRageRate = newPlayer.shieldRageRate;
		player.shieldDepleteTime = newPlayer.shieldDepleteTime;

		player.dataFragment = newPlayer.dataFragment;
		player.score = newPlayer.score;
	}
	public override void Save()
	{
		newPlayer.maxHealth = player.maxHealth;
		newPlayer.curHealth = player.curHealth;
		newPlayer.maxRage = player.maxRage;
		newPlayer.curRage = player.curRage;

		newPlayer.mySpeed = player.mySpeed;
		newPlayer.isInvincible = player.isInvincible;
		newPlayer.damage = player.myDamage;
		newPlayer.canAttack = player.canAttack;

		newPlayer.shieldRageRate = player.shieldRageRate;
		newPlayer.shieldDepleteTime = player.shieldDepleteTime;

		newPlayer.dataFragment = player.dataFragment;
		newPlayer.score = player.score;

		using (StreamWriter file = File.CreateText(playerPath)) {
			JsonSerializer serializer = new JsonSerializer();
			serializer.Serialize(file, newPlayer);
		}
	}
	#endregion

	class TempPlayer
	{
		public float mySpeed;
		public bool isInvincible;
		public float damage;
		public bool canAttack;

		public float maxHealth;
		public float curHealth;
		public float maxRage;
		public float curRage;

		public float shieldRageRate;
		public float shieldDepleteTime;

		public float dataFragment;
		public float score;

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
			curHealth = _curHealth;
			maxRage = _maxRage;
			curRage = _curRage;

			shieldRageRate = _shieldRageRate;
			shieldDepleteTime = _shieldDepleteTime;

			dataFragment = _dataFragment;
			score = _score;
		}
	}
}
