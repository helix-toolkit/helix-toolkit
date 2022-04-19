#ifndef PSMESHPBROITDP_HLSL
#define PSMESHPBROITDP_HLSL

#define MESH
#define PBR

#include "psOITDepthPeelingCommon.hlsl"
#include"psMeshPBR.hlsl"

DDPOutputMRT pbrOITDP(PSInput input)
{
    return depthPeelPS(input.p, main(input));
}
#endif