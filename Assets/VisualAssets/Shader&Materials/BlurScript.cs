using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BlurScript : MonoBehaviour {


	[Range(0,10)]
	public float blurAmount = 10f;

	//This gets the correct shader for the camera to use
	private Material renderMaterial;

	void Awake () {
		renderMaterial = new Material (Shader.Find ("Hidden/Blur"));
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination){
		//Sets the properties on the shader based on the public variable values above
		renderMaterial.SetFloat ("_BlurAmount", blurAmount);

		//Applies the shader material to the screen render image
		Graphics.Blit (source, destination, renderMaterial);

	}
}
