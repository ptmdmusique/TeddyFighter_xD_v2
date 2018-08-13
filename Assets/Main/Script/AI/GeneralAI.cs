using System.Collections;
using UnityEngine;
using DG.Tweening;

public enum AIType
{
    Simple = 0,
    Intermediate = 1,
    Complex = 2,
}

public abstract class GeneralAI : MonoBehaviour {
    public AIType myType = AIType.Simple;   //What AI type am I?

	[Header("Reaction stuff")]
	public bool followFormation = false;
	public float moveDelay = 5;
	public float moveDistance = 7;
	public float minDistance = 4;
	public float moveTime = 1;
	//NOTE: CHECK AI SCHEME FOR MORE INFO OF THE STATES

	protected GeneralObject script; //Linking the AI to the object

	#region Action
	public IEnumerator MoveSideWay()
	{
		float multiplier = Random.Range(-1f, 1f);
		
		if (transform.position.x + moveDistance > StaticGlobal.GetCameraWidth(Camera.main) / 2) {
			//Too much to the right then move to the left
			multiplier = -1;	
		} else if (transform.position.x - moveDistance < -StaticGlobal.GetCameraWidth(Camera.main) / 2){
			multiplier = 1;
		}
		Vector2 newLocation = new Vector2(transform.position.x + multiplier * moveDistance + Mathf.Sign(multiplier) * minDistance, transform.position.y + GetComponent<GeneralObject>().GetRB().velocity.y * moveTime);

		transform.DOMove(newLocation, moveTime).SetEase(Ease.OutQuad);

		yield return new WaitForSeconds(Random.Range(moveDelay * 0.80f, moveDelay));

		StartCoroutine(MoveSideWay());
	}
	public void StartObject(Vector3 direction)
	{
		if (direction == Vector3.zero) {
			if (tag == "Ally") {
				direction = Vector3.up;
			} else {
				direction = Vector3.down;
			}
		}
		script.Launch(direction, script.mySpeed, 1);
	}
	#endregion
}
