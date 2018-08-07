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

	[Header("Other singleton")]
	public BGMusicPlayer musicPlayer;

	public void NewGame()
	{
		Debug.Log("Pressed New game");

		//Enable outro
		outro.Play();

		//Load the first scene
		targetScene = "IntroScene";
		Invoke("LoadScene", (float)outro.duration);

		//Destory the player
		Destroy(musicPlayer);
	}

	public void LoadGame()
	{
		Debug.Log("Pressed Load game");

		//Enable outro
		outro.Play();

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
