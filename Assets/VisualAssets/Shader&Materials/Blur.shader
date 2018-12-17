Shader "Hidden/Blur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
//		_BlurAmount ("Blur Amount", Range(0,10)) = 1
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			half4 _MainTex_TexelSize;
			float _BlurAmount;


			fixed4 frag (v2f_img i) : SV_Target
			{
//				_BlurAmount /= 50;
				
				float2 coords = i.uv;
				float2 samplePoints[8] = { float2(1,0), float2(1,1), float2(1,-1), float2(-1,0), float2(-1,1), float2(-1,-1), float2(0,1), float2(0,-1) };

				float2 coordsSamples[8];
				fixed4 col = (0,0,0,0);

				for(int i = 0; i < 8; i++){
					coordsSamples[i] = coords + normalize(samplePoints[i]) * _BlurAmount * _MainTex_TexelSize;
					col += tex2D(_MainTex, coordsSamples[i]);
				}

				col /= 8;
				return col;
			}
			ENDCG
		}
	}
}
