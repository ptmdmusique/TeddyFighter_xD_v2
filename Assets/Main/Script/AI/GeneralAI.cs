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
	protected Vector2 initialDir = Vector2.down;

	#region Action
	public IEnumerator MoveSideWay()
	{
		float multiplier = Random.Range(-1f, 1f);
		Vector2 newLocation = new Vector2(transform.position.x + multiplier * moveDistance + Mathf.Sign(multiplier) * minDistance, transform.position.y + GetComponent<GeneralObject>().GetRB().velocity.y * moveTime);
		transform.DOMove(newLocation, moveTime).SetEase(Ease.Linear);

		yield return new WaitForSeconds(Random.Range(moveDelay * 0.80f, moveDelay));

		StartCoroutine(MoveSideWay());
	}
	public void StartObject()
	{
		script.Launch(initialDir, script.mySpeed, 1);
	}
	#endregion
}
