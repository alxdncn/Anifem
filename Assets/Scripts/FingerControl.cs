using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerControl : MonoBehaviour {

	enum State{
		ONE,
		TWO,
		THREE
	}

	State currentState = State.ONE;

	delegate void StateUpdate();
	StateUpdate stateUpdate;

	void SetState(State newState){
		currentState = newState;

		switch(newState){
			case State.ONE:
				InitStateOne();
				stateUpdate = StateOneUpdate;
				break;
			case State.TWO:
				InitStateTwo();
				stateUpdate = StateTwoUpdate;
				break;
			case State.THREE:
				InitStateThree();
				stateUpdate = StateThreeUpdate;
				break;
		}
	}

	[SerializeField] float forwardSpeed = 5f;
	[SerializeField] float angleJump = 15f;
	[SerializeField] float maxZAngle = 45f;
	[SerializeField] float minZAngle = 315f;

	[SerializeField] GameObject sweetSpot;

	[SerializeField] Camera cam;

	[SerializeField] float[] xThresholds;
	[SerializeField] float[] camSizes;
	[SerializeField] Vector3[] camPositions;

	bool canMove = true;

	// Use this for initialization
	void Start () {
		SetState(State.ONE);
	}
	
	// Update is called once per frame
	void Update () {
		if(stateUpdate != null){
			stateUpdate();
		}
	}

	#region ONE
	void InitStateOne(){
		SetCamPos(currentState);
	}

	void StateOneUpdate(){
		Move();

		if(transform.position.x < xThresholds[(int)currentState]){
			SetState(State.TWO);
		}
	}

	#endregion

	#region TWO
	void InitStateTwo(){
		SetCamPos(currentState);
	}

	void StateTwoUpdate(){
		Move();

		if(transform.position.x < xThresholds[(int)currentState]){
			SetState(State.THREE);
		}
	}

	#endregion

	#region THREE
	void InitStateThree(){
		SetCamPos(currentState);
	}

	void StateThreeUpdate(){
		Move();
	}

	#endregion

	void Move(){
		if(!canMove)
			return;
		transform.position += transform.right * -forwardSpeed * Time.deltaTime;
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

	void SetCamPos(State state){
		int stateAsInt = (int)state;
		cam.orthographicSize = camSizes[stateAsInt];
		cam.transform.position = camPositions[stateAsInt];
	}

	void OnTriggerEnter2D(Collider2D col){
		canMove = false;
		if(col.gameObject == sweetSpot)
			Debug.Log("You Win!");
		else
			Debug.Log("You Lose!");
	}
}
