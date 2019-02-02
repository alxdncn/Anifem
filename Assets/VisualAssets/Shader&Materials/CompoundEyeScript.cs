using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CompoundEyeScript : MonoBehaviour {


	//This gets the correct shader for the camera to use
	private Material renderMaterial;

	[SerializeField] Vector2 colsAndRows;
	[SerializeField] float eyeBarrel = 0.1f;
	[SerializeField] float barrelPower = 1;


	void Awake () {
		renderMaterial = new Material (Shader.Find ("Unlit/CompoundEyeOptimized"));
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination){
		//Sets the properties on the shader based on the public variable values above
		
		renderMaterial.SetFloat("_EyeBarrelPower", eyeBarrel);
		renderMaterial.SetFloat ("_BarrelPower", barrelPower);
		renderMaterial.SetVector("_Repeat", colsAndRows);


		//Applies the shader material to the screen render image
		Graphics.Blit (source, destination, renderMaterial);

	}
}
