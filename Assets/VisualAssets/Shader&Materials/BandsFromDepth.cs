using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandsFromDepth : MonoBehaviour {

	[SerializeField] Material renderMaterial;
	[SerializeField] [Range(0,1)] float depthThreshold  = 0.5f;

	// Use this for initialization
	void Start () {
		GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
	}
	
	void OnRenderImage(RenderTexture source, RenderTexture destination){
		//Sets the properties on the shader based on the public variable values above
		// renderMaterial.SetFloat ("_BlurAmount", blurAmount);
		// renderMaterial.SetFloat("_DepthThreshold", depthThreshold);

		//Applies the shader material to the screen render image
		Graphics.Blit (source, destination, renderMaterial);

	}
}
