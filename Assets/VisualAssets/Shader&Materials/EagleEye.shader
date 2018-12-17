// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/EagleEye"
{
	Properties{
		_MainTex("Base (RGB)", 2D) = "" {}
		_BubbleSize ("Size of barrel", float) = 0.7
		_ZoomTex("Zoomed Texture", 2D) = "" {}
		//_RenderTex("Other Camera", TEX)
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

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				//float4 col : COLOR;
			};

			sampler2D _MainTex;
			uniform sampler2D _ZoomTex;
			uniform float _BubbleSize;
			uniform float _Magnifier;
			uniform float _Divider;
			uniform float _SmoothstepStart;
			float2 intensity;

			v2f vert(appdata_img v)
			{
				v2f o;
				//o.col = v.col;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
				return o;
			}

			half4 frag(v2f i) : SV_Target
			{
				half2 coords = i.uv;

				coords = (coords - 0.5) * 2;

				half coordLength = length(coords);

				half2 distortedcoords = (coords / (1 + _Magnifier * _Divider - _Magnifier * coordLength));

				float t = coordLength;
				half2 blendedcoords = lerp(distortedcoords, coords, t * _SmoothstepStart);//smoothstep(_SmoothstepStart, _BubbleSize, t));

				half4 color = tex2D(_MainTex, (blendedcoords / 2) + 0.5);

				return color;
			}

		ENDCG
		}

		// GrabPass{}
		
		// Pass
		// {
		// 	CGPROGRAM
		// 	#pragma vertex vert
		// 	#pragma fragment frag

		// 	#include "UnityCG.cginc"

		// 	struct appdata
		// 	{
		// 		float4 vertex : POSITION;
		// 		float2 uv : TEXCOORD0;
		// 	};

		// 	struct v2f
		// 	{
		// 		float2 uv : TEXCOORD0;
		// 		float4 vertex : SV_POSITION;
		// 	};

		// 	v2f vert(appdata v)
		// 	{
		// 		v2f o;
		// 		o.vertex = UnityObjectToClipPos(v.vertex);
		// 		o.uv = v.uv;
		// 		return o;
		// 	}

		// 	sampler2D _GrabTexture;
		// 	uniform half4 _DetectThreshold;

		// 	fixed4 frag(v2f i) : COLOR
		// 	{
		// 		half4 screenCol = tex2D(_GrabTexture, i.uv);
		// 		half2 coords = i.uv;
		// 		coords = (coords - 0.5) * 2;

		// 		half coordLength = length(coords);

		// 		coordLength = pow(coordLength, 2);

		// 		coordLength = clamp(coordLength, 0.8, 5);

		// 		half4 newColor = screenCol / coordLength;

		// 		return newColor;

		// 	}
		// 	ENDCG
		// }
	}

	Fallback off

} // shader