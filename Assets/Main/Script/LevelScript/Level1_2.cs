using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1_2 : LevelScript
{

	public override IEnumerator LevelEvent()
	{
		switch (curCheckpoint)
		{
			case 0:
				if (needToUpdateCheckpoint == true)
				{
					//Did we move up a checkpoint but haven't updated the goal?
					enemyList[0].chance = 40;       //Asteroid
					enemyList[1].chance = 20;       //Unmanned Scout
					needToUpdateCheckpoint = false;
				}
				else
				{
					//Check for conditions to move to the next checkpoint
					if (player.dataFragment >= Random.Range(150, 175) || Manager.enemyKilled >= 50)
					{
						curCheckpoint++;
						needToUpdateCheckpoint = true;
					}
				}
				break;
			case 1:
				if (needToUpdateCheckpoint == true)
				{
					//Did we move up a checkpoint but haven't updated the goal?
					enemyList[0].chance = 40;       //Asteroid
					enemyList[1].chance = 30;       //Unmanned Scout
					needToUpdateCheckpoint = false;
				}
				else
				{
					//Check for conditions to move to the next checkpoint
					if (player.dataFragment >= Random.Range(300, 350))
					{
						curCheckpoint++;
						needToUpdateCheckpoint = true;
					}
				}
				break;
			case 2:
				if (needToUpdateCheckpoint == true)
				{
					//Did we move up a checkpoint but haven't updated the goal?
					enemyList[0].chance = 30;       //Asteroid
					enemyList[1].chance = 20;       //Unmanned Scout
					formationList[0].chance = 17;   //Zig-zag formation
					needToUpdateCheckpoint = false;
				}
				else
				{
					//Check for conditions to move to the next checkpoint
					if (player.dataFragment >= Random.Range(500, 550))
					{
						curCheckpoint++;
						needToUpdateCheckpoint = true;
					}
				}
				break;
			case 3:
				if (needToUpdateCheckpoint == true)
				{
					//Did we move up a checkpoint but haven't updated the goal?
					formationList[1].chance = 12;   //Straight-V formation
					needToUpdateCheckpoint = false;
				}
				else
				{
					//Check for conditions to move to the next checkpoint
					if (player.dataFragment >= Random.Range(700, 750))
					{
						curCheckpoint++;
						needToUpdateCheckpoint = true;
					}
				}
				break;
			case 4:
				//Collecteed 400 data fragments and has played more than 1 min
				if (player.dataFragment >= 800 && Manager.timer > 60 && player.isActiveAndEnabled == true)
				{
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
