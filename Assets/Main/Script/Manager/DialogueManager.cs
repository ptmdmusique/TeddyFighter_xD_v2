using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class DialogueManager : MonoBehaviour {

	[Header("Components")]
	public Text chatterName;
	public Text info;
	public TextMeshProUGUI tmpDialogue;
	private CanvasGroup canvasGroup;
	public Scrollbar scrollBar;

	[Header("Dialogue info")]
	public List<Dialogue> dialogueList;
	public float pauseBetweenCharacter = 0.1f;
	private Coroutine showText;
	public bool useEnterToContinue = false;
	public GameObject eventToTriggerAfter;
	public float newTimeScale = -1;
	private float oldTimeScale = 0;


	#region Default
	// Update is called once per frame
	private void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();
		scrollBar.value = 1;
	}

	void Update () {
		if(showText != null && Input.GetButtonDown("Submit") == true) {
			//While showing the text, if the player press enter or sth then skip and show all
			SkipShowing();
		}
	}
	#endregion

	#region Action
	public void StartDialogue()
	{
		//Reveal the text on enable
		canvasGroup.DOFade(1, 1);
		StartCoroutine(ShowAllText());
	}
	public void StopDialogue()
	{
		info.DOKill();
		info.color = new Color(info.color.r, info.color.g, info.color.b, 1);    //Make sure the text is visible at first
		StopAllCoroutines();
	}
	public IEnumerator ShowAllText()
	{
		//Enable the info
		info.enabled = true;
		info.DOFade(0.4f, 0.5f).SetLoops(-1, LoopType.Yoyo);
		//Timescale stuff
		oldTimeScale = Time.timeScale;
		if (newTimeScale > -1) {
			Time.timeScale = newTimeScale;
		}

		for (int indx = 0; indx < dialogueList.Count; indx++) {
			chatterName.text = "Name:" + dialogueList[indx].chatterName;
			scrollBar.value = 1;
			ShowText(dialogueList[indx].dialogue); //Revealing the current text
			while (showText != null) {
				//While revealing, wait
				yield return new WaitForEndOfFrame();
			}

			if (useEnterToContinue == true) {
				//When finish revealing, wait until the play press enter or something
				yield return new WaitUntil(() => Input.GetButtonDown("Submit")); 
			} else {
				yield return new WaitForSeconds(1);
			}
		}

		//Fading out
		canvasGroup.DOFade(0, 1).OnComplete(() => transform.gameObject.SetActive(false));

		//Reset the timescale
		Time.timeScale = oldTimeScale;

		//Clear the list
		dialogueList.Clear();

		//Trigger the event
		if (eventToTriggerAfter != null) {
			eventToTriggerAfter.SetActive(true);
			eventToTriggerAfter.SetActive(false);
		}
	}
	public void ShowText(string newDialogue)
	{
		tmpDialogue.text = newDialogue;
		showText = StartCoroutine(ShowText());
	}
	//Show text one by one
	private IEnumerator ShowText()
	{
		tmpDialogue.maxVisibleCharacters = 0;
		int count = 0;

		while (count < tmpDialogue.text.Length) {
			count++;
			tmpDialogue.maxVisibleCharacters = count;

			yield return new WaitForSeconds(pauseBetweenCharacter);
		}

		showText = null;
	}
	private void SkipShowing()
	{
		StopCoroutine(showText);
		tmpDialogue.maxVisibleCharacters = tmpDialogue.text.Length;
		showText = null;
	}
	#endregion
}
