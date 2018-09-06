#ifndef PSMESHOITQUAD
#define PSMESHOITQUAD

#include"..\Common\DataStructs.hlsl"
#include"..\Common\CommonBuffers.hlsl"

float4 main(MeshOutlinePS_INPUT input) : SV_Target
{
    float4 accum = texOITColor.Sample(samplerSurface, input.Tex.xy);
    float reveal = texOITAlpha.Sample(samplerSurface, input.Tex.xy).a;
    return float4(accum.rgb / max(accum.a, 1e-5), reveal);
}

#endif