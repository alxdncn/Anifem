Shader "Hidden/BandsFromDepth"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ReplacementTex ("Replacement Texture", 2D) = "white" {}
		_DepthExponent ("Depth Exponent", Float) = 1
		_DepthThreshold ("Depth Threshold", Range(0,0.2)) = 0.1
		_SineFrequency ("Sine Frequency", Float) = 10
		_SineThreshold ("Sine Threhold", Range(-1, 1)) = 0
		[Toggle] _AlphaIsTransparency ("Alpha is Transparency", Int) = 1
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
				float4 vertex : SV_POSITION;
				float2 replacementUV : TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;
			sampler2D _ReplacementTex;
			uniform float4 _ReplacementTex_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.replacementUV = TRANSFORM_TEX(v.uv, _ReplacementTex);
				return o;
			}
			
			float _DepthThreshold;
			float _DepthExponent;
			float _SineFrequency;
			float _SineThreshold;
			int _AlphaIsTransparency;

			fixed4 frag (v2f i) : SV_Target
			{
				float depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv));
				depth = pow(Linear01Depth(depth), _DepthExponent);
				if(depth < 1)
					depth *= (1 - i.uv.y);
				float4 col = tex2D(_MainTex, i.uv);

				float sineY = sin(i.uv.y * _SineFrequency);

				if(depth > _DepthThreshold && sineY > _SineThreshold){
					float4 altCol = tex2D(_ReplacementTex, i.replacementUV);
					// float alpha = altCol.a * (_AlphaIsTransparency) + length(altCol.rgb) * (1 - _AlphaIsTransparency);
					float alpha = altCol.a;
					if(_AlphaIsTransparency == 0)
						alpha = length(altCol.rgb);
					return lerp(col, altCol, alpha);
				}

				return col;
			}
			ENDCG
		}
	}
}
