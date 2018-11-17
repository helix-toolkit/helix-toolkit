#ifndef SSAOEFFECTS
#define SSAOEFFECTS
#define SSAO
#include"..\Common\CommonBuffers.hlsl"
#pragma pack_matrix( row_major )

float4 main(MeshOutlinePS_INPUT input) : SV_Target
{
    float depth = texSSAOMap.Sample(samplerSurface, input.Tex);

    float4 output = (float4) 0;

    return output;
}
#endif