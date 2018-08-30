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
					enemyList[0].chance = 40;       //Asteroid
					enemyList[1].chance = 20;		//Unmanned Scout
					needToUpdateCheckpoint = false;
				} else {
					//Check for conditions to move to the next checkpoint
					if (player.dataFragment >= Random.Range(150, 175) || Manager.enemyKilled >= 50) {
						curCheckpoint++;
						needToUpdateCheckpoint = true;
					}
				}
				break;
			case 1:
				if (needToUpdateCheckpoint == true) {
					//Did we move up a checkpoint but haven't updated the goal?
					enemyList[0].chance = 35;       //Asteroid
					enemyList[1].chance = 25;       //Unmanned Scout
					needToUpdateCheckpoint = false;
				} else {
					//Check for conditions to move to the next checkpoint
					if (player.dataFragment >= Random.Range(300, 350)) {
						curCheckpoint++;
						needToUpdateCheckpoint = true;
					}
				}
				break;
			case 2:
				if (needToUpdateCheckpoint == true) {
					//Did we move up a checkpoint but haven't updated the goal?
					enemyList[0].chance = 30;       //Asteroid
					enemyList[1].chance = 20;       //Unmanned Scout
					formationList[0].chance = 10;
					needToUpdateCheckpoint = false;
				} else {
					//Check for conditions to move to the next checkpoint
					if (player.dataFragment >= Random.Range(450, 475)) {
						curCheckpoint++;
						needToUpdateCheckpoint = true;
					}
				}
				break;
			case 3:
				//Collecteed 400 data fragments and has played more than 1 min
				if (player.dataFragment >= 800 && Manager.timer > 60 && player.isActiveAndEnabled == true) {
					//End game

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
