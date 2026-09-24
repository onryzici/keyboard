Shader "LittleSwitch/DriftingDust" {
Properties { _Color("Tint",Color)=(1,.86,.6,1) }
SubShader {Tags {"RenderPipeline"="HDRenderPipeline" "Queue"="Transparent" "RenderType"="Transparent"}
Pass {Tags {"LightMode"="ForwardOnly"} Blend SrcAlpha One ZWrite Off Cull Off
HLSLPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 4.5
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
struct A {float4 positionOS:POSITION;float2 uv:TEXCOORD0;half4 color:COLOR;};struct V {float4 positionCS:SV_POSITION;float2 uv:TEXCOORD0;half4 color:COLOR;};
CBUFFER_START(UnityPerMaterial)
half4 _Color;
CBUFFER_END
V vert(A i){V o;o.positionCS=TransformObjectToHClip(i.positionOS.xyz);o.uv=i.uv;o.color=i.color*_Color;return o;}
half4 frag(V i):SV_Target{float d=length(i.uv-.5)*2;return half4(i.color.rgb,i.color.a*pow(saturate(1-d),2));}
ENDHLSL
}}}
