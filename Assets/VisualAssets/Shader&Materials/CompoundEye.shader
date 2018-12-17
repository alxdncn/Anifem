// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/CompoundEye"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TexTwo ("Texture", 2D) = "white" {}
		_GradientTexture ("Gradient Texture", 2D) = "white" {}
		_CompoundRows ("Eye Rows", Float) = 8
		_CompoundCols ("Eye Columns", Float) = 8
		_EyeSize ("Eye Size", Range(0,1)) = 0.1
		_BarrelPower ("Barrel Power", Float) = 1
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _DownSampledTex;
			half4 _MainTex_TexelSize;
			float _CompoundRows;
			float _CompoundCols;
			float _EyeSize;
			float _BarrelPower;
			float _BlurAmount;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed2 coords = i.uv;
				fixed2 altCoords = i.uv;
				float2 samplePoints[8] = { float2(1,0), float2(1,1), float2(1,-1), float2(-1,0), float2(-1,1), float2(-1,-1), float2(0,1), float2(0,-1) };

				float2 coordsSamples[8];
				fixed4 blurCol = (0,0,0,0);

				for(int i = 0; i < 8; i++){
					coordsSamples[i] = altCoords + normalize(samplePoints[i]) * _BlurAmount * _MainTex_TexelSize;
					blurCol += tex2D(_MainTex, coordsSamples[i]);
				}

				blurCol /= 8;

				// return blurCol;
				
				
				//Shift to have range -1, 1 to allow negative values
				coords = (coords - 0.5) * 2;
				float black = 0; 
				fixed2 barrelCoords = coords;

				for(float width = -1.5; width <= 1.5; width += 2/_CompoundCols){
					for(float height = -1.5; height <= 1.5; height += 2/_CompoundRows){
						float2 newCoords = coords - fixed2(width, height);
						float mag = length(newCoords);
						if(mag <= _EyeSize){
							float theta  = atan2(newCoords.y, newCoords.x);
    						float radius = length(newCoords);
    						radius = pow(radius, _BarrelPower);
   	 						newCoords.x = radius * cos(theta);
						    newCoords.y = radius * sin(theta);
						    //newCoords = 0.5 * (newCoords + 1.0);
						    coords = newCoords + fixed2(width, height);
							black = 1;
							break;
						}
					}
				}

				coords = lerp(coords, barrelCoords, 0.4);

				coords = 0.5 * (coords + 1.0);

				fixed4 col = tex2D(_MainTex, coords);

				return col * black + blurCol * (1 - black); //Maybe make the alternate brighter than the regular?
				
			}

			float loopThrough(float2 uvCoords, float magnitude) : COLOR {
				float black;

				for(int i = 0; i < 8; i++){
					for(int j = 0; j < 8; j++){
						uvCoords.x /= i;
						uvCoords.y /= j;
						black = magnitude < 1;
					}
				}

				return black;

			}
			ENDCG
		}
	}
}
