#ifndef PSBILLBOARDTEXTOITDP_HLSL
#define PSBILLBOARDTEXTOITDP_HLSL
#define POINTLINE
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"
#include"psBillboardText.hlsl"
#include"psCommon.hlsl"

PSOITOutput billboardTextOIT(PSInputBT input)
{
    float4 color = main(input);
    return calculateOIT(color, input.vEye.w, input.p.z);
}

#endif