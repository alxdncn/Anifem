using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestValue : MonoBehaviour {

	struct TestStruct{
		public float x;

		public void IncrementX(){
			x += 1;
		}
	}

	class TestClass{
		public float x;
	}

	// Use this for initialization
	void Start () {
		TestStruct structOne = new TestStruct();
		TestStruct structTwo = structOne;

		structOne.x = 1;
		Debug.Log("Struct Two x??? : " + structTwo.x);

		TestClass classOne = new TestClass();
		TestClass classTwo = classOne;

		classOne.x = 1;
		Debug.Log("Class Two x??? : " + classTwo.x);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
