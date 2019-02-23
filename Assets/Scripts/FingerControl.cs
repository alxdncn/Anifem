using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerControl : MonoBehaviour {

	enum State{
		PROFILE,
		TOP
	}

	State currentState = State.PROFILE;

	delegate void StateUpdate();
	StateUpdate stateUpdate;

	void SetState(State newState){
		currentState = newState;

		switch(newState){
			case State.PROFILE:
				InitStateOne();
				stateUpdate = StateOneUpdate;
				break;
			case State.TOP:
				InitStateFront();
				stateUpdate = FrontUpdate;
				break;
		}
	}

	enum ProfilePositions{
		ONE,
		TWO,
		THREE
	}

	ProfilePositions currentProfilePosition = ProfilePositions.ONE;
	ProfilePositions currentTopPosition = ProfilePositions.ONE;

	[SerializeField] float forwardSpeed = 5f;
	[SerializeField] float angleJump = 15f;
	[SerializeField] float maxZAngle = 45f;
	[SerializeField] float minZAngle = 315f;

	[SerializeField] GameObject sweetSpot;

	[Header("Profile Mode")]
	[SerializeField] Camera profileCam;

	[SerializeField] float[] xThresholds;
	[SerializeField] float[] camSizes;
	[SerializeField] Vector3[] camPositions;

	[Header("Top Mode")]
	[SerializeField] Camera topCam;
	[SerializeField] float moveAmount = 0.2f;
	[SerializeField] float[] topXThresholds;
	[SerializeField] float[] topCamSizes;
	[SerializeField] Vector3[] topCamPositions;
	[SerializeField] Vector3[] topCamRotations;


	bool canMove = true;

	// Use this for initialization
	void Start () {
		SetState(State.PROFILE);
	}
	
	// Update is called once per frame
	void Update () {
		if(stateUpdate != null){
			stateUpdate();
		}
	}

	#region PROFILE
	void InitStateOne(){
		topCam.enabled = false;
		profileCam.enabled = true;
		SetCamPos(currentProfilePosition, profileCam, camSizes, camPositions);
	}

	void StateOneUpdate(){
		MoveForward();
		MoveUpAndDown();

		if(transform.position.x < xThresholds[(int)currentProfilePosition]){
			currentProfilePosition++;
			SetState(State.TOP);
		}
	}

	#endregion

	#region FRONT
	void InitStateFront(){
		topCam.enabled = true;
		profileCam.enabled = false;
		SetCamPos(currentTopPosition, topCam, topCamSizes, topCamPositions, topCamRotations);
	}

	void FrontUpdate(){
		// frontCam.orthographicSize = Mathf.Lerp(endCamSize, startCamSize, Mathf.Clamp01((fingerEndXPosition - transform.position.x)/fingerStartDistance));

		MoveForward();
		MoveSideWays();

		if((int)currentTopPosition < topXThresholds.Length && transform.position.x < topXThresholds[(int)currentTopPosition]){
			currentTopPosition++;
			SetState(State.PROFILE);
		}
	}

	void MoveSideWays(){
		if(!canMove)
			return;
			
		if(Input.GetKeyDown(KeyCode.LeftArrow)){
			transform.position -= new Vector3(0, 0, moveAmount);
		} else if(Input.GetKeyDown(KeyCode.RightArrow)){
			transform.position += new Vector3(0, 0, moveAmount);
		}
	}
	#endregion

	void MoveUpAndDown(){
		if(Input.GetKeyDown(KeyCode.Space)){
			SetState(State.TOP);
		}
		if(!canMove)
			return;
		if(Input.GetKeyDown(KeyCode.UpArrow)){
			transform.Rotate(0, 0, angleJump);
			if(transform.eulerAngles.z > maxZAngle && transform.eulerAngles.z < 180){
				transform.eulerAngles = new Vector3(0, 0, maxZAngle);
			}
		} else if(Input.GetKeyDown(KeyCode.DownArrow)){
			transform.Rotate(0, 0, -angleJump);
			if(transform.eulerAngles.z < minZAngle && transform.eulerAngles.z > 180){
				transform.eulerAngles = new Vector3(0, 0, minZAngle);
			}
		}
	}

	void MoveForward(){
		if(canMove)
			transform.position += transform.right * -forwardSpeed * Time.deltaTime;

		if(Input.GetKeyDown(KeyCode.Space)){
			ViewSwitcher.Instance.SwitchToBeeView();
		}
	}

	void SetCamPos(ProfilePositions position, Camera cam, float[] sizes, Vector3[] positions, Vector3[] rotations = null){
		int stateAsInt = (int)position;
		cam.orthographicSize = sizes[stateAsInt];
		cam.transform.position = positions[stateAsInt];
		if(rotations != null){
			cam.transform.eulerAngles = rotations[stateAsInt];
		}
	}

	void OnTriggerEnter(Collider col){
		canMove = false;
		if(col.gameObject == sweetSpot)
			Debug.Log("You Win!");
		else
			Debug.Log("You Lose!");
	}
}
