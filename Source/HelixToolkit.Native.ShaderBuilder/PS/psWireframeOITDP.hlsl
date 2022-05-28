#ifndef PSWIREFRAMEOITDP_HLSL
#define PSWIREFRAMEOITDP_HLSL

#define MESH
#include "psOITDepthPeelingCommon.hlsl"
#include "psWireframe.hlsl"

DDPOutputMRT wireframeOITDP(PSWireframeInput input)
{
    return depthPeelPS(input.p, main(input.p));
}
#endif