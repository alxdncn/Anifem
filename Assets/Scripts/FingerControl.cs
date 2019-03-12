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
				InitStateTop();
				stateUpdate = TopUpdate;
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
	[SerializeField] float maxZAngle = 45f;
	[SerializeField] float minZAngle = 315f;

	[Header("Collision Stuff")]
	[SerializeField] Transform contactPoint;
	[SerializeField] GameObject sweetSpot;
	[SerializeField] float maxDistFromSpot = 1.0f;
	[SerializeField] float maxPoints = 100f;

	[Header("Profile Mode")]
	[SerializeField] Camera profileCam;

	[SerializeField] float[] xThresholds;
	[SerializeField] float[] camSizes;
	[SerializeField] Vector3[] camPositions;
	[SerializeField] float angleMoveSpeed = 1f;

	[Header("Top Mode")]
	[SerializeField] Camera topCam;
	[SerializeField] float[] topXThresholds;
	[SerializeField] float[] topCamSizes;
	[SerializeField] Vector3[] topCamPositions;
	[SerializeField] Vector3[] topCamRotations;
	[SerializeField] float positionMoveSpeed = 0.1f;

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

	void MoveUpAndDown(){
		if(!canMove)
			return;

		float mouseChange = Input.GetAxis("Mouse Y");

		transform.Rotate(0, 0, mouseChange * angleMoveSpeed);

		if(transform.eulerAngles.z > maxZAngle && transform.eulerAngles.z < 180){
			transform.eulerAngles = new Vector3(0, 0, maxZAngle);
		}
		if(transform.eulerAngles.z < minZAngle && transform.eulerAngles.z > 180){
			transform.eulerAngles = new Vector3(0, 0, minZAngle);
		}
	}
	#endregion

	#region TOP
	void InitStateTop(){
		topCam.enabled = true;
		profileCam.enabled = false;
		SetCamPos(currentTopPosition, topCam, topCamSizes, topCamPositions, topCamRotations);
	}

	void TopUpdate(){
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

		float mouseChange = Input.GetAxis("Mouse X");

		transform.position += new Vector3(0, 0, mouseChange * positionMoveSpeed);
	}
	#endregion

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
		if(!canMove)
			return;
			
		canMove = false;

		float distFromSpot = Vector3.Distance(contactPoint.position, sweetSpot.transform.position);

		//.35 should be pretty good
		//.57 should be pretty bad
		//.84 should be very bad
		float points = Mathf.Lerp(maxPoints, 0, Mathf.Clamp01(distFromSpot/maxDistFromSpot));
		Debug.Log("Points: " + points);
	}
}
