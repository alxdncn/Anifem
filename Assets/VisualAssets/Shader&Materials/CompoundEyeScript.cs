using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CompoundEyeScript : MonoBehaviour {

	[Range(0,10)]
	public float blurAmount = 10f;

	//This gets the correct shader for the camera to use
	private Material renderMaterial;

	[SerializeField] int rows = 8;
	[SerializeField] int columns = 8;
	[SerializeField] [Range(0f,1f)] float eyeSize = 0.1f;
	[SerializeField] float barrelPower = 1;


	void Awake () {
		renderMaterial = new Material (Shader.Find ("Hidden/CompoundEye"));
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination){
		//Sets the properties on the shader based on the public variable values above
		renderMaterial.SetFloat ("_BlurAmount", blurAmount);

		renderMaterial.SetFloat ("_CompoundRows", rows);
		renderMaterial.SetFloat ("_CompoundCols", columns);
		renderMaterial.SetFloat ("_EyeSize", eyeSize);
		renderMaterial.SetFloat ("_BarrelPower", barrelPower);

		//Applies the shader material to the screen render image
		Graphics.Blit (source, destination, renderMaterial);

	}
}
