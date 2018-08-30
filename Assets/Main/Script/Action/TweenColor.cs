using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class TweenColor : MonoBehaviour {

	[Header("Color info")]
	public Color targetColor = Color.white;
	public bool randomColor = false;
	[Header("Tween info")]
	public float duration = 2;
	public float delay = 0;
	public Ease ease = Ease.Linear;
	public int loop = -1;
	public LoopType loopType = LoopType.Incremental;
	public bool tweenOnAwake = true;
	public bool ignoreTimescale = true;
	public GameObject onCompleteEvent;

	private Graphic[] graphicList;
	private SpriteRenderer spriteRenderer;
	private TextMeshProUGUI[] textGroup;

	#region Default
	private void Awake()
	{
		//Get the graphic list
		graphicList = GetComponents<Graphic>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		textGroup = GetComponents<TextMeshProUGUI>();

		if (tweenOnAwake == true) {
			StartTweening();
		}	
	}
	#endregion

	#region Actions
	public void StartTweening()
	{
		StartCoroutine(Tweening());
	}
	private IEnumerator Tweening()
	{
		yield return new WaitForSeconds(delay);
		int numberOfFlash = 16;

		if (randomColor == false) {
			if (graphicList != null) {
				//Start tweening to a certain color
				foreach (Graphic graphic in graphicList)
				{
					graphic.DOColor(targetColor, duration).SetEase(ease, numberOfFlash, 1).SetLoops(loop, loopType).SetUpdate(ignoreTimescale);
				}
			}

			if (spriteRenderer != null) {
				spriteRenderer.DOColor(targetColor, duration).SetEase(ease, numberOfFlash, 1).SetLoops(loop, loopType).SetUpdate(ignoreTimescale);
			}

			if (textGroup != null) {
				foreach (TextMeshProUGUI text in textGroup)
				{
					text.DOColor(targetColor, duration).SetEase(ease, numberOfFlash, 1).SetLoops(loop, loopType).SetUpdate(ignoreTimescale);
				}
			}
		} else {
			targetColor = Random.ColorHSV(0, 1, 0, 1, 0, 1);

			if (graphicList != null) {
				//Start tweening, ignore looping, after finish tweening, decide a new color and tween again
				foreach (Graphic graphic in graphicList)
				{
					graphic.DOColor(targetColor, duration).SetEase(ease, numberOfFlash, 1).SetLoops(loop, loopType).SetUpdate(ignoreTimescale)
						.OnComplete(() => targetColor = Random.ColorHSV(0, 1, 0, 1, 0, 1));
				}
			}

			if (spriteRenderer != null) {
				spriteRenderer.DOColor(targetColor, duration).SetEase(ease, numberOfFlash, 1).SetLoops(loop, loopType).SetUpdate(ignoreTimescale)
					.OnComplete(() => targetColor = Random.ColorHSV(0, 1, 0, 1, 0, 1));
			}

			if (textGroup != null) {
				foreach (TextMeshProUGUI text in textGroup)
				{
					text.DOColor(targetColor, duration).SetEase(ease, numberOfFlash, 1).SetLoops(loop, loopType).SetUpdate(ignoreTimescale)
						.OnComplete(() => targetColor = Random.ColorHSV(0, 1, 0, 1, 0, 1));
				}
			}
		}

		yield return new WaitForSeconds(duration);
		if (onCompleteEvent != null) {
			onCompleteEvent.SetActive(true);
		}
	}
	#endregion
}
