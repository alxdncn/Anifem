using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

	enum State{
		NORMAL,
		LANDING,
		LANDED,
		TAKING_OFF
	}

	State currentState;

	delegate void StateUpdate();
	StateUpdate stateUpdate;

	Vector2 mouseMovement;
	[SerializeField] float turnSpeedY = 1f;
	[SerializeField] float turnSpeedX = 1f;
	[SerializeField] float maxSpeed = 100f;
	[SerializeField] float zHeadTurnMultiplier = 5f;

	[SerializeField] [Range(0,1)] float turnDrag = 0.1f;

	Transform headTrans;
	Transform bodyTrans;

	[SerializeField] float forwardMoveSpeed;
	[SerializeField] float sideMoveSpeed;
	[SerializeField] float upwardMoveSpeed;

	Vector3 movementMomentum;
	[SerializeField] [Range(0,1)] float movementDrag = 0.1f;

	float zLean;
	[SerializeField] [Range(0,1)] float zLeanDrag = 0.05f;

	bool flapped = false;

	float landingTimer = 0f;
	[SerializeField] float landingTime = 1f;

	float noFlapTimer = 0f;
	[SerializeField] float noFlapTimeToLand = 1f;

	[SerializeField] [Range(0, 90)] float maxYRotation = 90f;

	[SerializeField] float buzzMagnitude = 0.1f;
	float currentBuzzMagnitude;
	[SerializeField] float buzzFrequency = 20f;

	bool landing = false;
	Vector3 landingPoint;
	Vector3 landingNormal;
	Vector3 startLandPoint;
	Vector3 startLandNormal;
	Quaternion startLandingRot;

	[SerializeField] AnimationCurve animationCurve;

	[SerializeField] float objectPushDistance = 3.5f;
	[SerializeField] float objectPushForce = 0.01f;

	Collider col;

	float sineWaveHolder = 0f;

	// Use this for initialization
	void Start () {
		bodyTrans = GetComponent<Transform>();
		headTrans = bodyTrans.GetChild(0).GetComponent<Transform>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		col = GetComponent<Collider>();
		SetState(State.NORMAL);
	}

	void SetState(State newState){
		Debug.Log(newState);

		currentState = newState;
		switch(currentState){
			case State.NORMAL:
				InitNormal();
				stateUpdate = NormalUpdate;
				break;
			case State.LANDING:
				InitLanding();
				stateUpdate = LandingUpdate;
				break;
			case State.LANDED:
				InitLanded();
				stateUpdate = LandedUpdate;
				break;
			case State.TAKING_OFF:
				InitTakingOff();
				stateUpdate = TakingOffUpdate;
				break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(stateUpdate != null)
			stateUpdate();
	}

	#region NORMAL STATE
	void InitNormal(){
		col.enabled = true;
		noFlapTimer = 0f;
		flapped = true;
		landingTimer = 0f;
		currentBuzzMagnitude = buzzMagnitude;
	}

	void NormalUpdate(){
		Turn();
		Move();
		Buzz();
	}

	void Move(){
		flapped = false;
		noFlapTimer += Time.deltaTime;
		if(Input.GetButtonDown("FlapLeft")){
			sineWaveHolder = 0f;
			noFlapTimer = 0f;
			flapped = true;
			landingTimer = 0f;
			movementMomentum += bodyTrans.right * sideMoveSpeed;
			movementMomentum += bodyTrans.up * upwardMoveSpeed;
			movementMomentum += headTrans.forward * forwardMoveSpeed;
		}
		if(Input.GetButtonDown("FlapRight")){
			sineWaveHolder = 0f;
			noFlapTimer = 0f;
			flapped = true;
			landingTimer = 0f;
			movementMomentum -= bodyTrans.right * sideMoveSpeed;
			movementMomentum += bodyTrans.up * upwardMoveSpeed;
			movementMomentum += headTrans.forward * forwardMoveSpeed;
		}

		bodyTrans.position += movementMomentum;
	}

	void Buzz(){
		headTrans.localPosition = new Vector3(headTrans.localPosition.x, Mathf.Sin(sineWaveHolder * buzzFrequency) * currentBuzzMagnitude, headTrans.localPosition.z);
		sineWaveHolder += Time.deltaTime;
	}
	#endregion

	#region LANDING STATE
	void InitLanding(){
		landingTimer = 0f;
		col.enabled = false;
		currentBuzzMagnitude = buzzMagnitude;
	}

	void LandingUpdate(){
		landingTimer += Time.deltaTime;
		float t = animationCurve.Evaluate(Mathf.Clamp01(landingTimer/landingTime));
		bodyTrans.position = Vector3.Lerp(startLandPoint, landingPoint + landingNormal, t);
		bodyTrans.rotation = Quaternion.Slerp(bodyTrans.rotation, Quaternion.FromToRotation(Vector3.up, landingNormal), t);
		headTrans.localRotation = Quaternion.Slerp(headTrans.localRotation, Quaternion.Euler(Vector3.zero), t);
		currentBuzzMagnitude = Mathf.Lerp(buzzMagnitude, 0, t);
		Buzz();
		if(t == 1){
			SetState(State.LANDED);
		}
	}
	#endregion

	#region LANDED STATE
	void InitLanded(){

	}

	void LandedUpdate(){
		Turn();
		if(Input.GetButtonDown("FlapLeft") || Input.GetButtonDown("FlapRight")){
			SetState(State.TAKING_OFF);
		}
	}
	#endregion

	#region TAKING OFF STATE
	Vector3 startTakeOffPos;
	void InitTakingOff(){
		startTakeOffPos = bodyTrans.position;
		landingTimer = 0f;
		currentBuzzMagnitude = 0;
	}

	void TakingOffUpdate(){
		landingTimer += Time.deltaTime;
		float t = animationCurve.Evaluate(Mathf.Clamp01(landingTimer/landingTime));
		bodyTrans.position = Vector3.Lerp(startTakeOffPos, startLandPoint + landingNormal, t);
		bodyTrans.rotation = Quaternion.Slerp(bodyTrans.rotation, startLandingRot, t);
		headTrans.localRotation = Quaternion.Slerp(headTrans.localRotation, Quaternion.Euler(Vector3.zero), t);
		currentBuzzMagnitude = Mathf.Lerp(0, buzzMagnitude, t);
		Buzz();
		if(t == 1){
			SetState(State.NORMAL);
		}
	}
	#endregion

	void FixedUpdate(){
		mouseMovement *= 1 - turnDrag;
		movementMomentum *= 1 - movementDrag;
		zLean *= 1 - zLeanDrag;
	}

	void Turn(){
		float x = Input.GetAxisRaw("Mouse X");
		float y = Input.GetAxisRaw("Mouse Y");

		mouseMovement.x = Mathf.Clamp(mouseMovement.x + x * turnSpeedX, -maxSpeed, maxSpeed);
		mouseMovement.y = Mathf.Clamp(mouseMovement.y + y * turnSpeedY, -maxSpeed, maxSpeed);
		zLean += -mouseMovement.x * zHeadTurnMultiplier;

		float newY = headTrans.localEulerAngles.x - mouseMovement.y;


		if(newY < -maxYRotation || (newY < 360 - maxYRotation && newY > 120)){
			mouseMovement.y = 0;
			headTrans.localEulerAngles = new Vector3(-maxYRotation, headTrans.localEulerAngles.y, 0);
		}
		else if(newY > maxYRotation && newY < 360 - maxYRotation){
			mouseMovement.y = 0;
			headTrans.localEulerAngles = new Vector3(maxYRotation, headTrans.localEulerAngles.y, 0);
		}

		headTrans.Rotate(-mouseMovement.y, 0, 0);
		
		bodyTrans.Rotate(0, mouseMovement.x, 0);
		headTrans.localRotation = Quaternion.Euler(headTrans.localEulerAngles.x, headTrans.localEulerAngles.y, zLean);
	}

	void OnCollisionStay(Collision collision){
		if(currentState == State.NORMAL){
			Vector3 distVector = bodyTrans.position - collision.contacts[0].point;
			// if(distVector.sqrMagnitude < objectPushDistance * objectPushDistance){
				Vector3 localDist = headTrans.InverseTransformDirection(distVector);
				Debug.Log(Mathf.Clamp(objectPushForce / distVector.magnitude, 0, 1));
				if(localDist.x < 0){
					float force = Mathf.Clamp(objectPushForce / Mathf.Pow(distVector.magnitude, objectPushDistance), 0, 1);
					movementMomentum -= headTrans.right * force;
				} else{
					float force = Mathf.Clamp(objectPushForce / Mathf.Pow(distVector.magnitude, objectPushDistance), 0, 1);
					movementMomentum += headTrans.right * force;
				}
			// } else 
			if(noFlapTimer > noFlapTimeToLand){
				ContactPoint closest = collision.contacts[0];
				float minDist = Vector3.Distance(bodyTrans.position, closest.point);
				if(collision.contacts.Length > 1){
					for(int i = 1; i < collision.contacts.Length; i++){
						float dist = Vector3.Distance(bodyTrans.position, collision.contacts[i].point);
						if(dist < minDist){
							minDist = dist;
							closest = collision.contacts[i];
						}
					}
				}
				landingPoint = closest.point;
				landingNormal = closest.normal.normalized * 1.2f;
				startLandPoint = bodyTrans.position;
				startLandNormal = headTrans.up;
				startLandingRot = bodyTrans.rotation;
				SetState(State.LANDING);
			} 
		}
	}

	// void OnDrawGizmos(){
	// 	Gizmos.DrawSphere(landingPoint, 0.1f);
	// 	Gizmos.DrawRay(landingPoint, landingPoint + landingNormal);
	// }
}
