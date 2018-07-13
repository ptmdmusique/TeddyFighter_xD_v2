using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponPattern{
	FullArc,
	Random,
	Even,
	Odd
}

public class PatternWeapon : Weapon {

	[System.Serializable]
	public class PatternInfo
	{
		[Header("Pattern Bullet Info")]
		public WeaponPattern myType = WeaponPattern.FullArc;
		public float startAngle = 0;
		public float endAngle = 360;
		public bool rightToLeft = false;        //Shoot from the left to right or right to left
		public float randomChance = 50;
		public float angleOffset = 0;

	}

	public List<PatternInfo> patternInfo;
	private PatternInfo curPattern;

	public override void Awake()
	{
		base.Awake();
		curPattern = patternInfo[0];
	}

	public override IEnumerator Shoot(Vector2 target)
	{
		for(int curWave = 0; curWave < curBullet.numberOfWave; curWave++) {
			if (curBullet.bulletPerWave > 1) {
				//Evenly distribute the bullet
				float step = (curPattern.endAngle - curPattern.startAngle) / (curBullet.bulletPerWave - 1) * (curPattern.rightToLeft == false ? -1 : 1);
				//Get to the initial position
				//Evenly distribute the bullet
				Vector2 curTarget = StaticGlobal.RotateVectorByAmount_2D
					(curPattern.angleOffset + (curPattern.rightToLeft == false ? 1 : -1) * (curPattern.endAngle - curPattern.startAngle) / 2.0f,
					target == Vector2.zero ? gunToNozzle : target);
				for (int shotBullet = 0; shotBullet < curBullet.bulletPerWave; shotBullet++) {
					if (curPattern.myType == WeaponPattern.FullArc ||
						(curPattern.myType == WeaponPattern.Even && shotBullet % 2 == 0 || curBullet.bulletPerWave == 1) ||
						(curPattern.myType == WeaponPattern.Odd && shotBullet % 2 != 0) ||
						(curPattern.myType == WeaponPattern.Random && Random.Range(0, 100) < curPattern.randomChance)) {
						//Fire bullet
						FireBullet(curTarget);
					}

					//Rotate
					curTarget = StaticGlobal.RotateVectorByAmount_2D(step, curTarget);

					//Should we wait?
					if ((shotBullet + 1) % curBullet.bulletBeforeWait == 0) {
						yield return new WaitForSeconds(curBullet.waitDuringWave);
					}
				}
			} else {
				//There is only 1 bullet
				Vector2 curTarget = StaticGlobal.RotateVectorByAmount_2D(curPattern.angleOffset,
					target == Vector2.zero ? gunToNozzle : target);
				FireBullet(curTarget);
			}

			//Wait after each wave
			yield return new WaitForSeconds(curBullet.waitAfterWave);
		}

		yield return new WaitForSeconds(1.0f / curBullet.fireRate);
		shooting = null;
	}
	public override int NextBullet()
	{
		int curIndx = bulletList.IndexOf(curBullet);
		curIndx = curIndx >= bulletList.Count - 1 ? 0 : curIndx + 1;
		curBullet = bulletList[curIndx];
		curPattern = patternInfo[curIndx];

		if (curBullet.canBeUsed == false) {
			return NextBullet();
		}
		return curIndx;
	}

	public override int NextBullet(int indx)
	{
		curBullet = bulletList[Mathf.Min(indx, bulletList.Count - 1)];
		curPattern = patternInfo[Mathf.Min(indx, bulletList.Count - 1)];

		if (curBullet.canBeUsed == false) {
			return NextBullet(indx + 1);
		}
		return bulletList.IndexOf(curBullet);
	}
}
