using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class BGMusicPlayer : MonoBehaviour
{
	[Header("Player stuff")]
	public bool shuffle = false;
	public bool playOnStart = true;
	public TextMeshProUGUI musicName;
	private bool isPlaying = false;

	[Header("Audio Stuff")]
	public AudioClip[] myAudioClips;
	private AudioSource myAudioSource;
	private int curIndx = 0;

	//Singleton stuff
	private static BGMusicPlayer musicInstance;

	void Awake()
	{
		//Singleton stuff
		if (musicInstance != null && musicInstance != this) {
			Destroy(this.gameObject);
			return;
		}

		musicInstance = this;
		myAudioSource = GetComponent<AudioSource>();
	}

	// Use this for initialization
	void Start()
	{
		if (isPlaying == false) {
			//Haven't started yet
			if (shuffle == true) {
				ShuffleDeck();
			}

			if (playOnStart == true) {
				//Play the track
				NextTrack();
			}
		}
	}

	public void ShuffleDeck()
	{
		//Knuth unbiased algorithm
		for (int indx = 0; indx < myAudioClips.Length; indx++) {
			AudioClip temp = myAudioClips[indx];
			int indx2 = Random.Range(indx, myAudioClips.Length);
			myAudioClips[indx] = myAudioClips[indx2];
			myAudioClips[indx2] = temp;
		}
	}

	public void NextTrack()
	{
		myAudioSource.clip = myAudioClips[curIndx++];
		myAudioSource.Play();

		if (curIndx >= myAudioClips.Length) {
			curIndx = 0;
		}

		if (musicName != null) {
			musicName.text = myAudioSource.clip.name;
			//Fading stuff
			musicName.DOFade(1, 1).OnComplete(
				() => musicName.DOFade(0, 1).SetDelay(2));
		}

		Invoke("NextTrack", myAudioSource.clip.length);
	}
}
