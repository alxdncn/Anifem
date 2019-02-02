using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MaggotMovement : MonoBehaviour {

	char[] middleKeyboardInOrder = new char[]{
		';',
		'p',
		'.',
		'l', // lo,kimjunhybgtvfrcdexswzaq
		'o',
		',',
		'k',
		'i',
		'm',
		'j',
		'u',
		'n',
		'h',
		'y',
		'b',
		'g',
		't',
		'v',
		'f',
		'r',
		'c',
		'd',
		'e',
		'x',
		's',
		'w',
		'z',
		'a',
		'q'
	};

	[SerializeField] VideoPlayer vp;

	int valueLastFrame = -1;

	float timeWithoutInput;
	[SerializeField] float noInputTimer = 0.5f;

	float startTime;
	float endTime;

	// Use this for initialization
	void Awake () {
		vp = GetComponent<VideoPlayer>();
		vp.Pause();
		startTime = Time.time;
	}

	void OnEnable(){
		vp.started += StartTimer;
		vp.loopPointReached += StopTimer;
	}

	void OnDisable(){
		vp.started -= StartTimer;
		vp.loopPointReached -= StopTimer;
	}

	void StartTimer(VideoPlayer v){
		if(v != vp)
			return;
		// Debug.Log("START!");
		// startTime = Time.time;
	}
	
	void StopTimer(VideoPlayer v){
		if(v != vp)
			return;
		endTime = Time.time - startTime;
		Debug.Log("STOP! " + endTime);
		vp.Stop();
	}

	// Update is called once per frame
	void Update () {
		if(Input.anyKeyDown){
			timeWithoutInput = 0f;
			// Debug.Log(Input.inputString);
			int highestValueThisFrame = -1;
			for(int i = 0; i < middleKeyboardInOrder.Length; i++){
				if(middleKeyboardInOrder[i].ToString() == Input.inputString){
					highestValueThisFrame = i;
				}
			}
			if(highestValueThisFrame > -1){
				if(highestValueThisFrame > valueLastFrame && valueLastFrame != -1 && highestValueThisFrame - valueLastFrame < 12){
					// Debug.Log("FORWARD! " + (highestValueThisFrame - valueLastFrame).ToString());
					if(endTime == 0f)
						vp.Play();
				}
				valueLastFrame = highestValueThisFrame;
			}
		} else{
			if(timeWithoutInput > noInputTimer){
				valueLastFrame = -1;
				vp.Pause();
			}
			timeWithoutInput += Time.deltaTime;
		}
	}
}
