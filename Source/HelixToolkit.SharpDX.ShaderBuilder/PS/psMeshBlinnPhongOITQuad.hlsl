#ifndef PSMESHOITQUAD
#define PSMESHOITQUAD

#include"..\Common\DataStructs.hlsl"
#include"..\Common\CommonBuffers.hlsl"

float4 main(MeshOutlinePS_INPUT input) : SV_Target
{
    float4 accum = texOITColor.Sample(samplerDiffuse, input.Tex.xy);
    float reveal = texOITAlpha.Sample(samplerDiffuse, input.Tex.xy).r;
    return float4(accum.rgb / clamp(accum.a, 1e04, 5e4), reveal);

}

#endif