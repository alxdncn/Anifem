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


			sampler2D _MainTex;

			fixed4 frag (v2f_img i) : SV_Target
			{
				//Sample/store the screen (MainTex) pixel color
				//We'll need this later when we re-saturate the scene
				fixed4 col = tex2D(_MainTex, i.uv);

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

//				fixed4 desaturatedColor = fixed4(redValue, greenValue, blueValue, 1); //* _UVDesaturationAmount;
//				fixed4 bloomColor = fixed4(redValue, greenValue, blueValue, 1) * 2;//fixed4(redValue * _RedBloomAmount, greenValue * _GreenBloomAmount, blueValue * _BlueBloomAmount, 1);
//
//				fixed4 newColor = smoothstep(desaturatedColor, bloomColor, 1);
//
//				return newColor;
				if(col.r > _SuperSaturationCheck){// && col.g < _SuperSaturationCheck && col.b < _SuperSaturationCheck){

					greenValue *= _GreenUVSaturation;
					blueValue *= _BlueUVSaturation;
					redValue *= _RedUVSaturation;

				} else if(redValue > 0.1 && greenValue > 0.1 && blueValue > 0.1){
					

					float tempRed = redValue;
					float tempGreen = greenValue;
					float tempBlue = blueValue;
					float tempYellow = (redValue + greenValue) / 2;
					
					greenValue = (1 - tempYellow) * _GreenNonUVSaturation;
					blueValue = (tempBlue) * _BlueNonUVSaturation;
					redValue = (1 - tempYellow) * _RedNonUVSaturation;
		
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
