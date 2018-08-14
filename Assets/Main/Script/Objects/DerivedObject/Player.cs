using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : Shooter {

	//Basic info
	//Rage
	[Header("Basic stats")]
	public float maxRage = 0;
	public float curRage;
	[Header("Powerup")]
	public int curPowerUp = 0;
	public int maxPowerUp = 5;
	public float powerupTime = 3;           //In second
	public float powerupTimer = 0;
	private Coroutine powerupCoroutine;
	//Gun index
	public const int singleMin = 0;
	public const int singleMax = 2;
	public const int multiGun = 3;
	public const int ultiGun = 4;
	//Misc
	[HideInInspector] public float dataFragment = 0;
	[HideInInspector] public float score = 0 ;
    [Header("Status bars + Icons")]
    [SerializeField] private StatusIndicator myHealthSI;
    [SerializeField] private StatusIndicator myRageSI;
    [SerializeField] private StatusIndicator dataFragmentSI;
	[SerializeField] private StatusIndicator scoreSI;
	[SerializeField] private StatusIndicator powerupSI;
	[SerializeField] private TextMeshProUGUI powerupValue;
	[SerializeField] private WeaponIcon singleIcon;
    [SerializeField] private WeaponIcon multiIcon;
    [SerializeField] private WeaponIcon ultiIcon;

    [Header("Shield")]
    public Transform myShield;
    private Shield myShieldScript;
    public float shieldRageRate = 1;
    public float shieldDepleteTime = 0.5f;
    private Coroutine shieldDepleting;
	
	#region Default
	protected override void Awake()
    {
        base.Awake();
        curRage = maxRage;

        myShieldScript = myShield.GetComponent<Shield>();
    }
    private void Start()
    {
        myShieldScript.SetActive(false);
		myHealthSI.SetValue(curHealth, maxHealth);
		myRageSI.SetValue(curRage, maxRage);
		scoreSI.SetValue(score, 99999);
		dataFragmentSI.SetValue(dataFragment, 99999);
    }
    protected void Update()
    {
        InputController();

        //Setting up the status indicator
    }
    protected override void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0.0f);
        myRb.velocity = movement * mySpeed;

        myRb.position = new Vector3
        (
            Mathf.Clamp(myRb.position.x, xScreen.x, xScreen.y),
            Mathf.Clamp(myRb.position.y, yScreen.x, yScreen.y),
            0.0f
        );
    }
	protected void OnTriggerEnter2D(Collider2D collision)
	{
		Collectible script = collision.GetComponent<Collectible>();
		if (collision.tag == "Collectible") {
			if(collision.name.Contains("Data") == true) {
				ChangeDataFragment((int) Random.Range(script.minValue, script.maxValue));
			} else if (collision.name.Contains("Health") == true) {
				ChangeHealth((int) Random.Range(script.minValue, script.maxValue));
			} else if (collision.name.Contains("Powerup") == true) {
				
				ChangePowerUp((int) script.minValue);
			}
			script.Collected();
		}
	}
	private void OnDisable()
	{
		myRb.velocity = Vector3.zero;
	}
	#endregion

	#region Action
	public override void ChangeHealth(float amount, int option = 0)
    {
        base.ChangeHealth(amount, option);
        myHealthSI.SetValue(curHealth, maxHealth);
    }
    public override void ChangeMaxHealth(float amount, int option = 0)
    {
        base.ChangeMaxHealth(amount, option);
        myHealthSI.SetValue(curHealth, maxHealth);
    }
    public void ChangeRage(float amount, int option = 0)
    {
        switch (option) {
            case 0:
                //Direct
                curRage += amount;
                curRage = Mathf.Clamp(curRage, 0, maxRage);
                break;
            case 1:
                //Percentage
                curRage += maxRage * amount / 100f;
                curRage = Mathf.Clamp(curRage, 0, maxRage);
                break;
        }
        myRageSI.SetValue(curRage, maxRage);
    }
    public void ChangeScore(float amount)
    {
		score += amount;
		scoreSI.SetValue(score, 99999);
    }
	public void ChangeDataFragment(float amount)
	{
		dataFragment += amount;
		dataFragmentSI.SetValue(dataFragment, 99999);
	}
	private float GetRageCost(int indx)
	{
		return weaponList[indx].myWeapon.GetCurRageCost();
	}
	private IEnumerator UsingShield()
	{
		ChangeRage(-shieldRageRate);
		yield return new WaitForSeconds(shieldDepleteTime);
		shieldDepleting = StartCoroutine(UsingShield());
	}
	public void ChangePowerUp(int amount)
	{
		curPowerUp = Mathf.Clamp(curPowerUp + amount, 0, maxPowerUp);
		if (amount > 0) {
			if (powerupCoroutine == null) {
				powerupTimer = powerupTime;
				powerupCoroutine = StartCoroutine(PowerupCoroutine());
			}
		}	

		//TODO: Change the statusIndicator
		foreach(WeaponList weapon in weaponList) {
			weapon.myWeapon.powerupLevel += amount;
		}

		powerupValue.text = curPowerUp.ToString();
	}
	private IEnumerator PowerupCoroutine()
	{
		while (curPowerUp > 0) {
			while (powerupTimer > 0) {
				powerupTimer -= Time.deltaTime;
				powerupSI.SetValue(powerupTimer, powerupTime);
				yield return new WaitForEndOfFrame();
			}
			ChangePowerUp(-1);
			powerupTimer = powerupTime;                 //Reset the timer
		}

		powerupCoroutine = null;
	}
	#endregion

	#region Input
	void FireInput()
    {
        if (Input.GetButton("SingleShot") == true) {
            //Shoot
            float rageCost = 0;
            for(int indx = singleMin; indx <= singleMax; indx++) {
                float temp = GetRageCost(indx);
                if (temp > 0) {
                    rageCost = temp;
                }
            }

			if (curRage >= rageCost) {
                bool didShoot = false;
                for (int indx = singleMin; indx <= singleMax; indx++) {
                    bool temp = Shoot(Vector2.zero, indx);
                    didShoot = temp == true ? temp : didShoot;
                }
				if (didShoot == true) {
                    if (rageCost != 0) {
						ChangeRage(-rageCost);
					}
                }
            }
        }
        if (Input.GetButton("MultiShot") == true) {
            float rageCost = GetRageCost(multiGun);   //Default rage cost
            if (curRage >= rageCost) {
                if (Shoot(Vector2.zero, multiGun) == true && rageCost != 0) { 
                    ChangeRage(-rageCost);
                }
            }
        }
        if (Input.GetButton("Ulti") == true) {
            float rageCost = GetRageCost(ultiGun);   //Default rage cost
            if (curRage >= rageCost) {
                if (Shoot(Vector2.zero, ultiGun) == true && rageCost != 0) { 
                    ChangeRage(-rageCost);
                }
            }
        }

    }
    void ShieldInput()
    {
        if(Input.GetButton("Shield") == true && shieldDepleting == null && curRage >= shieldRageRate) {
            //Turn on the shield and start draining rage
            myShieldScript.SetActive(true);
            shieldDepleting = StartCoroutine(UsingShield());
        } else if (Input.GetButtonUp("Shield") == true || curRage < shieldRageRate) {
            myShieldScript.SetActive(false);
            if(shieldDepleting != null) {
                StopCoroutine(shieldDepleting);
            }
            shieldDepleting = null;
        }
    }
    void ChangeBulletInput()
    {
        if (Input.GetButtonDown("ChangeSingle") == true) {
            for (int indx = singleMin; indx <= singleMax; indx++) {
                singleIcon.NextImage(weaponList[indx].myWeapon.NextBullet());
            }
        }  
        if (Input.GetButtonDown("ChangeMulti") == true) {
            int spriteIndx = weaponList[multiGun].myWeapon.NextBullet();
            multiIcon.NextImage(spriteIndx);
        }
    }
    private void InputController()
    {
        ChangeBulletInput();
        FireInput();
        ShieldInput();
    }
	#endregion
}

