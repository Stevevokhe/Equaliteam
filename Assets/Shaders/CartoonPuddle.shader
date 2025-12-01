Shader "Custom/CartoonPuddle"
{
    Properties
    {
        _Mask ("Puddle Mask (R or A)", 2D) = "white" {}

        _BodyColor      ("Body Color", Color)   = (0.0, 0.75, 1.0, 1.0)
        _RimColor       ("Rim Color", Color)    = (0.0, 0.55, 0.95, 1.0)
        _HighlightColor ("Highlight Color", Color) = (1.0, 1.0, 1.0, 0.6)

        _RimThreshold   ("Rim Threshold", Range(0,1)) = 0.65
        _RimSoftness    ("Rim Softness",  Range(0.001,0.5)) = 0.15

        // Highlight streak
        _HighlightOffset ("Highlight Offset", Float) = 0.15
        _HighlightWidth  ("Highlight Width",  Float) = 0.25
        _HighlightSpeed  ("Highlight Speed",  Float) = 0.3

        // Ripples from center
        _RippleStrength  ("Ripple Strength", Range(0,1)) = 0.4
        _RippleFrequency ("Ripple Frequency", Float) = 20.0
        _RippleSpeed     ("Ripple Speed", Float) = 3.0
        _RippleFalloff   ("Ripple Falloff", Float) = 3.0
        _RippleWidth     ("Ripple Width", Range(0.001,0.5)) = 0.05

        _Alpha ("Overall Alpha", Range(0,1)) = 0.95
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            sampler2D _Mask;
            float4 _Mask_ST;

            float4 _BodyColor;
            float4 _RimColor;
            float4 _HighlightColor;

            float  _RimThreshold;
            float  _RimSoftness;

            float  _HighlightOffset;
            float  _HighlightWidth;
            float  _HighlightSpeed;

            float  _RippleStrength;
            float  _RippleFrequency;
            float  _RippleSpeed;
            float  _RippleFalloff;
            float  _RippleWidth;

            float  _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = TRANSFORM_TEX(v.uv, _Mask);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Mask: 0 = outside puddle, 1 = inside
                float mask = tex2D(_Mask, i.uv).a; // use alpha; switch to .r if mask is in red
                // Early out fully transparent pixels
                if (mask <= 0.001)
                    return fixed4(0,0,0,0);

                // Base color
                float3 col = _BodyColor.rgb;

                // Rim near edge of mask (where mask goes from ~0 to ~1)
                // Assuming blurred edges in texture.
                float rim = smoothstep(_RimThreshold - _RimSoftness,
                                       _RimThreshold + _RimSoftness,
                                       mask);
                col = lerp(_RimColor.rgb, col, rim); // more rim near edge

                // Centered UV for highlight & ripples
                float2 centered = i.uv - 0.5;
                float dist = length(centered);

                // Highlight streak (fake specular)
                float2 hDir = normalize(float2(-0.7, 0.5));
                float hCoord = dot(centered, hDir);
                hCoord += _HighlightOffset + sin(_Time.y * _HighlightSpeed) * 0.05;

                float highlightMask =
                    1.0 - smoothstep(-_HighlightWidth, _HighlightWidth, abs(hCoord));
                highlightMask *= mask; // only where puddle exists

                col = lerp(col, _HighlightColor.rgb,
                           highlightMask * _HighlightColor.a);

                // Ripples: expanding rings from center
                float wave = sin(dist * _RippleFrequency - _Time.y * _RippleSpeed);
                float rippleBand = smoothstep(1.0 - _RippleWidth, 1.0, wave);
                float radialFalloff = exp(-dist * _RippleFalloff);
                float rippleMask = rippleBand * radialFalloff * mask;

                col = lerp(col, 1.0.xxx, rippleMask * _RippleStrength);

                float alpha = mask * _Alpha;

                return fixed4(col, alpha);
            }
            ENDCG
        }
    }
}
