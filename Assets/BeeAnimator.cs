using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeAnimator : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void EndBeeFlyin(){
		ViewSwitcher.Instance.SwitchToFingerView();
	}

	public void EndBeeRub(){
		GetComponent<Animator>().speed = 1f;
		ViewSwitcher.Instance.SwitchToFingerRub();
	}
}
