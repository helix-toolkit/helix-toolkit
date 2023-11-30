#ifndef PSMESHBLINNPHONGOITDEPTHPEEL_HLSL
#define PSMESHBLINNPHONGOITDEPTHPEEL_HLSL
#define CLIPPLANE
#define MESH

#include "psOITDepthPeelingCommon.hlsl"
#include "psMeshBlinnPhong.hlsl"
DDPOutputMRT blinnPhongOITDP(PSInput input)
{
    return depthPeelPS(input.p, main(input));
}
#endif