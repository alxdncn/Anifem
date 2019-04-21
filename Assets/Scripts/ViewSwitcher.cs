using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ViewSwitcher : MonoBehaviour {

	private static ViewSwitcher instance;
	public static ViewSwitcher Instance{
		get{
			if(instance == null){
				instance = (ViewSwitcher)FindObjectOfType(typeof(ViewSwitcher));
			}
			return instance;
		}
	}

	public int beeViewCount = 0;

	[SerializeField] GameObject beeHolder;
	[SerializeField] GameObject fingerHolder;
	[SerializeField] GameObject rubHolder;

	[SerializeField] Animator beeAnimator;
	[SerializeField] string animationName;
	[SerializeField] string rubAnimationName;
	[SerializeField] float rubAnimationSpeed = 1f;

	// Use this for initialization
	void Start () {
		// Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public void StartBeeAnim(){
		beeAnimator.Play(animationName);
	}

	public void SwitchToFingerView(){
		fingerHolder.SetActive(true);
		beeHolder.SetActive(false);
		rubHolder.SetActive(false);
	}

	public void SwitchToBeeView(bool countUp){
		if(countUp)
			beeViewCount++;
		else //We would only do this if we're restarting
			beeViewCount = 0;
		fingerHolder.SetActive(false);
		rubHolder.SetActive(false);
		beeHolder.SetActive(true);
		StartBeeAnim();
	}

	public void SwitchToRubView(){
		fingerHolder.SetActive(false);
		beeHolder.SetActive(true);
		rubHolder.SetActive(false);

		beeAnimator.speed = rubAnimationSpeed;
		beeAnimator.Play(rubAnimationName);
	}

	public void SwitchToFingerRub(){
		fingerHolder.SetActive(false);
		beeHolder.SetActive(false);
		rubHolder.SetActive(true);
	}
}
