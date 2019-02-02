using UnityEngine;
using System.Collections;

//JUST ADD THIS SCRIPT AS A COMPONENT TO THE CAMERA TO GET THE SHADER TO WORK!
[ExecuteInEditMode]
public class ColorDrainScript : MonoBehaviour {

	//variables, with range values, to set the shader properties
	//I put such big ranges to allow strange visual effects
	//Going to -1 takes out all of that color in the scene
	//Going above 1 supersaturates the scene with that color (may effect Bloom shaders)
	[Range(-1,3)]
	public float redSaturation = 0;
	[Range(-1,3)]
	public float greenSaturation = 0;
	[Range(-1,3)]
	public float blueSaturation = 0;
	[Range(-1,1)]
	public float brightness = 0;

	[Range(-1,5)]
	public float redUVSaturation = 1;
	[Range(-1,5)]
	public float redNonUVSaturation = 1;
	[Range(-1,5)]
	public float greenUVSaturation = 1;
	[Range(-1,5)]
	public float greenNonUVSaturation = 1;
	[Range(-1,5)]
	public float blueUVSaturation = 1;
	[Range(-1,5)]
	public float blueNonUVSaturation = 1;

	[Range(0,1)]
	public float uvSuperSaturationChecker = 0.7f;
	[Range(0,1)]
	public float generalDesaturation = 0.5f;

	[Range(0,1)]
	public float yellowBoost = 0.2f;

	[SerializeField] Color uvCheckColor = Color.magenta;

	//This gets the correct shader for the camera to use
	private Material renderMaterial;


	void Awake () {
		renderMaterial = new Material (Shader.Find ("Hidden/ColorDrain"));
		Shader.SetGlobalFloat ("_GeneralDesaturation", generalDesaturation);

	}

	void OnRenderImage(RenderTexture source, RenderTexture destination){

		//Sets the properties on the shader based on the public variable values above
		//Should be set in awake eventually, though it's nice to have it hear to test
		renderMaterial.SetFloat ("_RedSaturation", redSaturation);
		renderMaterial.SetFloat ("_GreenSaturation", greenSaturation);
		renderMaterial.SetFloat ("_BlueSaturation", blueSaturation);
		renderMaterial.SetFloat ("_Brightness", brightness);
		renderMaterial.SetFloat ("_RedUVSaturation", redUVSaturation);
		renderMaterial.SetFloat ("_RedNonUVSaturation", redNonUVSaturation);
		renderMaterial.SetFloat ("_GreenUVSaturation", greenUVSaturation);
		renderMaterial.SetFloat ("_GreenNonUVSaturation", greenNonUVSaturation);
		renderMaterial.SetFloat ("_BlueUVSaturation", blueUVSaturation);
		renderMaterial.SetFloat ("_BlueNonUVSaturation", blueNonUVSaturation);
		renderMaterial.SetFloat ("_SuperSaturationCheck", uvSuperSaturationChecker);
		renderMaterial.SetVector("_UvColor", uvCheckColor);
		renderMaterial.SetFloat("_YellowBoost", yellowBoost);

		//Applies the shader material to the screen render image
		Graphics.Blit (source, destination, renderMaterial);

	}
}
