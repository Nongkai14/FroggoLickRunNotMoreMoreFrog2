Shader "Custom/AnimeWater"
{
    Properties
    {
        _Color("Water Color", Color) = (0.2, 0.5, 1, 1)
        _WaveSpeed("Wave Speed", Float) = 1.0
        _WaveStrength("Wave Strength", Float) = 0.1
        _Transparency("Transparency", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            fixed4 _Color;
            float _WaveSpeed;
            float _WaveStrength;
            float _Transparency;

            v2f vert(appdata v)
            {
                v2f o;
                float wave = sin(_Time.y * _WaveSpeed + v.vertex.x * 5.0) * _WaveStrength;
                v.vertex.y += wave;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Toon shading: ใช้ระดับความสูงหรือระยะกล้อง
                float fresnel = saturate(dot(normalize(_WorldSpaceCameraPos - i.worldPos), float3(0,1,0)));
                fresnel = smoothstep(0.5, 1.0, fresnel);
                float edge = 1 - fresnel;

                fixed4 col = _Color;
                col.a = _Transparency + edge * 0.2;
                return col;
            }
            ENDCG
        }
    }
}
