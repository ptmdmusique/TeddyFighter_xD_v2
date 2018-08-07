using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideAI : GeneralAI {

    private Collider2D myCollider;
    public Transform target;
    [Header("Target stuff")]
    public bool useTarget = false;
    public float precision = 0.1f;                  //How precise do we check
    public float chargeWait = 0.5f;
    private bool isCharging = false;

	#region Default
	void Awake () {
        myCollider = GetComponent<Collider2D>();
        script = GetComponent<GeneralObject>();

        if (target == null) {
            if (myType == AIType.Simple) {
                if (tag == "Enemy") {
                    //Use the player as initial target
                    if (useTarget == true) { 
                        target = StaticGlobal.GetPlayer();
                    }
                }
            }
        }
    }
    private void Start()
    {
        Vector2 initialDir = Vector2.down;
        if (tag == "Ally") {
            initialDir = Vector2.up;
        } else {
			initialDir = Vector2.down;
		}

        script.Launch(initialDir, script.mySpeed, 1);
    }
    void Update () {
        if (myType == AIType.Simple) {
            if (isCharging == false && StaticGlobal.IsOutOfBound(transform) == false && TargetInFront() == true) {
                StartCoroutine(Charge());
            }
        }
    }
	#endregion

	#region Action
	public bool TargetInFront()
	{
		if (useTarget == true) {
			Vector2 leftPoint = myCollider.bounds.center;
			leftPoint.x -= myCollider.bounds.extents.x;
			Vector2 rightPoint = myCollider.bounds.center;
			rightPoint.x += myCollider.bounds.extents.x;

			float angle_1 = Vector2.Angle(leftPoint, target.position);
			float angle_2 = Vector2.Angle(rightPoint, target.position);

			//Need to generalize more
			if (angle_1 <= 90 && angle_2 >= 90) {
				return true;
			}
		} else {
			Vector2 leftPoint = myCollider.bounds.center;
			leftPoint.x -= myCollider.bounds.extents.x;
			Vector2 rightPoint = myCollider.bounds.center;
			rightPoint.x += myCollider.bounds.extents.x;

			for (; leftPoint.x <= rightPoint.x; leftPoint.x += precision) {
				RaycastHit2D[] hits = Physics2D.RaycastAll(leftPoint, transform.up);
				foreach (RaycastHit2D hit in hits) {
					if ((hit.transform.tag == "Enemy" && (tag == "Ally" || tag == "Player")) || (tag == "Enemy" && (hit.transform.tag == "Ally" || hit.transform.tag == "Player"))) {
						if (hit.transform.GetComponent<Projectile>() == false) {
							//Not a bullet
							return true;
						}
					}
				}
			}
		}

		return false;
	}
	public IEnumerator Charge()
    {
        isCharging = true;
        Vector2 direction = Vector2.up;
        StartCoroutine(script.StopMoving());

        if (tag == "Enemy") {
            direction = Vector2.down;
        }

        yield return new WaitForSeconds(chargeWait);

        script.SetSpeed(direction  * script.mySpeed);
        script.FasterThroughTime();
    }
    public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;
    }
	#endregion
}
