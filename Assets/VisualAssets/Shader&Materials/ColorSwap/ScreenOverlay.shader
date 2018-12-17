Shader "Hidden/ScreenOverlay"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_OverlayTex("Overlay Texture", 2D) = "white" {}
		_MainColorThreshold("Main Threshold", Range(0,1)) = 0.5
		_SecondaryThreshold("Secondary Threshold", Range(0,1)) = 0.5
		_TextureTint("Texture Tint Color", Color) = (1,1,1,1)
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
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _OverlayTex;
			float _MainColorThreshold;
			float _SecondaryThreshold;
			float4 _TextureTint;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				if(col.g > _MainColorThreshold && col.b < _SecondaryThreshold && col.r < _SecondaryThreshold){
					fixed4 colTwo = tex2D(_OverlayTex, i.uv) * _TextureTint; //will need to replace with screen space UV
					col = lerp(col, colTwo, colTwo.a);
				}

				return col;
			}
			ENDCG
		}
	}
}
