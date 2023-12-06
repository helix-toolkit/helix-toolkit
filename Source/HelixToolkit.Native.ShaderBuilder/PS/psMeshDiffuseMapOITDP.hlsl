#ifndef PSMESHDIFFUSEMAPOITDP_HLSL
#define PSMESHDIFFUSEMAPOITDP_HLSL

#define CLIPPLANE
#define MESH


#include "psOITDepthPeelingCommon.hlsl"
#include "psDiffuseMap.hlsl"

DDPOutputMRT diffuseOITDP(PSInput input)
{
    return depthPeelPS(input.p, main(input));
}
#endif