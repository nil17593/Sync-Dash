Shader "Custom/DissolveShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _DissolveTex ("Dissolve Texture (Grayscale)", 2D) = "white" {} // Noise texture for dissolve
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0.0
        _EdgeColor ("Edge Color", Color) = (1,0.5,0,1) // Color for edge glow
        _EdgeWidth ("Edge Width", Range(0, 1)) = 0.1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        sampler2D _DissolveTex;
        float _DissolveAmount;
        float4 _Color;
        float _EdgeWidth;
        float4 _EdgeColor;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_DissolveTex;
        };

        half _CalculateEdge(float dissolveThreshold, half dissolveValue, half edgeWidth)
        {
            // Determine if we're in the edge area and calculate edge strength
            return saturate((dissolveValue - dissolveThreshold) / edgeWidth);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Base texture
            half4 baseColor = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            // Dissolve texture (grayscale noise)
            half dissolveValue = tex2D(_DissolveTex, IN.uv_DissolveTex).r;

            // Calculate dissolve and edge
            half dissolveThreshold = _DissolveAmount;
            half edge = _CalculateEdge(dissolveThreshold, dissolveValue, _EdgeWidth);

            // Apply dissolve (discard parts below the threshold)
            if (dissolveValue < dissolveThreshold)
            {
                discard;
            }

            // Set the color with an edge effect at the dissolve threshold
            o.Albedo = lerp(baseColor.rgb, _EdgeColor.rgb, edge);
            o.Alpha = baseColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
