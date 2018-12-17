// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/MercatorProjectionCubemap"
{
	Properties
	{
		_Cube ("Cubemap texture", CUBE) = "white" {} 
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			ZTest Always Cull Off ZWrite Off
            Fog { Mode off }      

            CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			//#pragma fragmentoption ARB_precision_hint_nicest
			#include "UnityCG.cginc"

			#define PI    3.141592653589793
			#define TWOPI 6.283185307179587

			struct v2f {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert( appdata_img v )
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy * float2(TWOPI, PI);
				return o;
			}

			samplerCUBE _Cube;
			float4x4 _LocalTransform;

			fixed4 frag(v2f i) : COLOR 
			{
				float theta = i.uv.y;
				float phi = i.uv.x;
				float3 unit = float3(0,0,0);

				unit.x = sin(phi) * sin(theta) * -1;
				unit.y = cos(theta) * -1;
				unit.z = cos(phi) * sin(theta) * -1;

				unit = mul(unit, _LocalTransform);

				return texCUBE(_Cube, unit);
			}
			ENDCG
		}
	}
}
