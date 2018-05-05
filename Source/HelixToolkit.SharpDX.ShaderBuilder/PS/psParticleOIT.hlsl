#ifndef PSPARTICLEOIT_HLSL
#define PSPARTICLEOIT_HLSL
#define PARTICLE
#define POINTLINE
#include"..\Common\CommonBuffers.hlsl"
#include"..\Common\DataStructs.hlsl"
#include"psParticle.hlsl"
#include"psCommon.hlsl"


PSOITOutput particleOIT(in ParticlePS_INPUT input)
{
    float4 color = main(input);
    return calculateOIT(color, input.z, input.position.z);
}

#endif