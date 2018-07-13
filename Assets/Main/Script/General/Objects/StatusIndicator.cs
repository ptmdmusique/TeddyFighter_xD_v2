﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StatusIndicator : MonoBehaviour {

    [SerializeField]
    private RectTransform statusRect;
    public Image statusImage;
    public SpriteRenderer statusIcon;
    private Sprite tempIcon;
    [SerializeField]
    private Text statusText;

    public Vector3 offset;
    public bool followTarget = false;
    public Transform target;
    public string statusName;
    private Animator myAnim;

    [Header("Image Alpha Effect")]
    public bool mapAlpha = false;               //Do we increase object's alpha as value increase
    public bool inverseAlpha = false;           //Value increase -> alpha increase
    public float alphaMultiplier = 1;           //More intense?

	[Header("Image Color Effect")]
    public bool mapColor = false;
    public Color minColor = Color.black;
	public Color maxColor = Color.white;

	[Header("Shaking")]
	public bool shakeWhenUpdated = true;
	public Vector3 shakeStrength;
	public float shakeTime = 1f;
    
	//Lerping stuff
	private class LerpInfo
	{
		public float timeTakenDuringLerp = 1f;
		public bool isLerping = false;

		public float curMaxVal = -1;
		public float maxVal;
		public float curVal;
	}
	private LerpInfo lerping;
	private Tweener tween;

	#region Default
	private void Awake() {
        myAnim = GetComponent<Animator>();

		lerping = new LerpInfo();
    }
    private void Start() {
        SetTarget(target);
    }
    private void Update() {
        if (followTarget == true) {
            transform.position = offset + target.position;
        }
	}
	#endregion

	#region Action
	public void SetValue(float curValue, float maxValue) {
		//Reset the value
		lerping.curMaxVal = curValue;
		lerping.maxVal = maxValue;

		//Reset the tween
		tween.Kill();

		//Scale image
		//healthBarRect.localScale = new Vector3(value, healthBarRect.localScale.y, healthBarRect.localScale.z);
		
		tween = DOTween.To(curVal => {
			//Fill the bar and text and other stuff!
			if (statusText != null) {
				statusText.text = Mathf.Round(curVal) + "/" + maxValue + " " + statusName;
			}
			if (statusImage != null) {
				statusImage.fillAmount = curVal / lerping.maxVal;

				//Color and alpha
				if (mapAlpha == true) {
					float newValue = inverseAlpha == false ?
						StaticGlobal.Map(0, maxValue, 0, 1, curValue * alphaMultiplier) :
						StaticGlobal.Map(0, maxValue, 0, 1, (maxValue - curValue) * alphaMultiplier);
					DOTween.ToAlpha(() => statusImage.color, curAlpha => statusImage.color = curAlpha, newValue, lerping.timeTakenDuringLerp);
				}
				if (mapColor == true) {
					Color newColor = Color.Lerp(minColor, maxColor, curValue / maxValue);
					statusImage.DOColor(newColor, lerping.timeTakenDuringLerp);
				}
			}
			
			//Set the record
			lerping.curVal = curVal;
		}, lerping.curVal, lerping.curMaxVal, lerping.timeTakenDuringLerp);
		
		//Do we shake?
		if (shakeWhenUpdated == true) {
			//Scale the shake based on the percentage of curVal & maxVal
			Shake(1 + curValue / maxValue);
		}
	}
	//Change max value
	public void ChangeMaxValue(float newMax)
	{
		lerping.maxVal = newMax;
		SetValue(lerping.curVal, lerping.maxVal);
	}
    //Set up new target
    public void SetTarget(Transform newTarget) {
        if (newTarget != null) {
            offset = transform.position - newTarget.position;
        }
    }
	public void Shake(float scale = 1, float time = -1f)
	{
		transform.DOShakePosition(time == -1 ? shakeTime : time, shakeStrength * scale);
	}
    public void ChangeIcon() {
        if (tempIcon != null) {
            statusIcon.sprite = tempIcon;
        }

        tempIcon = null;
    }
    public void ChangeIcon(Sprite newIcon) {
        myAnim.SetTrigger("Change");
        tempIcon = newIcon;
        Invoke("ChangeIcon", 0.20f);
    }
	public void RestartBar(float curValue, float maxValue) {
        statusImage.fillAmount = 0;
		SetValue(curValue, maxValue);
    }
	#endregion
}