using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    //Part of the weapon
    public Transform nozzle;        //Will be used to calculate the firing vector

    //Basic info
    public bool ableToShoot = false;
    public float fireRate = 1;             //Bullet/s
    public Transform bullet;
    //private Bullet bulletScript;
    private Rigidbody2D bulletRB;
    public bool autoShoot = false;
    private Coroutine shooting;
    public Transform myTarget;
    private Vector2 targetVector;

    private void Awake()
    {
        //bulletScript = bullet.GetComponent<Bullet>();
        bulletRB = bullet.GetComponent<Rigidbody2D>();

        if (myTarget == null) {
            targetVector = nozzle.position - transform.position;
        } else {
            targetVector = myTarget.position - transform.position;
        }
    }

    //Action
    public virtual void Shoot(Vector2 target)    //Need to work on
    {

    }
    public virtual void FireBullet(Vector2 target, int option = 0)
    {
        //Option == 0: Just fire, constant velocity
        //Option == 1: Push bullet out
        
    }
    public virtual IEnumerator AutoShoot(Vector2 target)
    {

        yield return new WaitForSeconds(1.0f / fireRate);
    }   //Auto firing through time
    public void StartShooting(float delay = 0)
    {
        if (autoShoot == true) {
            shooting = StartCoroutine(AutoShoot(targetVector));
        } else {
            Shoot(targetVector);
        }
    }             //Start shooting
    public IEnumerator StopShooting(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        StopCoroutine(shooting);
    }   //Stop the shooting
    public IEnumerator ResetShooting(float delay = 0)     //Reset shooting after an amount of time
    {
        StopShooting();
        yield return new WaitForSeconds(delay);
        StartShooting();
    }
    public void UpdateTarget(Transform newTarget)         //Need to work on rotation of the nozzle
    {
        myTarget = newTarget;
        targetVector = newTarget.position - transform.position;
    }
    public void UpdateTarget(Vector2 newTarget)             //Need to work on rotation of the nozzle
    {
        targetVector = newTarget;
    }
}
