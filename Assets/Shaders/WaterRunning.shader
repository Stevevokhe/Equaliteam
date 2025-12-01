Shader "Custom/WaterTubeUnlit"
{
    Properties
    {
        _Tint ("Tint", Color) = (0.0, 0.55, 1.0, 1.0)

        // Base flow
        _NoiseScale ("Noise Scale", Float) = 2.0
        _NoiseSpeed ("Noise Speed", Float) = 1.0
        _BrightnessVariation ("Brightness Variation", Range(0,1)) = 0.2

        // 1 = one direction, -1 = reversed
        _FlowDirection ("Flow Direction", Float) = 1.0

        // Edge / opacity
        _EdgeSoftness ("Edge Softness", Float) = 1.5
        _EdgeHighlight ("Edge Highlight", Range(0,1)) = 0.25
        _VerticalFade ("Vertical Fade Amount", Range(0,1)) = 0.05
        _Opacity ("Opacity", Range(0,1)) = 1.0

        // Caustics
        _CausticColor ("Caustic Color", Color) = (1,1,1,1)
        _CausticIntensity ("Caustic Intensity", Range(0,3)) = 1.5

        _CausticScale1 ("Caustic Scale 1", Float) = 2.0
        _CausticScale2 ("Caustic Scale 2", Float) = 2.3
        _CausticSpeed1 ("Caustic Speed 1", Float) = 1.5
        _CausticSpeed2 ("Caustic Speed 2", Float) = 1.8

        _CausticThreshold ("Caustic Threshold", Range(0,1)) = 0.55
        _CausticSoftness ("Caustic Softness", Range(0.001,0.5)) = 0.08
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite On

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos       : SV_POSITION;
                float2 uv        : TEXCOORD0;
                float3 normalWS  : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
            };

            float4 _Tint;
            float  _NoiseScale;
            float  _NoiseSpeed;
            float  _BrightnessVariation;
            float  _FlowDirection;
            float  _EdgeSoftness;
            float  _EdgeHighlight;
            float  _VerticalFade;
            float  _Opacity;

            float4 _CausticColor;
            float  _CausticIntensity;
            float  _CausticScale1;
            float  _CausticScale2;
            float  _CausticSpeed1;
            float  _CausticSpeed2;
            float  _CausticThreshold;
            float  _CausticSoftness;

            // ---- simple smooth value noise ----
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            float noise2D(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);

                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));

                float2 u = f * f * (3.0 - 2.0 * f);

                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                o.uv = v.uv;
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.normalWS = UnityObjectToWorldNormal(v.normal);
                o.viewDirWS = _WorldSpaceCameraPos - worldPos.xyz;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 n = normalize(i.normalWS);
                float3 v = normalize(i.viewDirWS);

                // Fresnel edge term
                float fresnel = pow(1.0 - saturate(dot(n, v)), _EdgeSoftness);

                // ===== Base blue water (UV-based flow) =====
                // Assume cylinder UV.y goes from bottom (0) to top (1).
                // _FlowDirection = 1 -> one way, -1 -> reversed (set in inspector).
                float2 flowUV = float2(
                    i.uv.x * _NoiseScale,
                    i.uv.y * _NoiseScale + _Time.y * _NoiseSpeed * _FlowDirection
                );

                float baseNoise = noise2D(flowUV);       // 0..1
                float baseCentered = (baseNoise - 0.5) * 2.0;

                fixed3 baseCol = _Tint.rgb;
                float brightness = 1.0 + _BrightnessVariation * baseCentered;
                baseCol *= brightness;

                // slight vertical fade (mostly solid)
                float vFade = lerp(1.0 - _VerticalFade, 1.0, saturate(i.uv.y));
                baseCol *= vFade;

                // ===== Caustics: intersection of 2 UV noises =====
                // Both scroll along +FlowDirection.

                float2 cUV1 = float2(
                    i.uv.x * _CausticScale1,
                    i.uv.y * _CausticScale1 + _Time.y * _CausticSpeed1 * _FlowDirection
                );

                float2 cUV2 = float2(
                    (i.uv.x + i.uv.y * 0.5) * _CausticScale2,
                    i.uv.y * _CausticScale2 + _Time.y * _CausticSpeed2 * _FlowDirection
                );

                float c1 = noise2D(cUV1);
                float c2 = noise2D(cUV2);

                float combined = c1 * c2;

                float causticMask = smoothstep(
                    _CausticThreshold,
                    _CausticThreshold + _CausticSoftness,
                    combined
                );

                fixed3 causticCol = _CausticColor.rgb * (causticMask * _CausticIntensity);

                // ===== Combine & edge highlight =====
                fixed3 colRGB = baseCol + causticCol;
                colRGB = lerp(colRGB, colRGB * (1.0 + _EdgeHighlight), fresnel);

                fixed4 col;
                col.rgb = saturate(colRGB);
                col.a   = _Opacity;

                return col;
            }
            ENDCG
        }
    }
}
