using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class MainMenuManager : MonoBehaviour {

	[Header("Cinematic")]
	public PlayableDirector outro;

	[Header("Level")]
	private string targetScene;
	public GameObject areYouSurePanel;
	public GameObject menuUI;

	[Header("Other singleton")]
	public BGMusicPlayer musicPlayer;

	[Header("Save manager")]
	public List<JsonManager> myJSonManagers;

	public void NewGame()
	{
		Debug.Log("Pressed New game");

		//Turn on the question panel and ask for user confirmation
		NewGameActiveFlip();

		//Destory the player
		Destroy(musicPlayer);
	}
	public void NewGameActiveFlip()
	{
		areYouSurePanel.SetActive(!areYouSurePanel.activeSelf);
		menuUI.SetActive(!menuUI.activeSelf);
	}
	public void StartNewGame()
	{
		//Turn off the question panel
		NewGameActiveFlip();

		foreach(JsonManager manager in myJSonManagers)
		{
			manager.OverwriteSave();
		}

		//Enable outro
		outro.Play();

		//Load the first scene
		targetScene = "Level1_1";
		Invoke("LoadScene", (float)outro.duration);
	}
	public void LoadGame()
	{
		Debug.Log("Pressed Load game");

		//Enable outro
		outro.Play();

		//Set up target scene
		targetScene = PlayerPrefs.GetString("CurrentRound", "Level1_1");
		
		//TODO: load scene from save file
		Invoke("LoadScene", (float)outro.duration);

		//Destroy the player
		Destroy(musicPlayer);
	}
	public void Options()
	{
		Debug.Log("Pressed Options");
	}
	public void Highscore()
	{
		Debug.Log("Pressed Highscore");
	}
	public void Exit()
	{
		Debug.Log("Pressed Exit");
		Application.Quit();
	}
	private void LoadScene()
	{
		SceneManager.LoadScene(targetScene);
	}
}
