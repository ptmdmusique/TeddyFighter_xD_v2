using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonManager: MonoBehaviour {
	//Player
	public Player player;
	protected string shortPath;

	#region Default
	protected virtual void Awake()
	{
		//Get the player
		Transform temp = StaticGlobal.GetPlayer();
		if (temp != null) { 
			player = temp.GetComponent<Player>();
		}
		shortPath = "Database/";
	}
	#endregion

	#region Action
	public virtual void Load(){}
	public virtual void Save() { }
	public virtual void OverwriteSave() { }		//Overwrite the current save file with the default file
	public virtual void SaveGlobalStat() { }
	public virtual void LoadGlobalStat() { }
	#endregion
}
