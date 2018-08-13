using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GeneralObject : MonoBehaviour {

	[System.Serializable]
	public class DropCollectible
	{
		public int numberOfSpawn = 1;
		public float spawnChance = 0;
		public Transform collectible;
	}

	//Basic stats
	[Header("Basic stats")]
	public float maxHealth = 0;
	public float curHealth;
	public float mySpeed = 0;
	public bool isInvincible = false;
	public float myDamage = 0;
	public bool canAttack = false;
	public float myScore = 10;
	public bool autoMove = false;

	[Header("Speed stuff")]
	public float fasterDuration = -1;
	public float fasterMax = 0;
	private bool isFastering = false;
	private float fasteringTime = 0;

	//On destroy
	[Header("On being destroyed")]
	public List<Transform> explosionList;
	public float aoeRadius = 0;                 //Damage surrounding when die
	public float aoePercentage = 0;
	public List<DropCollectible> collectible;
	public int collectibleOption = 0;

	//Private info
	protected Rigidbody2D myRb;
	protected Vector2 initialPos;
	protected Collider2D myCollider;

	//Camera info
	protected Vector2 xScreen;
	protected Vector2 yScreen;

	//Collector 
	[HideInInspector] public Collector vfxMaster;
	[HideInInspector] public Collector bulletMaster;
	[HideInInspector] public Collector objectMaster;

	#region Default
	protected virtual void Awake()
    {
        initialPos = transform.position;
        myRb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();

        curHealth = maxHealth;

        xScreen = StaticGlobal.GetCameraXBound(Camera.main);
        yScreen = StaticGlobal.GetCameraYBound(Camera.main);

        vfxMaster = GameObject.Find("VFX_Collector").GetComponent<Collector>();
        objectMaster = GameObject.Find("Bullet_Collector").GetComponent<Collector>();

        //Clamp the aoe radius
        aoePercentage = Mathf.Clamp01(aoePercentage);

		if (autoMove == true) {
			if (tag == "Enemy") {
				Move(Vector3.down, mySpeed);
			} else if (tag == "Ally") {
				Move(Vector3.up, mySpeed);
			}
		}
    }
    protected virtual void FixedUpdate()
    {
        if (isFastering == true) {
            mySpeed = Mathf.Lerp(mySpeed, fasterMax, fasteringTime);    //Changing the magnitude
            myRb.velocity = myRb.velocity.normalized * mySpeed;         //Keeping the same direction
            if (fasteringTime < 1) {
                fasteringTime += Time.fixedDeltaTime / fasterDuration;
            } else {
                isFastering = false;
            }
        }
    }
	protected virtual void OnCollisionEnter2D(Collision2D collision)
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
	}
	#endregion

	#region Stats
	public virtual void ChangeHealth(float amount, int option = 0)
    {
        switch (option) {
            case 0:
                //Direct
                curHealth += amount;
                curHealth = Mathf.Clamp(curHealth, 0, maxHealth);
                break;
            case 1:
                //Percentage
                curHealth += maxHealth * amount / 100f;
                curHealth = Mathf.Clamp(curHealth, 0, maxHealth);
                break;
        }
		CheckHealthCondition();
    }
    public virtual void ChangeMaxHealth(float amount, int option = 0)
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
    public void CheckHealthCondition()
    {
        if (maxHealth <= -1) {
            return;
        }

        if (curHealth <= 0) { 
            Die();
        }
    }
	#endregion

	#region Action
	public virtual void Move(Vector2 dir, float speed = 0)
    {
        if (dir == Vector2.zero) {
            dir = Vector2.down;
        } else {
            dir = dir.normalized;
        }
        speed = speed == 0 ? mySpeed : speed;

        myRb.velocity = dir * speed;
    }
	public virtual void MoveToPosition(Vector3 target, float time, Ease easeType = Ease.Linear)
	{
		transform.DOMove(target, time).SetEase(easeType);
	}
	public virtual void Attack(Transform target = null) { }         
    public virtual void Attack(Vector3 target) { }                  
    public virtual void Die()
    {
        CreateExplosion();
        if (tag == "Enemy" && GetComponent<Projectile>() == null) {
            StaticGlobal.GetPlayer().GetComponent<Player>().ChangeScore(myScore);
        }

        if (aoeRadius > 0) {
            Collider2D[] withinRadius = Physics2D.OverlapCircleAll(transform.position, aoeRadius);
            foreach (Collider2D collider in withinRadius) {
                GeneralObject otherScript = collider.GetComponent<GeneralObject>();
                if (((tag == "Ally" || tag == "Player") && (collider.tag == "enemy")) &&
                    ((collider.tag == "Ally" || collider.tag == "Player") && (tag == "enemy"))) {
                    if (otherScript.isInvincible == false) {
                        otherScript.ChangeHealth(-myDamage * aoePercentage);
                    }
                }
            }
        }
		SpawnCollectible(collectibleOption);

		Destroy(gameObject);
    }
	public virtual void SpawnCollectible(int option = 0)
	{
		//Option = 0: spawn only 1 object
		//		 = 1: spawn all
		//Spawn collectible 

		bool spawned = false;
		foreach (DropCollectible child in collectible) {
			int chance = Random.Range(0, 100);
			if (chance < child.spawnChance && ((spawned == false && option == 0) || option == 1)) {
				spawned = true;
				for (int indx = 0; indx < child.numberOfSpawn; indx++) {
					objectMaster.AddChild(Instantiate(child.collectible, transform.position, Quaternion.identity));
				}
			}
		}
	}
    public virtual void Launch(Vector2 direction, float speed, int option = 0)
    {
        myRb.velocity = direction.normalized * speed;
        if (option == 1) {
            //Rotate to velocity vector
            StartCoroutine(RotateToVector(direction));
        }
    }
    public virtual IEnumerator RotateToVector(Vector2 target, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        //Rotate. I hate quaternion...
        transform.eulerAngles = new Vector3(0, 0,
                Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg - 90);
    }
    public void FasterThroughTime()
    {
        if (fasterDuration > -1) {
            isFastering = true;
        }
    }
    public IEnumerator ReduceDamageThroughTime(float reduceDamage, float resetTime = -1)
    {
        float temp = myDamage;
        myDamage = myDamage > reduceDamage ? myDamage - reduceDamage : 0;
		Debug.Log(myDamage);
        if (resetTime <= -1) {
            //Reduce forever
            yield return null;
        } else {
            //Reset after some time
            yield return new WaitForSeconds(resetTime);
            myDamage = temp;
        }
    }
    public void SetSpeed(Vector2 newSpeed)
    {
        myRb.velocity = newSpeed;
    }
	#endregion

	#region Reset
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
	#endregion

	#region Misc
	public virtual void CreateExplosion(int minIndx = -1, int maxIndx = -1)
    {
        if (minIndx == -1 || maxIndx == -1) {
            //Create all 
            foreach(Transform explosion in explosionList) {
                vfxMaster.AddChild(Instantiate(explosion, transform.position, Quaternion.identity));
            }
            return;
        }

        for (int indx = minIndx; indx <= maxIndx; indx++) {
            vfxMaster.AddChild(Instantiate(explosionList[indx], transform.position, Quaternion.identity));
        }
    }
    public void RotateToTarget(Vector2 target)
    {

    }
    public void SetTag(string newTag)   //Set the tag of its children and itself 
    {
        tag = newTag;
        foreach(Transform child in transform) {
            child.tag = newTag;
        }
    }
    //public void SetLayer(string newLayer)   //Set the layer of its children and itself 
    //{
    //    gameObject.layer = LayerMask.NameToLayer(newLayer);
    //    foreach (Transform child in transform) {
    //        child.gameObject.layer = LayerMask.NameToLayer(newLayer);
    //    }
    //}
	#endregion

	#region GetSet
	public Collider2D GetCollider() {
		return myCollider;
	}
	public Rigidbody2D GetRB()
	{
		return myRb;
	}
	public virtual void SetAutoShoot(bool v){}
	//For shooting AI
	public virtual bool Shoot(Vector2 target, int indx = -1, int delay = 0) { return false; }
	public virtual void StopShooting(int indx = -1, int delay = 0) { }
	#endregion
}
