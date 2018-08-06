#ifndef PSPlaneGrid_HLSL
#define PSPlaneGrid_HLSL

#define PLANEGRID

#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"
#include"psCommon.hlsl"

float4 main(PSPlaneGridInput input) : SV_TARGET
{
    float grid = whengt(whenle(frac(input.uv.x / pfParams.x), pfParams.y) + whenle(frac(input.uv.y / pfParams.x), pfParams.y), 0);
    float4 gridColor = lerp(gColor, pColor, clamp(length(input.uv - vEyePos.xz) / (pfParams.z * pfParams.x * 100), 0, 1));
    float4 color = (1 - grid) * pColor + grid * gridColor;
    if (hasShadowMap)
    {
        float s = shadowStrength(input.sp);
        color.rgb *= s;
    }
    return color;
}
#endif