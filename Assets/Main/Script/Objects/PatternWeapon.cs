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

	public virtual void FireBullet(Vector2 target)
	{
		if (curBullet.canBeUsed == false) {
			return;
		}

		target = target == Vector2.zero ? gunToNozzle : target;

		//Launch == 0: Just fire, constant velocity
		//Pulse  == 1: Push bullet out

		Transform bullet = Instantiate(curBullet.bullet, transform.position, Quaternion.identity);
		Projectile script = bullet.GetComponent<Projectile>();
		script.tag = tag;

		//Update with custom parameter
		StaticGlobal.ChangeIfNotEqual<float>(ref script.myDamage, curBullet.damage * (powerupLevel * 0.2f + 1), -1);
		StaticGlobal.ChangeIfNotEqual<float>(ref script.myRageReward, curBullet.rageReward, -1);
		StaticGlobal.ChangeIfNotEqual<float>(ref script.lifeTime, curBullet.lifeTime, -1);
		StaticGlobal.ChangeIfNotEqual<float>(ref script.aoeRadius, curBullet.aoeRadius, 0);
		StaticGlobal.ChangeIfNotEqual<float>(ref script.aoePercentage, curBullet.aoePercentage, 0);
		StaticGlobal.ChangeIfNotEqual<float>(ref script.fasterDuration, curBullet.fasterDuration, -1);
		StaticGlobal.ChangeIfNotEqual<float>(ref script.fasterMax, curBullet.fasterMax, -1);
		StaticGlobal.ChangeIfNotEqual<float>(ref script.mySpeed, curBullet.bulletSpeed, -1);

		if (powerupLevel > 0) {
			//Deploy the effect
			Transform newPS = Instantiate(powerupPS, bullet.position, Quaternion.identity);
			vfxMaster.AddChild(newPS);

			newPS.GetComponent<FollowTarget>().target = bullet;
		}

		script.tag = tag;
		script.gameObject.layer = gameObject.layer;
		bulletMaster.AddChild(bullet);

		script.Launch(target, curBullet.bulletSpeed, 1);
		if (curBullet.fasterMax > -1 && curBullet.fasterDuration > -1) {
			script.FasterThroughTime();
		}

		//Play the audio
		if (mAudioSource != null) {
			mAudioSource.Play();
		}
	}
	public override IEnumerator Shoot()
	{

		for (int curWave = 0; curWave < curBullet.numberOfWave; curWave++) {
			gunToNozzle = nozzle.position - transform.position;
			Vector2 target = myTarget == null ? gunToNozzle : (Vector2)(myTarget.position - transform.position).normalized;

			if (curBullet.bulletPerWave > 1) {
				//Evenly distribute the bullet
				float step = (curPattern.endAngle - curPattern.startAngle) / (curBullet.bulletPerWave - 1) * (curPattern.rightToLeft == false ? -1 : 1);
				//Get to the initial position
				//Evenly distribute the bullet
				Vector2 curTarget = StaticGlobal.RotateVectorByAmount_2D
					(curPattern.angleOffset + (curPattern.rightToLeft == false ? 1 : -1) * (curPattern.endAngle - curPattern.startAngle) / 2.0f, target);
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
