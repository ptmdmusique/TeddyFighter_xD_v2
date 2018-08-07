using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldParticle : MonoBehaviour {

    public float damageReduce = 10;
    public float resetTime = 3f;
    public float myDamage = 60;
    public List<Transform> explosionList;
    private Collector vfxMaster;

	#region Default
	private void Awake()
    {
        vfxMaster = GameObject.Find("VFX_Collector").GetComponent<Collector>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GeneralObject otherScript = collision.GetComponent<GeneralObject>();
        if ((((tag == "Ally" || tag == "Player") && (collision.tag == "Enemy")) ||
            ((collision.tag == "Ally" || collision.tag == "Player") && (tag == "Enemy")))) {
            if (otherScript.isInvincible == false) {         //If the bullet is not invincible
                otherScript.ChangeHealth(-myDamage);        //Cause damage
                CreateExplosion();                          //VFX

                if (otherScript.GetComponent<Projectile>() == true && otherScript.myDamage < damageReduce) {
                    otherScript.Die();                      //Meet projectile with lower damage reduction
                } else {
					ReduceDamage(otherScript);
				}

            }
        }
    }
	private void OnCollisionEnter2D(Collision2D collision)
	{
		
		GeneralObject otherScript = collision.transform.GetComponent<GeneralObject>();
		if ((((tag == "Ally" || tag == "Player") && (collision.transform.tag == "Enemy")) ||
			((collision.transform.tag == "Ally" || collision.transform.tag == "Player") && (tag == "Enemy")))) {
			if (otherScript.isInvincible == false) {         //If the bullet is not invincible
				otherScript.ChangeHealth(-myDamage);        //Cause damage
				CreateExplosion();                          //VFX

				if (otherScript.GetComponent<Projectile>() == true && otherScript.myDamage < damageReduce) {
					otherScript.Die();                      //Meet projectile with lower damage reduction
				} else {
					ReduceDamage(otherScript);
				}

			}
		}
	}
	#endregion

	#region Action
	private void ReduceDamage(GeneralObject otherScript)
    {
        otherScript.ReduceDamageThroughTime(damageReduce, resetTime);
    }
    public virtual void CreateExplosion(int minIndx = -1, int maxIndx = -1)
    {
        if (minIndx == -1 || maxIndx == -1) {
            //Create all 
            foreach (Transform explosion in explosionList) {
                vfxMaster.AddChild(Instantiate(explosion, transform.position, Quaternion.identity));
            }
            return;
        }

        for (int indx = minIndx; indx <= maxIndx; indx++) {
            vfxMaster.AddChild(Instantiate(explosionList[indx], transform.position, Quaternion.identity));
        }
    }
	#endregion
}
