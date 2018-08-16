using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class Manager : MonoBehaviour {
	//Singleton stats
	public static int enemyKilled = 0;
	public static float timer = 0;

	//Report panel
	[SerializeField] private TextMeshProUGUI enemyKilledText;
	[SerializeField] private TextMeshProUGUI datafragText;
	[SerializeField] private TextMeshProUGUI scoreText;
	[SerializeField] private TextMeshProUGUI timeText;
	[SerializeField] private Image introOutro; 

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
	private void Update()
	{
		timer += Time.deltaTime;
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

		string minutes = Mathf.Floor(timer / 60).ToString("00");
		string seconds = (timer % 60).ToString("00");
		string niceTime = string.Format("{0:00}:{1:00}", minutes, seconds);
		timeText.text = niceTime;

		datafragText.text = player.dataFragment.ToString();
		scoreText.text = player.score.ToString();

		StartCoroutine(StartEndGame());
	}
	public IEnumerator StartEndGame()
	{
		//Wait until player press submit to go out
		yield return new WaitUntil(() => Input.GetButtonDown("Submit") == true);
		
		if (introOutro != null) {
			introOutro.DOFade(1, 2);	
		}

		//Load new scene
	}
	#endregion


	//TODO: Work on save and load
}
