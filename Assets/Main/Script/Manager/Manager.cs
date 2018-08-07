using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {
	//Singleton
	private static Manager instance;

	//Saver
	public List<JsonManager> myJSonManagers;

	//Important markers
	public Transform bossInitialLocation;
	public StatusIndicator bossSI;

	#region Default
	// Use this for initialization
	void Awake () {
		if (instance != null) {
			Destroy(this);
			return;
		}

		instance = this;
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
}
