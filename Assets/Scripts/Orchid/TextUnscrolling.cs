using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextUnscrolling : MonoBehaviour {

	#region State Machine Declarations
	enum State{
		NotScrolling,
		NormalScrolling,
		FastScrolling,
		FadingButton
	}

	State lastState;
	State currentState;

	delegate void InitState();
	InitState initState;
	delegate void StateUpdate();
	StateUpdate stateUpdate;

	void SetState (State newState, bool reset = false){
		if(currentState == newState){
			lastState = currentState; // I think this is what I should do here, a bit ambiguous
			if(initState != null && reset)
				initState();
			return;
		}

		switch (newState){
			case State.NotScrolling:
				initState = InitNotScrolling;
				stateUpdate = UpdateNotScrolling;
				break;
			case State.NormalScrolling:
				initState = InitNormalScrolling;
				stateUpdate = UpdateNormalScrolling;
				break;
			case State.FastScrolling:
				initState = InitFastScrolling;
				stateUpdate = UpdateFastScrolling;
				break;
			case State.FadingButton:
				initState = InitFadingButton;
				stateUpdate = UpdateFadingButton;
				break;
		}

		lastState = currentState;
		currentState = newState;

		if(initState != null){
			initState();
		}
	}
	#endregion

	[SerializeField] bool debug = false;

	public GameObject nextHolder;

	bool overButton = false;

	string initialText;
	List<Text> textComponents;
	int currentTextComponent = 0;

	[SerializeField] float normalTextCharsPerSecond = 1.0f;
	[SerializeField] float fastTextCharsPerSecond = 100.0f;
	float textRevealSpeed;
	float timeSinceLastCharReveal = 0;
	int characterIndex = 0;
	bool doneText = false;

	Button[] buttons;
	int buttonIndex = 0;
	Text buttonText;
	float buttonFadeTime;
	[SerializeField] float normalButtonFadeTime = 0.5f;
	[SerializeField] float fastButtonFadeTime = 0.1f;

	Color startColor;
	Color endColor;
	float t;

	// Use this for initialization
	void Start () {
		textComponents = new List<Text>();

		for(int i = 0; i < transform.childCount; i++){
			Text getText = transform.GetChild(i).GetComponent<Text>();
			if(getText != null){
				textComponents.Add(getText);
			}
		}

		Debug.Log(textComponents[0].text);

		initialText = textComponents[0].text;
		textComponents[0].text = "";

		for(int i = 1; i < textComponents.Count; i++){
			textComponents[i].enabled = false;
		}

		buttons = GetComponentsInChildren<Button>();

		for(int i = 0; i < buttons.Length; i++){
			Debug.Log("Disabling button " + buttons[i].gameObject.name);
			buttons[i].gameObject.SetActive(false);
		}

		SetState(State.NormalScrolling);
	}
	
	// Update is called once per frame
	void Update () {
		if(stateUpdate != null){
			stateUpdate();
		}

		if(Input.GetKeyDown(KeyCode.S) && debug){
			Debug.Log("Current state is " + currentState);
		}
	}

	#region NotScrolling
	void InitNotScrolling(){
	}

	void UpdateNotScrolling(){
		if(Input.GetMouseButtonDown(0) && !overButton){
			GoToNextBlock();
			SetState(State.NormalScrolling);
		}
	}

	void GoToNextBlock(){
		textComponents[currentTextComponent].enabled = false;
		currentTextComponent++;
		initialText = textComponents[currentTextComponent].text;
		textComponents[currentTextComponent].text = "";
		textComponents[currentTextComponent].enabled = true;
	}
	#endregion

	#region NormalScrolling
	void InitNormalScrolling(){
		if(lastState != State.FadingButton)
			characterIndex = 0; //only reset the character index if we weren't just fading in a button

		textRevealSpeed = normalTextCharsPerSecond;
		timeSinceLastCharReveal = 1.0f / textRevealSpeed; //make it show the first letter right away
	}

	void UpdateNormalScrolling(){
		ScrollThroughText();
		if(Input.GetMouseButtonDown(0) && !overButton){
			SetState(State.FastScrolling);
		}
	}
	#endregion

	#region FastScrolling
	void InitFastScrolling(){
		textRevealSpeed = fastTextCharsPerSecond;
		timeSinceLastCharReveal = 1.0f / textRevealSpeed; //make it show the first letter right away
	}

	void UpdateFastScrolling(){
		ScrollThroughText();
	}
	#endregion

	#region FadingButton
	void InitFadingButton(){
		if(buttonIndex >= buttons.Length){
			SetState(lastState);
			return;
		}

		buttonText = buttons[buttonIndex].GetComponentInChildren<Text>();
		startColor = buttonText.color;
		endColor = startColor;

		startColor.a = 0;
		endColor.a = 1;

		t = 0;

		buttonText.color = startColor; //make the text invisible
		buttons[buttonIndex].gameObject.SetActive(true);

		if(lastState == State.NormalScrolling)
			buttonFadeTime = normalButtonFadeTime;
		else
			buttonFadeTime = fastButtonFadeTime;
	}

	void UpdateFadingButton(){
		buttonText.color = Color.Lerp(startColor, endColor, t);

		if(t >= 1){
			buttonIndex++;
			SetState(lastState);
		}

		t += buttonFadeTime * Time.deltaTime; //should maybe make this more deterministic, like number of seconds it takes
	}
	#endregion

	void ScrollThroughText(){
		float threshold = 1.0f/textRevealSpeed;

		int charsToReveal = Mathf.FloorToInt(timeSinceLastCharReveal / threshold);

		for(int i = 0; i < charsToReveal;){
			if(characterIndex < initialText.Length){
				if(initialText[characterIndex] != ' '){	//only iterate on the for loop if we didn't write a blank character!
					i++;
				}
				if(initialText[characterIndex] == '|'){
					textComponents[currentTextComponent].text += " "; //add a space so we don't mess up the kerning
					characterIndex++; //increment the index so we don't write the | when we return to showing text
					SetState(State.FadingButton);
					return;
				}
				timeSinceLastCharReveal = 0f;
				textComponents[currentTextComponent].text += initialText[characterIndex++];
			} else{
				SetState(State.NotScrolling);
				break; //otherwise we could get caught in an infinite loop if the last character is a blank
			}
		}

		timeSinceLastCharReveal += Time.deltaTime;
	}


	#region Button Events
	public void OverButton(){
		overButton = true;
	}

	public void ExitButton(){
		overButton = false;
	}

	public void JumpToText(GameObject textHolderToActivate){

	}
	#endregion
	
}
