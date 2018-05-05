#ifndef PSPARTICLE_HLSL
#define PSPARTICLE_HLSL
#define PARTICLE
#define POINTLINE
#include"..\Common\CommonBuffers.hlsl"
#include"..\Common\DataStructs.hlsl"


float4 main(in ParticlePS_INPUT input) : SV_Target
{
    float4 color = input.color * input.opacity;
    if (pbParams.x)
    {
        color *= texParticle.Sample(samplerParticle, input.texcoords);
    }
    return color;
}

#endif