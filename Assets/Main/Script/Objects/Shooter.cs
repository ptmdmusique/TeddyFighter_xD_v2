using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : GeneralObject {
    //List
    [System.Serializable]
    public class WeaponList
    {
        public Weapon myWeapon;
        public bool isActive;
        
        public void StopShooting(float delay)
        {
            myWeapon.StopShooting(delay);
        }
    }
    [Header("Weapon Info")]
    public List<WeaponList> weaponList;

    //Basic info
    [HideInInspector] public Transform myTarget;
    [HideInInspector] public Vector2 targetVector;

	#region Default
	protected override void Awake()
    {
        base.Awake();

        bool allInactive = true;
        foreach (WeaponList weapon in weaponList) {
            if (weapon.isActive == true) {
                allInactive = false;
                break;
            }
        }
        if (weaponList.Count > 0 && allInactive == true) {
            weaponList[0].isActive = true;
        }

        if (tag == "Enemy") {
            targetVector = Vector2.down;
        } else {
            targetVector = Vector2.up;
        }
        UpdateTarget(targetVector);
    }
	#endregion

	#region Action
	public override bool Shoot(Vector2 target, int indx = -1, int delay = 0)
    {
        if (indx != -1 && weaponList[indx].isActive == false) {
            return false;
        }

        if (indx == -1) {
            //Shoot all
            bool didShoot = false;
            foreach (WeaponList weapon in weaponList) {
                Weapon curGun = weapon.myWeapon;
                if (canAttack == true && curGun != null) {
                    bool temp = curGun.StartShooting(target, 0);
                    didShoot = temp == true ? temp : false;
                }
            }
            return didShoot;
        } else {
            //Shoot only the specified gun
            Weapon curGun = weaponList[indx].myWeapon;
            if (canAttack == true && curGun != null) {
                return curGun.StartShooting(target, 0);
            }
        }
        return false;
    }   //Fire a weapon
    public override void StopShooting(int indx = -1, int delay = 0)      //Stop shooting
    {
        if (indx > -1) {
            weaponList[indx].StopShooting(delay);
        } else {
            foreach (WeaponList weapon in weaponList) {
                weapon.StopShooting(delay);
            }
        }
    }
    public void SetActive(int indx = -1) //Set active a weapon
    {
        if(indx > -1) {
            weaponList[indx].isActive = true;
        } else {
            foreach(WeaponList weapon in weaponList) {
                weapon.isActive = true;
            }
        }
    }
    public void SetInactive(int indx = -1)      //Deactivate a weapon
    {
        if (indx > -1) {
            weaponList[indx].isActive = false;
        } else {
            foreach (WeaponList weapon in weaponList) {
                weapon.isActive = false;
            }
        }
    }
    public void UpdateTarget(Vector2 newTarget)
    {
        foreach (WeaponList weaponElement in weaponList) {
			if (weaponElement != null) { 
				weaponElement.myWeapon.UpdateTarget(newTarget);
			}
		}
    }
    public override void SetAutoShoot(bool target = true, int indx = -1)
    {
        if (indx == -1) {
            foreach (WeaponList weapon in weaponList) {
                weapon.myWeapon.autoShoot = target;
            }
        } else {
            weaponList[indx].myWeapon.autoShoot = target;
        }
    }
	#endregion
}
