Shader "Custom/TransparentZone"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100
        
        ZWrite Off
        ZTest Always
        ColorMask 0
        
        // 스텐실 값 2로 마킹
        Stencil
        {
            Ref 2
            Comp Always
            Pass Replace
        }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(0,0,0,0); // 완전 투명
            }
            ENDCG
        }
    }
}