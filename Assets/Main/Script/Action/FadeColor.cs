using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class FadeColor : MonoBehaviour {
	[Header("Fading info")]
	public Ease ease = Ease.Linear;
	public float duration;
	public float from = -1;
	public float to = 1;
	public float delay = 0;
	public bool fadeOnAwake = true;
	public GameObject onCompleteEvent;

	private Graphic[] graphicList;
	private SpriteRenderer spriteRenderer;
	private CanvasGroup canvasGroup;
	private TextMeshProUGUI[] textGroup;

	private void Awake()
	{
		//Get the graphic list
		graphicList = GetComponents<Graphic>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		canvasGroup = GetComponent<CanvasGroup>();
		textGroup = GetComponents<TextMeshProUGUI>();

		if (fadeOnAwake == true) {
			StartFading();
		}
	}

	public void StartFading()
	{
		StartCoroutine(Fade());
	}
	private IEnumerator Fade()
	{
		yield return new WaitForSeconds(delay);

		if (from == -1) {
			//Fade from current alpha value
			foreach (Graphic graphic in graphicList) {
				graphic.DOFade(to, duration).SetEase(ease);
			}

			if (spriteRenderer != null) {
				spriteRenderer.DOFade(to, duration).SetEase(ease);
			}

			if (canvasGroup != null) {
				canvasGroup.DOFade(to, duration).SetEase(ease);
			}

			foreach(TextMeshProUGUI text in textGroup) {
				text.DOFade(to, duration).SetEase(ease);
			}
		} else {
			//First set the alpha value to the desired value
			foreach(Graphic graphic in graphicList) {
				graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, from);
				graphic.DOFade(to, duration).SetEase(ease);
			}

			if (spriteRenderer != null) {
				spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, from);
				spriteRenderer.DOFade(to, duration).SetEase(ease);
			}

			if (canvasGroup != null) {
				canvasGroup.alpha = from;
				canvasGroup.DOFade(to, duration).SetEase(ease);
			}

			foreach (TextMeshProUGUI text in textGroup) {
				text.color = new Color(text.color.r, text.color.g, text.color.b, from);
				text.DOFade(to, duration).SetEase(ease);
			}
		}

		yield return new WaitForSeconds(duration);
		if (onCompleteEvent != null) {
			onCompleteEvent.SetActive(true);
		}
	}
}
