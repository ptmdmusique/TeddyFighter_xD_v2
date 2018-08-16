using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1_1 : LevelScript {

	public override IEnumerator LevelEvent()
	{
		switch (curCheckpoint) {
			case 0:
				if (needToUpdateCheckpoint == true) {
					//Did we move up a checkpoint but haven't updated the goal?
					enemyList[0].chance = 30;       //Asteroid
					enemyList[1].chance = 50;		//Unmanned Scout
					needToUpdateCheckpoint = false;
				} else {
					//Check for conditions to move to the next checkpoint
					if (player.dataFragment >= Random.Range(150, 175)) {
						curCheckpoint++;
						needToUpdateCheckpoint = true;
					}
				}
				break;

			case 1:
				//Collecteed 400 data fragments and has played more than 1 min
				if (player.dataFragment >= 400 && Manager.timer > 60) {
					//End game
						//Disable player control

					player.enabled = false;
					player.GetComponent<RotateToMouse>().enabled = false;

					//Play the cinematic
					endScene.Play();
					
					//Stop all spawning coroutine
					StopAllCoroutines();
				}
				break;
		}

		yield return new WaitForFixedUpdate();

		levelScript = StartCoroutine(LevelEvent());
	}
}
