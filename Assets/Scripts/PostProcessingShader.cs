using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PostProcessingShader : MonoBehaviour {

	[SerializeField] bool applyShader = true;
	[SerializeField] Material postMaterial;

	// Use this for initialization
	void Start () {
		
	}
	
	void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
		if(applyShader && postMaterial != null)
			Graphics.Blit(src, dest, postMaterial);
    }
}
