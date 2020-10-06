using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageTextStates : MonoBehaviour {
	//Need to make it a singleton!

	GameObject activeHolder;

	GameObject[] allHolders;

	// Use this for initialization
	void Start () {
		allHolders = new GameObject[transform.childCount];

		for(int i = 0; i < allHolders.Length; i++){
			allHolders[i] = transform.GetChild(i).gameObject;
		}

		if(allHolders.Length > 0){
			activeHolder = allHolders[0];
		} else if(allHolders.Length > 1){
			for(int i = 1; i < allHolders.Length; i++){
				allHolders[i].SetActive(false);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void FinishedTextHolder(){ //This should maybe be an event listener that TextUnscrolling.cs fires?
		GameObject holder = activeHolder.GetComponent<TextUnscrolling>().nextHolder;
		activeHolder.SetActive(false);
		holder.SetActive(true);
		activeHolder = holder;
	}

	public void SetTextHolderActive(GameObject holder){
		activeHolder.SetActive(false);
		holder.SetActive(true);
		activeHolder = holder;
	}
}
