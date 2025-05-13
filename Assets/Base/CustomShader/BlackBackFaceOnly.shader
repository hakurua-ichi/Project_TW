Shader "Custom/URP/BlackBackFaceOccluder"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,1)
        _TargetPos ("Target Position", Vector) = (0,0,0,0)
        _IsInside ("Is Target Inside", Float) = 0  // 플레이어 내부 여부 (0: 외부, 1: 내부)
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        // ──────────── 1️⃣ DEPTH‐ONLY PASS (앞면만 깊이 기록) ────────────
        Pass
        {
            Name "DepthOnlyFront"
            Tags { "LightMode"="DepthOnly" }
            ZWrite On
            ColorMask 0           // 색상은 쓰지 않음
            Cull Back             // 앞면(Front)만 처리 → 뒷면은 무시

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings { float4 positionCS : SV_POSITION; };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionCS = TransformWorldToHClip(worldPos);
                return OUT;
            }
            
            float4 frag(Varyings IN) : SV_Target { return 0; }
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }

        // ──────────── 2️⃣ COLOR PASS (뒷면만, 깊이 검사 후 검은색 렌더) ────────────
        Pass
        {
            Name "ColorBack"
            Tags { "LightMode"="UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha  // 알파 블렌딩 활성화
            Cull Front            // 앞면은 무시, 뒷면만 그리기
            ZWrite Off            // 깊이는 이미 앞서 기록됨
            ZTest LEqual          // 가까운 것만 그리기

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _Color;
            float4 _TargetPos;
            float _IsInside;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionCS = TransformWorldToHClip(OUT.positionWS);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 viewDir = normalize(_TargetPos.xyz - IN.positionWS);
                float3 normalDir = normalize(IN.normalWS);
                
                // 플레이어 내부 상태에 따라 다르게 처리
                if (_IsInside > 0.5) // 플레이어가 내부에 있을 때
                {
                    float dotProduct = dot(viewDir, normalDir);
                    
                    // 플레이어 방향과 법선 방향이 비슷한 경우만 렌더링 (플레이어 뒤의 면)
                    if (dotProduct < 0.2) // 내적이 작을수록 플레이어 뒤에 있음
                    {
                        return _Color;
                    }
                    else
                    {
                        // 플레이어와 가까운 면은 투명하게
                        return float4(0, 0, 0, 0);
                    }
                }
                else // 플레이어가 외부에 있을 때
                {
                    // 기존 로직 유지 (뒷면만 렌더링)
                    return _Color;
                }
            }
            ENDHLSL
        }
    }
}
