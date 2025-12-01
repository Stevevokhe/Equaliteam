Shader "Custom/StylizedFire"
{
    Properties
    {
        _Brightness    ("Brightness",   Float)           = 3.0
        _EdgeWidth     ("Edge Softness (thin edge)", Range(0.0,0.5)) = 0.05
        _Cutoff        ("Hard Edge Cutoff", Range(0.3,1.0)) = 0.75
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        // Additive fire look
        Blend SrcAlpha One
        Cull Off
        ZWrite Off

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
                fixed4 color  : COLOR;
            };

            struct v2f
            {
                float4 pos   : SV_POSITION;
                float2 uv    : TEXCOORD0;
                fixed4 color : COLOR;
            };

            float _Brightness;
            float _EdgeWidth;
            float _Cutoff;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos   = UnityObjectToClipPos(v.vertex);
                o.uv    = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Center UV -> [-1,1]
                float2 p = i.uv * 2.0 - 1.0;
                float r = length(p);

                // HARD CUTOFF
                // r < cutoff = full opacity
                // r > cutoff = soft edge to zero
                float edgeStart = _Cutoff;
                float edgeEnd = _Cutoff + _EdgeWidth;

                float mask = smoothstep(edgeEnd, edgeStart, r);

                if (mask <= 0.001)
                    return 0;

                fixed4 col = i.color;
                col.rgb *= _Brightness * mask;
                col.a   *= mask;

                return col;
            }
            ENDCG
        }
    }
}
