#ifndef PSPARTICLE_HLSL
#define PSPARTICLE_HLSL
#define PARTICLE

#include"..\Common\CommonBuffers.hlsl"
#include"..\Common\DataStructs.hlsl"
#include"..\Common\CommonSamplers.hlsl"


float4 main(in ParticlePS_INPUT input) : SV_Target
{
    float4 color = input.color * input.opacity;
    if (bParams.x)
    {
        color *= texDiffuseMap.Sample(LinearSampler, input.texcoords);
    }
    return color;
}

#endif