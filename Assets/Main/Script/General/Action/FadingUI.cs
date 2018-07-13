using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadingUI : MonoBehaviour {

	public bool fadeOutChild = true;
	public float fadingTime = 1.0f;	//Time to complete the fading process
	public float outTarget = 0f;	//Fade out to
	public float inTarget = 1f;		//Fade in to
	public List<Graphic> graphicList;

	[Header("Event keyframe")]
	public bool startFadeOut = false;
	public bool startFadeIn = false;

	#region Default
	void Awake () {
		FillGraphicList();
	}
	// Update is called once per frame
	void Update () {
		if (startFadeIn == true) {
			FadeIn();
			startFadeIn = false;
		}

		if (startFadeOut == true) {
			FadeOut();
			startFadeOut = false;
		}
	}
	#endregion

	#region Action
	public void FadeIn()
	{
		foreach (Graphic graphic in graphicList) {
			if (graphic.isActiveAndEnabled == true) {
				graphic.DOFade(inTarget, fadingTime);
			}
		}
	}
	public void FadeOut()
	{
		foreach (Graphic graphic in graphicList) {
			if (graphic.isActiveAndEnabled == true) {
				graphic.DOFade(outTarget, fadingTime);
			}
		}
	}
	public void FillGraphicList()
	{
		//Clear up the list first
		graphicList.Clear();

		//Re-fill the list
		Image[] images;
		Text[] texts;		

		if (fadeOutChild == true) {
			images = GetComponentsInChildren<Image>();
			if (images.Length > 0) {
				graphicList.AddRange(images);
			}
			texts = GetComponentsInChildren<Text>();
			if (texts.Length > 0) {
				graphicList.AddRange(texts);
			}
		} else {
			images = GetComponents<Image>();
			if (images.Length > 0) {
				graphicList.AddRange(images);
			}
			texts = GetComponents<Text>();
			if (texts.Length > 0) {
				graphicList.AddRange(texts);
			}
		}
	}
	#endregion
}
