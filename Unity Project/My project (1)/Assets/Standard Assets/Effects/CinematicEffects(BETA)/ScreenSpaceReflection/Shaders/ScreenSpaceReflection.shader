Shader "Hidden/ScreenSpaceReflection"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "white" {}
    }

    CGINCLUDE
    #include "UnityCG.cginc"

    // Manual Declarations
    sampler2D _MainTex;
    float4 _MainTex_TexelSize;

    // Include all your variables you use (minimal for now)
    
    struct v2f {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
        float2 uv2 : TEXCOORD1;
    };

    v2f vert(appdata_img v)
    {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = v.texcoord.xy;
        o.uv2 = v.texcoord.xy;

        #if UNITY_UV_STARTS_AT_TOP
        if (_MainTex_TexelSize.y < 0)
            o.uv2.y = 1.0 - o.uv2.y;
        #endif

        return o;
    }

    half4 fragBlit(v2f i) : SV_Target
    {
        return tex2D(_MainTex, i.uv);
    }

    half4 fragComposite(v2f i) : SV_Target
    {
        return tex2D(_MainTex, i.uv);
    }

    half4 fragRaytrace(v2f i) : SV_Target
    {
        return tex2D(_MainTex, i.uv);
    }

    half4 fragGBlur(v2f i) : SV_Target
    {
        return tex2D(_MainTex, i.uv);
    }

    half4 fragCompositeSSR(v2f i) : SV_Target
    {
        return tex2D(_MainTex, i.uv);
    }

    half4 fragEdge(v2f i) : SV_Target
    {
        return tex2D(_MainTex, i.uv);
    }

    half4 fragMin(v2f i) : SV_Target
    {
        return tex2D(_MainTex, i.uv);
    }

    ENDCG

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZTest Always Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragRaytrace
            #pragma target 3.0
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragComposite
            #pragma target 3.0
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragGBlur
            #pragma target 3.0
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragCompositeSSR
            #pragma target 3.0
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragBlit
            #pragma target 3.0
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragEdge
            #pragma target 3.0
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragMin
            #pragma target 3.0
            ENDCG
        }
    }

    Fallback Off
}
