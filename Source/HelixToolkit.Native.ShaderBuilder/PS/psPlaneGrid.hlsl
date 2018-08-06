#ifndef PSPlaneGrid_HLSL
#define PSPlaneGrid_HLSL

#define POINTLINE

#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"
#include"psCommon.hlsl"

float4 main(PSPlaneGridInput input) : SV_TARGET
{
    float grid = whengt(whenle(frac(input.uv.x / pfParams.x), pfParams.y) + whenle(frac(input.uv.y / pfParams.x), pfParams.y), 0);
    float4 color = (1 - grid) * float4(0, 0, 0, 0) + grid * float4(0.8, 0.8, 0.8, 1);
    return color;
}
#endif