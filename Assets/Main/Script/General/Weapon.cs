﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    //Part of the weapon
    public Transform nozzle;        //Will be used to calculate the firing vector
    public Transform aimPoint;

    //Basic info
    public bool ableToShoot = false;
        //Bullet
    [System.Serializable]
    public class Bullet
    {
        [Header("Gun relating info")]
        public float fireRate = 5;          //Bullet per second: 1/fireRate = reloadTime
        public int bulletPerWave = 3;
        public float waitDuringWave = 3;
        public float waitAfterWave = 0.5f;
        public int numberOfWave = 1;
        public int bulletBeforeWait = -1;
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
    public List<Bullet> bulletList;
	protected Bullet curBullet;
    //protected int curIndx = 0;
        //Weapon itself
    public bool autoShoot = false;
    public float rageCost = 0;
    private Coroutine shooting;
    public Transform myTarget;
    protected Vector2 targetVector;
    protected Vector2 gunToNozzle;
    protected Vector2 curAutoShoot;

    //Collector
    private Collector bulletMaster;

	#region Default
	private void Awake()
    {
        bulletMaster = GameObject.Find("Bullet_Collector").GetComponent<Collector>();

		curBullet = bulletList[0];
        //Applying target
        gunToNozzle = nozzle.position - transform.position;
        if (myTarget == null) {
            targetVector = gunToNozzle;
            myTarget = nozzle;
        } else {
            targetVector = myTarget.position - transform.position;
        }

        //Start checking 
        StartCoroutine(UpdateTargetRoutine());
    }
	#endregion

	#region Action
	//Shooting
	public virtual void FireBullet(Vector2 target)
    {
        if (target == Vector2.zero) {
            target = gunToNozzle;
        }
        //Launch == 0: Just fire, constant velocity
        //Pulse  == 1: Push bullet out
        Transform bullet = Instantiate(curBullet.bullet, transform.position, Quaternion.identity);
        Projectile script = bullet.GetComponent<Projectile>();
        script.tag = tag;

        //Update with custom parameter
        StaticGlobal.ChangeIfNotEqual<float>(ref script.myDamage, curBullet.damage, -1);
        StaticGlobal.ChangeIfNotEqual<float>(ref script.myRageReward, curBullet.rageReward, -1);
        StaticGlobal.ChangeIfNotEqual<float>(ref script.lifeTime, curBullet.lifeTime, -1);
        StaticGlobal.ChangeIfNotEqual<float>(ref script.aoeRadius, curBullet.aoeRadius, 0);
        StaticGlobal.ChangeIfNotEqual<float>(ref script.aoePercentage, curBullet.aoePercentage, 0);
        StaticGlobal.ChangeIfNotEqual<float>(ref script.fasterDuration, curBullet.fasterDuration, -1);
        StaticGlobal.ChangeIfNotEqual<float>(ref script.fasterMax, curBullet.fasterMax, -1);
        StaticGlobal.ChangeIfNotEqual<float>(ref script.mySpeed, curBullet.bulletSpeed, -1);

        script.tag = tag;
        script.gameObject.layer = gameObject.layer;
        bulletMaster.AddChild(bullet);

        script.Launch(target, curBullet.bulletSpeed, 1);
        if (curBullet.fasterMax > -1 && curBullet.fasterDuration > -1) {
            script.FasterThroughTime();
        }
    }
    public virtual IEnumerator Shoot(Vector2 target)
    {
        for (int indx = 0; indx < curBullet.numberOfWave; indx++) {
            for (int indx2 = 0; indx2 < curBullet.bulletPerWave; indx2++) {
                FireBullet(target);
                if ((indx2 + 1) % curBullet.bulletBeforeWait == 0) {
                    yield return new WaitForSeconds(curBullet.waitDuringWave);
                }
                yield return new WaitForSeconds(curBullet.waitAfterWave);
            }
        }
        
        yield return new WaitForSeconds(1.0f / curBullet.fireRate);
        shooting = null;
    }
    public virtual IEnumerator AutoShoot(Vector2 target)
    {
        curAutoShoot = target;
        for (int indx = 0; indx < curBullet.numberOfWave; indx++) {
            for (int indx2 = 0; indx2 < curBullet.bulletPerWave; indx2++) {
                FireBullet(target);
                if ((indx2 + 1) % curBullet.bulletBeforeWait == 0) {
                    yield return new WaitForSeconds(curBullet.waitDuringWave);
                }
                yield return new WaitForSeconds(curBullet.waitAfterWave);
            }
        }

        yield return new WaitForSeconds(1.0f / curBullet.fireRate);
        //Shoot again
        shooting = StartCoroutine(AutoShoot(target));
    }   //Auto firing through time
    public virtual bool StartShooting(Vector2 target, float delay = 0)
    {
        if (shooting != null) {
            rageCost = 0;               //Return if we are already shooting. Reset the rage cost so that we don't reduce the rage 
            return false;
        }

        targetVector = target;
        rageCost = curBullet.rageCost;

        if (autoShoot == true) {
            shooting = StartCoroutine(AutoShoot(target));
        } else {
            shooting = StartCoroutine(Shoot(target));
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
        StartShooting(curAutoShoot);
    }
        //Target
    public void UpdateTarget(Transform newTarget)         //Need to work on rotation of the nozzle
    {
        if (newTarget == null) {
            myTarget = nozzle;
        } else {
            myTarget = newTarget;
        }
        targetVector = newTarget.position - transform.position;
    }
    public void UpdateTarget(Vector2 newTarget)             //Need to work on rotation of the nozzle
    {
        targetVector = newTarget;
    }
    private IEnumerator UpdateTargetRoutine()
    {
        gunToNozzle = nozzle.position - transform.position;  //Update nozzle position
        UpdateTarget(myTarget);
        yield return new WaitForSeconds(Time.deltaTime);
        StartCoroutine(UpdateTargetRoutine());
    }
        //Bullet
    public int NextBullet()
    {
		int curIndx = bulletList.IndexOf(curBullet);
		curIndx = curIndx >= bulletList.Count - 1? 0 : curIndx + 1;
		curBullet = bulletList[curIndx];
		return curIndx;
    }
    public int NextBullet(int indx)
    {
		curBullet = bulletList[Mathf.Min(indx, bulletList.Count - 1)];
        return  bulletList.IndexOf(curBullet);
    }
    public float GetCurRageCost()
    {
        return curBullet.rageCost;
    }
	#endregion
}
