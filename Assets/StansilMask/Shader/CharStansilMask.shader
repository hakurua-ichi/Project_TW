Shader "Custom/CharStansilMask"
{
    SubShader
    {
        Tags 
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry-1"
        }
        
        ColorMask 0 // 색상 출력 없음
        ZWrite Off  // Z버퍼 쓰기 없음
        
        // 스텐실 설정은 그대로 유지
        Stencil 
        {
            Ref 1
            Comp Always
            Pass Replace
            ReadMask 255
            WriteMask 255
        }
        
        Pass
        {
            Name "StencilOnly"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings { float4 positionHCS : SV_POSITION; };
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }
            
            half4 frag() : SV_Target { return half4(0,0,0,0); }
            ENDHLSL
        }
    }
}