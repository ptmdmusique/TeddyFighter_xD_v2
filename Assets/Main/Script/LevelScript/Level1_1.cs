using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1_1 : LevelScript {

	public override IEnumerator LevelEvent()
	{
		switch (curCheckpoint) {
			case 1:
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

			case 2:
				if (player.dataFragment >= 300) {
					//End game
				}
				break;
		}

		yield return new WaitForFixedUpdate();

		levelScript = StartCoroutine(LevelEvent());
	}
}
