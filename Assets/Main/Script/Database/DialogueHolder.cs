using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueHolder : MonoBehaviour {

	public DialogueManager manager;
	public List<List<Dialogue>> listOfDialogueLists;           //List of dialogue lists
	public List<GameObject> eventToTrigger;
	public int curIndx = 0;

	#region Default
	private void Awake()
	{
		listOfDialogueLists = new List<List<Dialogue>>();
		//Get the dialogues from child
		foreach (Transform child in transform) {
			List<Dialogue> temp = new List<Dialogue>();
			foreach(Dialogue component in child.GetComponents<Dialogue>()) {
				temp.Add(component);
			}
			listOfDialogueLists.Add(temp);
		}
	}
	#endregion

	#region Action
	public void PlayCurrentList()
	{
		manager.dialogueList = listOfDialogueLists[curIndx];
		manager.StartDialogue();
		manager.eventToTriggerAfter = eventToTrigger[curIndx++];
	}
	#endregion
}
