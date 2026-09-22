Shader "LittleSwitch/SoftWindowRay" {
Properties { _Color("Light tint",Color)=(1,0.86,0.6,0.06) }
SubShader { Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
Pass { Blend SrcAlpha One ZWrite Off Cull Off
HLSLPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
struct A {float4 positionOS:POSITION;float2 uv:TEXCOORD0;};
struct V {float4 positionCS:SV_POSITION;float2 uv:TEXCOORD0;};
CBUFFER_START(UnityPerMaterial)
half4 _Color;
CBUFFER_END
V vert(A i){V o;o.positionCS=TransformObjectToHClip(i.positionOS.xyz);o.uv=i.uv;return o;}
half4 frag(V i):SV_Target {float edge=pow(saturate(sin(i.uv.x*3.14159265)),2);float lengthFade=smoothstep(0,.12,i.uv.y)*(1-smoothstep(.45,1,i.uv.y));return half4(_Color.rgb,_Color.a*edge*lengthFade);}
ENDHLSL
} } }
