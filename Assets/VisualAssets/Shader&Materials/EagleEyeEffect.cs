using UnityEngine;
using System.Collections;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("ShaderForgeShaders/Eagle/EagleEye")]
    class EagleEyeEffect: MonoBehaviour
    {

        public float bubbleSize = 0.7f;
        public Shader eagleEyeShader = null;
        private Material eagleEyeMat = null;
        public RenderTexture zoomCam = null;
        
		[SerializeField] float magnifier = 4.0f;
		[SerializeField] float divider = 1.4f;
		[SerializeField] [Range(0,1)] float smoothStepStart = 0.2f;


        void Awake(){
            eagleEyeMat = new Material(eagleEyeShader);
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            // if (CheckResources() == false)
            // {
            //     Graphics.Blit(source, destination);
            //     return;
            // }

            eagleEyeMat.SetFloat("_BubbleSize", bubbleSize);
            eagleEyeMat.SetFloat("_Magnifier", magnifier);
            eagleEyeMat.SetFloat("_Divider", divider);
            eagleEyeMat.SetFloat("_SmoothstepStart", smoothStepStart);
            eagleEyeMat.SetTexture("_ZoomTex", zoomCam);
            Graphics.Blit(source, destination, eagleEyeMat);
        }
    }
}