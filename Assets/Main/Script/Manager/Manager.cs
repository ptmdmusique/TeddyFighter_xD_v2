using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour {

	//Singleton stats
	public static int enemyKilled = 0;
	public static float timer = 0;

	//Report panel
	[Header("Report Panel")]
	[SerializeField] private TextMeshProUGUI enemyKilledText;
	[SerializeField] private TextMeshProUGUI datafragText;
	[SerializeField] private TextMeshProUGUI scoreText;
	[SerializeField] private TextMeshProUGUI timeText;
	[SerializeField] private Image introOutro;

	[Header("Pausing")]
	[SerializeField] private GameObject pausePanel;
	[SerializeField] private float oldTimeScale = 1;
	private bool isPausing;

	//Singleton
	private static Manager instance;

	//Saver
	public List<JsonManager> myJSonManagers;

	[Header("Marker")]
	public Transform bossInitialLocation;
	public StatusIndicator bossSI;

	[Header("Level info")]
	public string nextSceneName;

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

		if (Input.GetButtonDown("Pause") == true) {
			if (isPausing == true) {
				UnPause();
			} else {
				Pause();
			}
			player.isPausing = isPausing;
		}
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
	public void SaveGlobalStat()
	{
		foreach (JsonManager saver in myJSonManagers)
		{
			saver.SaveGlobalStat(); //Save the global stat if there is a defined function
		}
	}
	public void Load()
	{
		Debug.Log("Loaded!");
		foreach (JsonManager loader in myJSonManagers) {
			loader.Load();
		}

		//Check if we load from a new game button
		if (PlayerPrefs.GetInt("NewGame", 0) == 1)
		{
			//If yes, since the loader.Load() has already handled that, we only need to reset the value
			PlayerPrefs.SetInt("NewGame", 0);
		}
	}
		//To call by events and menu stuff
		//TODO: work on loading and saving
	public void RestartScene()
	{
		//TODO: work on loading and saving
		Time.timeScale = 1;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	public void ToMainMenu()
	{
		Time.timeScale = 1;						//Re-adjust the timescale
		SaveGlobalStat();						//Save all the global stats (not the regular player stat)
		SceneManager.LoadScene("Main Menu");
	}
	public void Pause()
	{
		//Stop all the current stuff
		oldTimeScale = Time.timeScale;
		Time.timeScale = 0;

		pausePanel.SetActive(true);
		isPausing = true;
	}
	public void UnPause()
	{
		Time.timeScale = oldTimeScale;

		pausePanel.SetActive(false);
		isPausing = false;
	}
	#endregion

	#region Events
	public void OnPlayerWin()
	{
		//Set up the report panel
		enemyKilledText.text = enemyKilled.ToString();

		string minutes = Mathf.Floor(timer / 60).ToString("00");
		string seconds = (timer % 60).ToString("00");
		string niceTime = string.Format("{0:00}:{1:00}", minutes, seconds);
		timeText.text = niceTime;

		datafragText.text = player.dataFragment.ToString();
		scoreText.text = player.score.ToString();

		//Save all the stats
		Save();
		SaveGlobalStat();

		StartCoroutine(StartPlayerWin());
	}
	public IEnumerator StartPlayerWin()
	{
		//Wait until player press submit to go out
		yield return new WaitUntil(() => Input.GetButtonDown("Submit") == true);

		//Set up save
		PlayerPrefs.SetString("CurrentRound", nextSceneName);

		//Fade out and load new scene
		if (introOutro != null) {
			introOutro.DOFade(1, 2).OnComplete(() => SceneManager.LoadScene(nextSceneName));	
		} else
		{
			SceneManager.LoadScene(nextSceneName);
		}		
	}
	#endregion


	//TODO: Work on save and load
}
