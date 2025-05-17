// URP용 Camera-DepthTexture 셰이더
Shader "Hidden/Universal/Camera-DepthTexture"
{
    Properties
    {
        _MainTex ("", 2D) = "white" {}
        _Cutoff ("", Float) = 0.5
        _Color ("", Color) = (1,1,1,1)
    }
    
    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    
    CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_ST;
        half _Cutoff;
        half4 _Color;
    CBUFFER_END
    
    struct Attributes
    {
        float4 positionOS : POSITION;
        float2 uv : TEXCOORD0;
        float4 color : COLOR;
        #if defined(SHADER_API_MOBILE)
        float3 normalOS : NORMAL;
        #endif
    };
    
    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 uv : TEXCOORD0;
    };
    
    TEXTURE2D(_MainTex);
    SAMPLER(sampler_MainTex);
    
    Varyings StandardDepthOnlyVertex(Attributes input)
    {
        Varyings output = (Varyings)0;
        output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
        output.uv = TRANSFORM_TEX(input.uv, _MainTex);
        return output;
    }
    
    half4 StandardDepthOnlyFragment(Varyings input) : SV_TARGET
    {
        return 0;
    }
    
    half4 StandardAlphaClipDepthOnlyFragment(Varyings input) : SV_TARGET
    {
        half alpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).a;
        clip(alpha - _Cutoff);
        return 0;
    }
    ENDHLSL
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }
            
            ZWrite On
            ColorMask 0
            Cull Back
            
            HLSLPROGRAM
            #pragma vertex StandardDepthOnlyVertex
            #pragma fragment StandardDepthOnlyFragment
            #pragma multi_compile_instancing
            ENDHLSL
        }
    }
    
    SubShader
    {
        Tags { "RenderType"="OpaqueDoubleSided" "RenderPipeline"="UniversalPipeline" }
        
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }
            
            ZWrite On
            ColorMask 0
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex StandardDepthOnlyVertex
            #pragma fragment StandardDepthOnlyFragment
            #pragma multi_compile_instancing
            ENDHLSL
        }
    }
    
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "RenderPipeline"="UniversalPipeline" }
        
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }
            
            ZWrite On
            ColorMask 0
            Cull Back
            
            HLSLPROGRAM
            #pragma vertex StandardDepthOnlyVertex
            #pragma fragment StandardAlphaClipDepthOnlyFragment
            #pragma multi_compile_instancing
            ENDHLSL
        }
    }
    
    // Tree 렌더링 관련 서브셰이더들
    SubShader
    {
        Tags { "RenderType"="TreeBark" "RenderPipeline"="UniversalPipeline" }
        
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }
            
            ZWrite On
            ColorMask 0
            Cull Back
            
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"
            
            #pragma vertex StandardDepthOnlyVertex
            #pragma fragment StandardDepthOnlyFragment
            #pragma multi_compile_instancing
            ENDHLSL
        }
    }
    
    SubShader
    {
        Tags { "RenderType"="TreeLeaf" "RenderPipeline"="UniversalPipeline" }
        
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }
            
            ZWrite On
            ColorMask 0
            Cull Back
            
            HLSLPROGRAM
            #pragma vertex StandardDepthOnlyVertex
            #pragma fragment StandardAlphaClipDepthOnlyFragment
            #pragma multi_compile_instancing
            ENDHLSL
        }
    }
    
    // 빌보드 및 잔디 관련 서브셰이더들
    SubShader
    {
        Tags { "RenderType"="TreeBillboard" "RenderPipeline"="UniversalPipeline" }
        
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }
            
            ZWrite On
            ColorMask 0
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex StandardDepthOnlyVertex
            #pragma fragment StandardAlphaClipDepthOnlyFragment
            #pragma multi_compile_instancing
            ENDHLSL
        }
    }
    
    SubShader
    {
        Tags { "RenderType"="Grass" "RenderPipeline"="UniversalPipeline" }
        
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }
            
            ZWrite On
            ColorMask 0
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex StandardDepthOnlyVertex
            #pragma fragment StandardAlphaClipDepthOnlyFragment
            #pragma multi_compile_instancing
            ENDHLSL
        }
    }
    
    SubShader
    {
        Tags { "RenderType"="GrassBillboard" "RenderPipeline"="UniversalPipeline" }
        
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode"="DepthOnly" }
            
            ZWrite On
            ColorMask 0
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex StandardDepthOnlyVertex
            #pragma fragment StandardAlphaClipDepthOnlyFragment
            #pragma multi_compile_instancing
            ENDHLSL
        }
    }
    
    Fallback "Hidden/Universal Render Pipeline/FallbackError"
}
