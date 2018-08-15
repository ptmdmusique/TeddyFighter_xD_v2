using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Manager : MonoBehaviour {
	//Singleton stats
	public static int enemyKilled = 0;
	public static float timePassed = 0;

	//Report panel
	[SerializeField] private TextMeshProUGUI enemyKilledText;
	[SerializeField] private TextMeshProUGUI datafragText;
	[SerializeField] private TextMeshProUGUI scoreText;
	[SerializeField] private TextMeshProUGUI timeText;

	//Singleton
	private static Manager instance;

	//Saver
	public List<JsonManager> myJSonManagers;

	//Important markers
	public Transform bossInitialLocation;
	public StatusIndicator bossSI;

	private Player player;

	#region Default
	// Use this for initialization
	void Awake () {
		if (instance != null) {
			Destroy(this);
			return;
		}

		instance = this;

		player = StaticGlobal.GetPlayer().GetComponent<Player>();
	}
	#endregion

	#region Actions
	public void Save()
	{
		Debug.Log("Save!");
		foreach(JsonManager saver in myJSonManagers) {
			saver.Save();
		}
	}
	public void Load()
	{
		Debug.Log("Loaded!");
		foreach (JsonManager loader in myJSonManagers) {
			loader.Load();
		}
	}
	#endregion

	#region Events
	public void OnGameEnd()
	{
		//Set up the report panel
		enemyKilledText.text = enemyKilled.ToString();
		timeText.text = timePassed.ToString();
		datafragText.text = player.dataFragment.ToString();
		scoreText.text = player.score.ToString();
	}
	#endregion
}
