using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralObject : MonoBehaviour {

    //Basic stats
    public float maxHealth = 0;
    private float curHealth;
    public float mySpeed = 0;
    public bool isInvicible = false;
    public float myDamage;
    public bool canAttack = false;

    //List
    public List<Transform> explosionList;
    
    //Private info
    private Rigidbody2D myRb;
    private Vector2 initialPos;
    private Collider2D myCollider;

    protected virtual void Awake()
    {
        initialPos = transform.position;
        myRb = GetComponent<Rigidbody2D>();

        curHealth = maxHealth;
    }

    //Stat
    public void ChangeHealth(float amount, int option = 0)
    {
        switch (option) {
            case 0:
                //Direct
                curHealth += amount;
                break;
            case 1:
                //Percentage
                curHealth += maxHealth * amount;
                break;
        }
    }
    public void ChangeMaxHealth(float amount, int option = 0)
    {
        switch (option) {
            case 0:
                //Direct
                maxHealth += amount;
                break;
            case 1:
                //Percentage
                maxHealth += maxHealth * amount;
                break;
        }
    }

    //Collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((tag == "Ally" || tag == "Player") && (collision.tag == "enemy")) &&
            ((collision.tag == "Ally" || collision.tag == "Player") && (tag == "enemy"))){
            collision.GetComponent<GeneralObject>().ChangeHealth(-myDamage);
        }
    }

    //Actions
    public virtual void Move(Vector2 dir, float speed = 0)
    {
        dir = dir == null ? Vector2.down : dir.normalized;
        speed = speed == 0 ? mySpeed : speed;

        myRb.velocity = dir * speed;
    }
    public virtual void Attack(Transform target = null) { }         //Need to work on
    public virtual void Attack(Vector3 target) { }                  //Need to work on
    public virtual void Die()
    {
        CreateExplosion();
        if (tag == "Enemy") {

        }
    }

    //Reseting
    public virtual IEnumerator StopMoving(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        myRb.velocity = Vector2.zero;
    }
    public virtual IEnumerator StopAttacking(float delay = 0) {
        yield return new WaitForSeconds(delay);

    }           //Need to work on
    public virtual IEnumerator ResetObject(float delay = 0)
    {
        StopAll(delay);
        yield return new WaitForSeconds(delay);
        transform.position = initialPos;
    }
    public virtual void StopAll(float delay = 0)
    {
        StartCoroutine(StopMoving(delay));
        StartCoroutine(StopAttacking(delay));
    }

    //Misc
    public virtual void CreateExplosion(int minIndx = -1, int maxIndx = -1)
    {
        if (minIndx == -1 || maxIndx == -1) {
            //Create all 
            foreach(Transform explosion in explosionList) {
                Transform myExplosion = Instantiate(explosion, transform.position, Quaternion.identity);
            }
            return;
        }

        for (int indx = minIndx; indx <= maxIndx; indx++) {
            Transform myExplosion = Instantiate(explosionList[indx], transform.position, Quaternion.identity);
        }
    }
}
