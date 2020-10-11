using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class SetTextureFromVideo : MonoBehaviour {

	[SerializeField] UnityEngine.UI.RawImage imageToSet;

	[SerializeField] RenderTexture renderTexture;

	Texture2D tex;

	VideoPlayer videoPlayer;

	// Use this for initialization
	void Start () {
		videoPlayer = GetComponent<VideoPlayer>();
	}
	
	// Update is called once per frame
	void Update () {
		SetTex();

	}

	void SetTex(){
		RenderTexture initRT = RenderTexture.active;

		imageToSet.color = Color.white;
		RenderTexture.active = renderTexture;

		Texture2D tex2D = imageToSet.texture as Texture2D;

		tex2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);

		tex2D.Apply();

		RenderTexture.active = initRT;
	}
}
