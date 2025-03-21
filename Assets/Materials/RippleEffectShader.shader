Shader "Custom/RippleEffectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RippleCenter ("Ripple Center", Vector) = (0.5, 0.5, 0, 0)
        _RippleStrength ("Ripple Strength", Float) = 0.1
        _RippleTime ("Ripple Time", Float) = 0.0
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

            sampler2D _MainTex;
            float4 _RippleCenter;
            float _RippleStrength;
            float _RippleTime;

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

            fixed4 frag (v2f i) : SV_Target
            {
                float2 ripplePos = i.uv - _RippleCenter.xy;
                float dist = length(ripplePos);
                float rippleEffect = sin(dist * 30 - _RippleTime * 15) * _RippleStrength;
                float2 rippleUv = i.uv + ripplePos * rippleEffect;
                return tex2D(_MainTex, rippleUv);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
