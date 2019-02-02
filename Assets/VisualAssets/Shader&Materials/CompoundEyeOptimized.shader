Shader "Unlit/CompoundEyeOptimized"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float2 _Repeat;
            float _BarrelPower;
            float _EyeBarrelPower;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // initial barrel
                i.uv -= 0.5;
                float maguv = length(abs(i.uv));
                maguv *= 2.0;
                maguv = pow(maguv, _BarrelPower);
                maguv *= 0.5;
                i.uv = normalize(i.uv) * abs(maguv);
                i.uv += 0.5;
                // just for testing, can comment out
                fixed4 initCol = tex2D(_MainTex, i.uv);

                // individual barrels
                float2 repeat = _Repeat;
                // adding 10 just to account for possible negatives, will subtract out later.
                // +0.5/repeat centers it
                i.uv += 10 + 0.5 / repeat;
                float2 initEyeCoords = i.uv * repeat % 1.0;
                initEyeCoords -= 0.5;
                float2 s = sign(initEyeCoords);
                float2 eyeCoords = initEyeCoords;
                // using the magnitude keeps it "round" when it goes through the pow function
                float mag = length(abs(eyeCoords));
                mag *= 2.0;
                // increase this to do more fly eye
                mag = pow(mag, _EyeBarrelPower);
                mag /= 2.0;
                eyeCoords = normalize(eyeCoords) * mag;
                i.uv -= 10 + 0.5 / repeat;
                float2 finalCoords = i.uv + eyeCoords / repeat;
                fixed4 col = tex2D(_MainTex, finalCoords);
                if (length(abs(initEyeCoords)) > 0.5){
                    col = tex2D(_MainTex, (round(i.uv * repeat) + s * 0.5) / repeat);
                    // slight discoloration
                    col = lerp(col, float4(0.1, 0.05, 0.0, 1.0), 0.05);
                }
                return col;
                // uncomment for testing
            }
            ENDCG
        }
    }
}
