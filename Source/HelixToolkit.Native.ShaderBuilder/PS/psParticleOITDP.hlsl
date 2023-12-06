#ifndef PSPARTICLEOITDP_HLSL
#define PSPARTICLEOITDP_HLSL
#define PARTICLE
#include "psOITDepthPeelingCommon.hlsl"
#include "psParticle.hlsl"

DDPOutputMRT particleOITDP(ParticlePS_INPUT input)
{
    return depthPeelPS(input.position, main(input));
}
#endif