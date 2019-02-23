using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamAnimEnd : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void TurnOffCam(){
		this.gameObject.SetActive(false);
		ViewSwitcher.Instance.StartBeeAnim();
	}
}
