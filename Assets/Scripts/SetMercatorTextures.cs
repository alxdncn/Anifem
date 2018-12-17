using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class SetMercatorTextures : MonoBehaviour {

	Material mat;

	[SerializeField] Camera[] cameras;

	RenderTexture cubemapTex;

	// Use this for initialization
	void Awake () {
		SetupCameras();
	}

	void OnValidate(){
		SetupCameras();
	}

	void SetupCameras(){
		mat = new Material(Shader.Find("Hidden/MercatorProjectionCubemap"));
		for(int i = 1; i < cameras.Length; i++){
			cameras[i].enabled = false;
		}
	}

	void Update(){
		
	}
	
	void OnRenderImage (RenderTexture src, RenderTexture dest) {
		if(cubemapTex == null){
			cubemapTex = new RenderTexture(512, 512, 16);
			cubemapTex.dimension = UnityEngine.Rendering.TextureDimension.Cube;
			cubemapTex.hideFlags = HideFlags.HideAndDontSave;
			mat.SetTexture("_Cube", cubemapTex);
		}

		mat.SetMatrix("_LocalTransform", transform.worldToLocalMatrix);
	
		for(int i = 0; i < cameras.Length; i++){
			cameras[i].RenderToCubemap(cubemapTex, 1 << i);
		}
		Graphics.Blit(null, dest, mat);
	}
}
