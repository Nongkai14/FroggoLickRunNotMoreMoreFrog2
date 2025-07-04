Shader "Custom/AnimeWater"
{
    Properties
    {
        _ShallowColor("Shallow Water Color", Color) = (0.2, 0.5, 1, 1)
        _DeepColor("Deep Water Color", Color) = (0, 0.2, 0.6, 1)
        _DepthThreshold("Depth Threshold", Float) = 5.0
        _SurfaceSmoothness("Surface Smoothness", Range(0,1)) = 0.5
    }

     SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
            LOD 200
            Pass
        {
            Name "AnimeWaterPass"
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
            float4 _ShallowColor;
            float4 _DeepColor;
            float _DepthThreshold;
            float _SurfaceSmoothness;
        CBUFFER_END

        Varyings vert(Attributes IN)
        {
            Varyings OUT;
            // ***** เริ่มต้นการแก้ไขตรงนี้ *****
            // บรรทัดเดิม: float4 worldPos = TransformObjectToWorld(IN.positionOS);
            // เปลี่ยนเป็น:
            float3 worldPosXYZ = TransformObjectToWorld(IN.positionOS.xyz); // ส่งเฉพาะค่า xyz ไปยังฟังก์ชัน
            OUT.worldPos = worldPosXYZ; // กำหนดค่า float3 ให้กับ worldPos
            OUT.positionHCS = TransformWorldToHClip(worldPosXYZ); // ใช้ค่า float3 ที่ถูกต้อง
            // ***** สิ้นสุดการแก้ไขตรงนี้ *****
            OUT.screenPos = ComputeScreenPos(OUT.positionHCS);
            return OUT;
        }

        float4 frag(Varyings IN) : SV_Target
        {
            float2 uv = IN.screenPos.xy / IN.screenPos.w;
            float sceneDepth = SampleSceneDepth(uv);
            float linearDepth = LinearEyeDepth(sceneDepth, _ZBufferParams);

            float surfaceDepth = LinearEyeDepth(IN.positionHCS.z, _ZBufferParams);
            float depthDiff = saturate((linearDepth - surfaceDepth) / _DepthThreshold);

            float4 color = lerp(_ShallowColor, _DeepColor, depthDiff);
            color.a = 1.0 - depthDiff * 0.6; // more depth = more transparent
            return color;
        }
        ENDHLSL
    }
}
    }

    FallBack Off
}
