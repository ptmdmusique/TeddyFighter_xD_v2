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
    public List<WeaponList> weaponList;

    //Basic info
    public Transform myTarget;
    public Vector2 targetVector;

    protected override void Awake()
    {
        if (weaponList.Count > 0) {
            weaponList[0].isActive = true;
        }

        if (tag == "Enemy") {
            targetVector = Vector2.down;
        } else {
            targetVector = Vector2.up;
        }
        UpdateTarget(targetVector);
    }

    //Actions
    public void Shoot(int indx = -1, int delay = 0)
    {
        if (indx == -1) {
            foreach(WeaponList weaponElement in weaponList) {
                if (weaponElement.isActive == true) {

                    weaponElement.myWeapon.StartShooting(delay);
                }
            }
        }
    }
    public void UpdateTarget(Vector2 newTarget)
    {
        foreach (WeaponList weaponElement in weaponList) {
            weaponElement.myWeapon.UpdateTarget(newTarget);
        }
    }

}
