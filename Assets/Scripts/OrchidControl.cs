using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrchidControl : MonoBehaviour {

	[SerializeField] Transform fingerTrans;
	[SerializeField] float maxPosShake = 1f;
	[SerializeField] float minPosShake = 0f;
	[SerializeField] float maxRotShake = 1f;
	[SerializeField] float minRotShake = 0f;

	[SerializeField] float maxDist= 5;
	[SerializeField] float minDist = 1f;

	int rotDirection = 1;
	int posDirection = 1;

	Vector3 startPos;
	Vector3 startRot;

	// Use this for initialization
	void Start () {
		startPos = transform.position;
		startRot = transform.eulerAngles;

		StartCoroutine(ShakeFlower());
	}

	IEnumerator ShakeFlower(){
		while(true){
			float dist = fingerTrans.position.x - transform.position.x;

			float t = Mathf.Clamp01((dist - minDist)/(maxDist - minDist));

			float xPos = Mathf.Lerp(maxPosShake, minPosShake, t);
			transform.position = new Vector3(startPos.x + xPos * posDirection, transform.position.y, transform.position.z);
			posDirection *= -1;

			float yRot = Mathf.Lerp(maxRotShake, minRotShake, t);
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, startRot.y + yRot * rotDirection, transform.eulerAngles.z);
			rotDirection *= -1;

			yield return new WaitForSeconds(0.04f);
		}
	}
}
