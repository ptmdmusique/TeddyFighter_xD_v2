using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonManager: MonoBehaviour {
	//Player
	protected Player player;

	#region Default
	protected virtual void Awake()
	{
		//Get the player
		player = StaticGlobal.GetPlayer().GetComponent<Player>();
	}
	#endregion

	#region Action
	public virtual void Load(){}
	public virtual void Save() { }
	#endregion
}
