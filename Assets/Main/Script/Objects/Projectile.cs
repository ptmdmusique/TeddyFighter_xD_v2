using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : GeneralObject {

    [Header("Projectile relating info")]
    public float myRageReward;
    public float lifeTime;
    public bool canDestroyProjectile = false;

    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        if (lifeTime > 0) {
            Invoke("Die", lifeTime);
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (tag == "Player" && collision.transform.tag == "Enemy") {
            StaticGlobal.GetPlayer().GetComponent<Player>().ChangeRage(myRageReward);
        }

        if (maxHealth <= -1 && ((tag == "Ally" || tag == "Player") && (collision.transform.tag == "Enemy")) ||
                ((collision.transform.tag == "Ally" || collision.transform.tag == "Player") && (tag == "Enemy"))) {
            Die();
        }
    }
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        GeneralObject otherScript = collision.transform.GetComponent<GeneralObject>();

        if (otherScript == null || otherScript.maxHealth <= -1) {
            return;
        }

        if (((tag == "Ally" || tag == "Player") && (collision.transform.tag == "Enemy")) ||
            ((collision.transform.tag == "Ally" || collision.transform.tag == "Player") && (tag == "Enemy"))) {
            if (otherScript.isInvincible == false) {
                otherScript.ChangeHealth(-myDamage);
            }
        }

        if (tag == "Player" && collision.transform.tag == "Enemy") {
            StaticGlobal.GetPlayer().GetComponent<Player>().ChangeRage(myRageReward);
        }

        if (maxHealth <= -1 && ((tag == "Ally" || tag == "Player") && (collision.transform.tag == "Enemy")) ||
                ((collision.transform.tag == "Ally" || collision.transform.tag == "Player") && (tag == "Enemy"))) {
            Die();
        }
    }
}
