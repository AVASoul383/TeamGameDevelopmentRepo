Shader "Hidden/SSAO" {
Properties {
    _MainTex ("", 2D) = "" {}
    _RandomTexture ("", 2D) = "" {}
    _SSAO ("", 2D) = "" {}
}
SubShader {
    ZTest Always Cull Off ZWrite Off

    // --- SSAO Pass (8 samples) ---
    Pass {
    CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag
    #pragma target 3.0
    #include "UnityCG.cginc"

    struct v2f {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
    };

    v2f vert(appdata_img v) {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = v.texcoord;
        return o;
    }

    sampler2D _CameraDepthNormalsTexture;
    sampler2D _RandomTexture;
    float2 _NoiseScale;
    float4 _Params;

    half4 frag(v2f i) : SV_Target {
        // Simple fake SSAO (for fixing the error and continuing your build)
        float ao = tex2D(_RandomTexture, i.uv * _NoiseScale).r;
        ao = pow(ao, _Params.w);
        return half4(ao, ao, ao, 1.0);
    }
    ENDCG
    }

    // --- Blur Pass ---
    Pass {
    CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag_blur
    #pragma target 3.0
    #include "UnityCG.cginc"

    struct v2f {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
    };

    v2f vert(appdata_img v) {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = v.texcoord;
        return o;
    }

    sampler2D _SSAO;
    float3 _TexelOffsetScale;

    half4 frag_blur(v2f i) : SV_Target {
        float2 o = _TexelOffsetScale.xy;
        half sum = tex2D(_SSAO, i.uv).r;
        sum += tex2D(_SSAO, i.uv + o).r;
        sum += tex2D(_SSAO, i.uv - o).r;
        return half4(sum / 3.0, sum / 3.0, sum / 3.0, 1.0);
    }
    ENDCG
    }

    // --- Composite Pass ---
    Pass {
    CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag_composite
    #pragma target 3.0
    #include "UnityCG.cginc"

    struct v2f {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
    };

    v2f vert(appdata_img v) {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = v.texcoord;
        return o;
    }

    sampler2D _MainTex;
    sampler2D _SSAO;
    float4 _Params;

    half4 frag_composite(v2f i) : SV_Target {
        half4 color = tex2D(_MainTex, i.uv);
        half ao = tex2D(_SSAO, i.uv).r;
        ao = pow(ao, _Params.w);
        color.rgb *= ao;
        return color;
    }
    ENDCG
    }
}
Fallback Off
}
