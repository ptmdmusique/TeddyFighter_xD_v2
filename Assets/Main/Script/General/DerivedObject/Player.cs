using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : GeneralObject {

    //Basic info
    public float maxRage = 0;               
    private float curRage;

    protected override void Awake()
    {
        base.Awake();
        curRage = maxRage;
    }

    //Stat
    public void ChangeRage(float amount, int option = 0)
    {
        switch (option) {
            case 0:
                //Direct
                curRage += amount;
                break;
            case 1:
                //Percentage
                curRage += maxRage * amount;
                break;
        }
    }
}
