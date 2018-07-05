using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterAI : GeneralAI {

    private bool isShooting = false;
    private Shooter script;                 //Linking the AI to the object

	#region Default
	private void Awake()
    {
        script = GetComponent<Shooter>();
    }
    private void Start()
    {
        if (myType == AIType.Simple) {

            Vector2 initialDir = Vector2.down;
            if (tag == "Ally") {
                initialDir = Vector2.up;
            }

            script.Launch(initialDir, script.mySpeed, 1);
            script.SetAutoShoot(true);
        }
    }
    private void Update()
    {
        if (myType == AIType.Simple) {
            if (isShooting == false) {
                if (StaticGlobal.IsOutOfBound(transform) == false) {
                    Vector2 target = Vector2.up;
                    if (tag == "Enemy") {
                        target = Vector2.down;
                    }
                    script.canAttack = true;
                    script.Shoot(target);
                    isShooting = true;
                }
            } else {
                if (StaticGlobal.IsOutOfBound(transform) == true) {
                    script.canAttack = false;
                    script.StopShooting();
                    isShooting = false;
                }
            }
        }
    }
	#endregion
}
