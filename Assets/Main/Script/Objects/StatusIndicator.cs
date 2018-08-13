using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StatusIndicator : MonoBehaviour {

	[Header("Component")]
    public Image statusBar;
    public SpriteRenderer statusIcon;
    private Sprite tempIcon;
	[SerializeField] private Text statusText;

	[Header("Own info")]
    public Vector3 offset;
    public bool followTarget = false;
    public Transform target;
	public bool useMaxValue = true;
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
	public float shakeTime = 0.1f;
	private Tween shaking;

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

		//Scale image
		//healthBarRect.localScale = new Vector3(value, healthBarRect.localScale.y, healthBarRect.localScale.z);
		
		DOTween.To(curVal => {
			//Fill the bar and text and other stuff!
			if (statusText != null) {
				if (useMaxValue == true) {
					statusText.text = Mathf.Round(curVal) + "/" + maxValue;
				} else {
					statusText.text = Mathf.Round(curVal).ToString();
				}
			}
			if (statusBar != null) {
				statusBar.fillAmount = curVal / lerping.maxVal;
			}
			//Set the record
			lerping.curVal = curVal;
		}, lerping.curVal, lerping.curMaxVal, lerping.timeTakenDuringLerp);
		
		if (statusBar != null) {
			//Color and alpha
			if (mapAlpha == true) {
				float newValue = inverseAlpha == false ?
					StaticGlobal.Map(0, maxValue, 0, 1, curValue * alphaMultiplier) :
					StaticGlobal.Map(0, maxValue, 0, 1, (maxValue - curValue) * alphaMultiplier);
				DOTween.ToAlpha(() => statusBar.color, curAlpha => statusBar.color = curAlpha, newValue, lerping.timeTakenDuringLerp);
			}
			if (mapColor == true) {
				Color newColor = Color.Lerp(minColor, maxColor, curValue / maxValue);
				statusBar.DOColor(newColor, lerping.timeTakenDuringLerp);
			}
		}

		//Do we shake?
		if (shakeWhenUpdated == true && shaking == null) {
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
		shaking = transform.DOShakePosition(time == -1 ? shakeTime : time, shakeStrength * scale).OnComplete(() => shaking = null);
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
        statusBar.fillAmount = 0;
		SetValue(curValue, maxValue);
    }
	#endregion
}
