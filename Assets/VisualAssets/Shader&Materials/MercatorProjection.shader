Shader "Hidden/MercatorProjection"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {} //This will be set by OnRenderImage
		_Cam0 ("Texture", 2D) = "white" {}
		_Cam1 ("Texture", 2D) = "white" {}
		_Cam2 ("Texture", 2D) = "white" {}
		_Cam3 ("Texture", 2D) = "white" {}
		_Cam4 ("Texture", 2D) = "white" {}
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
			#define PI 3.141592653589793
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _Cam0;
			sampler2D _Cam1;
			sampler2D _Cam2;
			sampler2D _Cam3;
			sampler2D _Cam4;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed2 normalizedCoords = i.uv * 2 - fixed2(1,1);

				fixed2 polarCoords;
				polarCoords.x = normalizedCoords.x * PI;
				polarCoords.y = normalizedCoords.y * PI/2;

				float3 unitVector;
				unitVector.x = cos(polarCoords.y) * cos(polarCoords.x);
				unitVector.y = sin(polarCoords.y);
				unitVector.z = cos(polarCoords.y) * sin(polarCoords.x);

				float y = normalizedCoords.y;
				y = 2.305 * log(tan(y) + (1/cos(y)));
				y = (y + 1) / 2;

				float maxVal = max(unitVector.x, max(unitVector.y, unitVector.z));
				unitVector /= maxVal;

				fixed2 coords = fixed2(i.uv.x, y);
				float eigth = 1.0/8.0;

				if(abs(unitVector.z) > abs(unitVector.y) && abs(unitVector.z) > abs(unitVector.x)){ //If we are either front or back
					if(unitVector.z > 0){ //front
						// return fixed4(0,0,0,0);
						return tex2D(_MainTex, coords);
					} else{ //back
						// return fixed4(1,1,1,0);
						return tex2D(_Cam1, coords);
					}
				} else if(abs(unitVector.y) > abs(unitVector.x)){ //if we are top or bottom
					if(unitVector.y > 0){ //top
						// return fixed4(1,0,0,0);
						return tex2D(_Cam3, coords);
					} else{ //bottom
						// return fixed4(0,1,0,0);
						return tex2D(_Cam4, coords);
					}
				} else{ //We are on the sides
					if(unitVector.x > 0){ //right
						// return fixed4(0,0,1,0);
						return tex2D(_Cam0, coords);
					} else{ //left
						// return fixed4(0,1,1,0);
						return tex2D(_Cam2, coords);
					}
				}

				

				// if(i.uv.x < subtractor){
				// 	coords.x = (coords.x + subtractor) * 4;
				// 	col = tex2D(_Cam2, coords);
				// } else if(i.uv.x < subtractor + 0.25){
				// 	coords.x = (coords.x - subtractor) * 4;
				// 	subtractor += 0.25;
				// 	col = tex2D(_MainTex, coords);
				// } else if(i.uv.x < subtractor + 0.25){
				// 	coords.x = (coords.x - subtractor) * 4;
				// 	subtractor += 0.25;
				// 	col = tex2D(_Cam0, coords);
				// } else if(i.uv.x < subtractor + 0.25){
				// 	coords.x = (coords.x - subtractor) * 4;
				// 	subtractor += 0.25;
				// 	col = tex2D(_Cam1, coords);
				// } else{
				// 	coords.x = (coords.x - subtractor) * 4;
				// 	col = tex2D(_Cam2, coords);
				// }
				
				// just invert the colors
			}
			ENDCG
		}
	}

	//x	=	i.uv.x - xStart
	//y =	ln[tan(1/4pi+1/2phi)]	
	// 	=	1/2ln((1+sinphi)/(1-sinphi))	
	// 	=	sinh^(-1)(tanphi)	
	// 	=	tanh^(-1)(sinphi)	
	// 	=	ln(tanphi+secphi).
}
