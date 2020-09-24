using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLineRendererDynamically : MonoBehaviour {

	Vector3[] childrenNodes;

	[SerializeField] LineRenderer lineRenderer;

	// Use this for initialization
	void Start () {
		int children = transform.childCount;
		childrenNodes = new Vector3[children];
		lineRenderer.positionCount = children + 1;
		for(int i = 0; i < children; i++){
			childrenNodes[i] = transform.GetChild(i).GetComponent<Transform>().position;
			lineRenderer.SetPosition(i, childrenNodes[i]);
		}
		lineRenderer.SetPosition(children, childrenNodes[0]);

		
	}
	
	// Update is called once per frame
	void Update () {
	}
}
