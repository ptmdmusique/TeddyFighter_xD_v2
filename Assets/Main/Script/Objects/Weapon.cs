using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    //Part of the weapon
    public Transform nozzle;        //Will be used to calculate the firing vector

	//Audio
	protected AudioSource mAudioSource;

    //Basic info
    public bool ableToShoot = false;
		//Powerup Info
	[Header("Power-up")]
	public int powerupLevel = 0;
	public Transform powerupPS;
        //Bullet
    [System.Serializable]
    public class BulletInfo
    {
        [Header("Gun relating info")]
        public float fireRate = 5;          //Bullet per second: 1/fireRate = reloadTime
        public int bulletPerWave = 3;
        public float waitDuringWave = 3;
        public float waitAfterWave = 0.5f;
        public int numberOfWave = 1;
        public int bulletBeforeWait = -1;
		public bool canBeUsed = true;
        [Header("Bullet relating info")]
        public float bulletSpeed = -1;
        public float damage = 10;
        public float rageCost = 10;
        public float rageReward = -1;
        public float lifeTime = -1;
        [Header("Additional damage")]
        public float aoeRadius = 0;
        public float aoePercentage = 0;
        [Header("Speed stuff")]
        public float fasterMax = -1;
        public float fasterDuration = -1;

        public Transform bullet;
    }
    [Header("Bullet List")]
    public List<BulletInfo> bulletList;
	protected BulletInfo curBullet;
        //Weapon itself
    public bool autoShoot = false;
    protected float rageCost = 0;
    protected Coroutine shooting;
    public Transform myTarget;
    protected Vector2 gunToNozzle;
    protected Vector2 curAutoShoot;

    //Collector
    protected Collector bulletMaster;
	protected Collector vfxMaster;

	#region Default
	public virtual void Awake()
    {
        bulletMaster = GameObject.Find("Bullet_Collector").GetComponent<Collector>();
		vfxMaster = GameObject.Find("VFX_Collector").GetComponent<Collector>();

		curBullet = bulletList[0];
        //Applying target
        gunToNozzle = nozzle.position - transform.position;

		mAudioSource = GetComponent<AudioSource>();
    }
	#endregion

	#region Action
	//Shooting
	public virtual void FireBullet()
    {
		if (curBullet.canBeUsed == false) {
			return;
		}

		gunToNozzle = nozzle.position - transform.position;
		Vector2 target = myTarget == null ? gunToNozzle : (Vector2) (myTarget.position - transform.position).normalized;

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
    public virtual IEnumerator Shoot()
    {
        for (int indx = 0; indx < curBullet.numberOfWave; indx++) {
            for (int indx2 = 0; indx2 < curBullet.bulletPerWave; indx2++) {
                FireBullet();
                if ((indx2 + 1) % curBullet.bulletBeforeWait == 0) {
                    yield return new WaitForSeconds(curBullet.waitDuringWave);
                }
                yield return new WaitForSeconds(curBullet.waitAfterWave);
            }
        }
        
        yield return new WaitForSeconds(1.0f / curBullet.fireRate);
        shooting = null;
    }
    public virtual IEnumerator AutoShoot()
    {
        for (int indx = 0; indx < curBullet.numberOfWave; indx++) {
            for (int indx2 = 0; indx2 < curBullet.bulletPerWave; indx2++) {
                FireBullet();
                if ((indx2 + 1) % curBullet.bulletBeforeWait == 0) {
                    yield return new WaitForSeconds(curBullet.waitDuringWave);
                }
                yield return new WaitForSeconds(curBullet.waitAfterWave);
            }
        }

        yield return new WaitForSeconds(1.0f / curBullet.fireRate);
        //Shoot again
        shooting = StartCoroutine(AutoShoot());
    }   //Auto firing through time
    public virtual bool StartShooting(float delay = 0)
    {
        if (shooting != null) {
            rageCost = 0;               //Return if we are already shooting. Reset the rage cost so that we don't reduce the rage 
            return false;
        }

        rageCost = curBullet.rageCost;

        if (autoShoot == true) {
            shooting = StartCoroutine(AutoShoot());
        } else {
            shooting = StartCoroutine(Shoot());
        }

        return true;
    }             //Start shooting
    public IEnumerator StopShooting(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        StopCoroutine(shooting);
        shooting = null;
    }   //Stop the shooting
    public IEnumerator ResetShooting(float delay = 0)     //Reset shooting after an amount of time
    {
        StopShooting();
        yield return new WaitForSeconds(delay);
        StartShooting();
    }
	public void UpdateTarget(Transform newTarget)
	{
		myTarget = newTarget;
	}
        //Bullet
    public virtual int NextBullet()
    {
		int curIndx = bulletList.IndexOf(curBullet);
		curIndx = curIndx >= bulletList.Count - 1? 0 : curIndx + 1;
		curBullet = bulletList[curIndx];

		if (curBullet.canBeUsed == false) {
			return NextBullet();
		}
		return curIndx;
    }
    public virtual int NextBullet(int indx)
    {
		curBullet = bulletList[Mathf.Min(indx, bulletList.Count - 1)];

		if (curBullet.canBeUsed == false) {
			return NextBullet(indx + 1);
		}
        return  bulletList.IndexOf(curBullet);
    }
    public float GetCurRageCost()
    {
        return curBullet.rageCost;
    }
	#endregion
}
