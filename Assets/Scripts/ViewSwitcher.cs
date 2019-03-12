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

	[SerializeField] Animator beeAnimator;
	[SerializeField] string animationName;

	// Use this for initialization
	void Start () {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public void StartBeeAnim(){
		beeAnimator.Play(animationName);
	}

	public void SwitchToFingerView(){
		fingerHolder.SetActive(true);
		beeHolder.SetActive(false);
	}

	public void SwitchToBeeView(){
		beeViewCount++;
		fingerHolder.SetActive(false);
		beeHolder.SetActive(true);
		StartBeeAnim();
	}
}
