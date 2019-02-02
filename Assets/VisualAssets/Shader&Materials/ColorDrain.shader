Shader "Hidden/ColorDrain"
{
	Properties
	{
		//This is the "Texture" that is actually just the image rendered onscreen
		//before doing the postprocessing effect
		_MainTex ("Texture", 2D) = "white" {}

		//The saturation levels of each color, which will be set externally on ColorDrainScript
//		_RedSaturation("_RedSaturation", Range (-1, 3)) = 0
//		_GreenSaturation("_GreenSaturation", Range (-1, 3)) = 0
//		_BlueSaturation("_BlueSaturation", Range (-1, 3)) = 0

//		_RedBloom("Red Bloom", Range(1, 3)) = 1
//		_GreenBloom("Green Bloom", Range(1,3)) = 1
//		_BlueBloom("Blue Bloom", Range(1,3)) = 1
//
//		_RedBloomCutoff("Red Bloom Cutoff", Range(0,1)) = 1
//		_GreenBloomCutoff("Green Bloom Cutoff", Range(0,1)) = 1
//		_BlueBloomCutoff("Blue Bloom Cutoff", Range(0,1)) = 1

		//A variable for brightness of scene
//		_Brightness("_Brightness", Range(-1, 1)) = 0
	}
	SubShader
	{

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
 
			#include "UnityCG.cginc"

			//Declare the variables/properties
			uniform float4 _UvColor;

			uniform float _RedSaturation;
			uniform float _GreenSaturation;
			uniform float _BlueSaturation;
			uniform float _Brightness;

			uniform float _GreenUVSaturation;
			uniform float _BlueUVSaturation;
			uniform float _RedUVSaturation;
			uniform float _GreenNonUVSaturation;
			uniform float _BlueNonUVSaturation;
			uniform float _RedNonUVSaturation;

			uniform float _SuperSaturationCheck;

			uniform float _YellowBoost;

			float3 rgb2hsv(float3 c) {
              float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
              float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
              float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
              float d = q.x - min(q.w, q.y);
              float e = 1.0e-10;
              return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            float3 hsv2rgb(float3 c) {
              c = float3(c.x, clamp(c.yz, 0.0, 1.0));
              float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
              float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
              return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }


			sampler2D _MainTex;

			fixed4 frag (v2f_img i) : SV_Target
			{
				//Sample/store the screen (MainTex) pixel color
				//We'll need this later when we re-saturate the scene
				fixed4 col = tex2D(_MainTex, i.uv);
				float3 hsvColor = rgb2hsv(col.rgb);
				// return float4(hsvColor.rgb, 1); //This actually looks p good

				//Set the desaturation value based on r, g, and b
				//These colors are better than just averaging
				//see http://www.alanzucconi.com/2015/07/08/screen-shaders-and-postprocessing-effects-in-unity3d/
				float saturation = col.r * 0.33 + col.g * 0.59 * col.b * 0.11;

				//Set the values of each color, which will depend on the saturation variables
				//set above, which will be determined by an outside script
				//Also, add brightness at end
				float redValue = saturation * clamp((1 - _RedSaturation), 0, 1) + col.r * (_RedSaturation) + _Brightness;
				float greenValue = saturation * clamp((1 - _GreenSaturation), 0, 1) + col.g * (_GreenSaturation) + _Brightness;
				float blueValue = saturation * clamp((1 - _BlueSaturation), 0, 1) + col.b * (_BlueSaturation) + _Brightness;

				//Supersaturate certain areas within threshold

				//First off, should smoothstep here instead of doing a rigid cutoff
				//Secondly, there should be a way to math this
				//Thirdly, I should make it more flexible

				// fixed4 desaturatedColor = fixed4(redValue, greenValue, blueValue, 1); //* _UVDesaturationAmount;
				// fixed4 bloomColor = fixed4(redValue, greenValue, blueValue, 1) * 2;//fixed4(redValue * _RedBloomAmount, greenValue * _GreenBloomAmount, blueValue * _BlueBloomAmount, 1);

//				fixed4 newColor = smoothstep(desaturatedColor, bloomColor, 1);
//
//				return newColor;

				float3 checkHSVColor = rgb2hsv(_UvColor.rgb);

				if(abs(hsvColor.x - checkHSVColor.x) < _SuperSaturationCheck && hsvColor.z < 0.95){

					greenValue *= _GreenUVSaturation;
					blueValue *= _BlueUVSaturation;
					redValue *= _RedUVSaturation;

				} else {//if(redValue > 0.1 && greenValue > 0.1 && blueValue > 0.1){
					
					//It's very hard to get yellow in there, so add it in
					float tempYellow = ((redValue + greenValue) / 2) * _YellowBoost;
					
					greenValue *= _GreenNonUVSaturation + tempYellow;
					blueValue *= _BlueNonUVSaturation;
					redValue *= _RedNonUVSaturation + tempYellow;
		
				}

//				redValue += _RedBloomAmount * (1 - ceil(redValue - _RedBloomCutoff)*2);
//				greenValue += _GreenBloomAmount * ceil(greenValue - _GreenBloomCutoff);
//				blueValue += _BlueBloomAmount * ceil(blueValue - _BlueBloomCutoff);

				//Use these values to make a new color, and return it
				return fixed4(redValue, greenValue, blueValue, col.a);
			}
			ENDCG
		}
	}
}
