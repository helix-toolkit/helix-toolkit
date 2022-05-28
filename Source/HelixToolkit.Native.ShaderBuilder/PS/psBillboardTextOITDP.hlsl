#ifndef PSBILLBOARDTEXTOIT_HLSL
#define PSBILLBOARDTEXTOIT_HLSL
#define POINTLINE
#include "psOITDepthPeelingCommon.hlsl"
#include "psBillboardText.hlsl"

DDPOutputMRT billboardTextOIT(PSInputBT input)
{
    return depthPeelPS(input.p, main(input));
}
#endif