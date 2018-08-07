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
	[HideInInspector] public string newName;
	[TextArea] public List<string> curDialogues;
	public List<string> curName;
	public float pauseBetweenCharacter = 0.1f;
	private Coroutine showText;

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
	private void OnEnable()
	{
		//Reveal the text on enable
		canvasGroup.DOFade(1, 1);
		StartCoroutine(ShowAllText());
	}
	private void OnDisable()
	{
		info.DOKill();
		info.color = new Color(info.color.r, info.color.g, info.color.b, 1);    //Make sure the text is visible at first
		StopAllCoroutines();
	}
	#endregion

	#region Action
	public IEnumerator ShowAllText()
	{
		//Enable the info
		info.enabled = true;
		info.DOFade(0.4f, 0.5f).SetLoops(-1, LoopType.Yoyo);

		for (int indx = 0; indx < curDialogues.Count; indx++) {
			chatterName.text = "Name: " + curName[indx];
			scrollBar.value = 1;
			ShowText(curDialogues[indx]); //Revealing the current text
			while (showText != null) {
				//While revealing, wait
				yield return new WaitForEndOfFrame();
			}

			//When finish revealing, wait until the play press enter or something
			yield return new WaitUntil(() => Input.GetButtonDown("Submit"));
		}

		//Fading out
		canvasGroup.DOFade(0, 1).OnComplete(() => transform.gameObject.SetActive(false));
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
