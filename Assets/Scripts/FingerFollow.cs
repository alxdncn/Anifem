using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerFollow : MonoBehaviour {

	[SerializeField] Transform fingerOneTrans;
	Vector3 offset;

	// Use this for initialization
	void Start () {
		offset = fingerOneTrans.position - transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = fingerOneTrans.position - offset;
	}
}
