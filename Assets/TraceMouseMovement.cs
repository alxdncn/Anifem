using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceMouseMovement : MonoBehaviour {

	LineRenderer lineRenderer;

	Vector3 delta;
	Vector3 lastMousePos;
	Vector3 currentMousePos;

	[SerializeField] float changeThreshold = 0.1f;

	[SerializeField] int positionsToHold = 10;

	// Use this for initialization
	void Start () {
		lineRenderer = GetComponent<LineRenderer>();

		lineRenderer.positionCount = positionsToHold;

		for(int i = 0; i < positionsToHold; i++){
			lineRenderer.SetPosition(i, Input.mousePosition);
		}

		lastMousePos = Input.mousePosition;
		lastMousePos.z = 0;

	}
	
	// Update is called once per frame
	void Update () {
		Vector3 currentScreenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		currentScreenPos.z = 0;

		if(Input.GetMouseButtonDown(0)){
			for(int i = 0; i < positionsToHold; i++){
				lineRenderer.SetPosition(i, currentScreenPos);
			}
		} else if(Input.GetMouseButton(0)){
			currentMousePos = Input.mousePosition;
			currentMousePos.z = 0;

			delta = currentMousePos - lastMousePos;

			lastMousePos = currentMousePos;

			

			if(delta.magnitude >= changeThreshold){
				SetMousePositions();
				lineRenderer.SetPosition(positionsToHold - 1, currentScreenPos);
			}
		} else if(DifferenceBetweenLinePoints() > 0.01f){
			SetMousePositions();
		}
	}

	void SetMousePositions(){
		for(int i = 0; i < positionsToHold - 1; i++){
			lineRenderer.SetPosition(i, lineRenderer.GetPosition(i + 1));
		}
	}

	float DifferenceBetweenLinePoints(){
		float diff = 0;
		for(int i = 0; i < positionsToHold - 1; i++){
			diff += Vector3.Magnitude(lineRenderer.GetPosition(i) - lineRenderer.GetPosition(i + 1));
		}

		return diff;
	}
}
