#ifndef PSPlaneGrid_HLSL
#define PSPlaneGrid_HLSL

#define PLANEGRID

#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"
#include"psCommon.hlsl"

float4 main(PSPlaneGridInput input) : SV_TARGET
{
    float grid = 0;
    if (type == 0)
    {
        float g = floor(input.uv.x / gridSpacing) + floor(input.uv.y / gridSpacing);
        g = (uint) abs(g) % 2;
        grid = whengt(g, 0);
    }
    else
    {
        grid = whengt(whenle(frac(input.uv.x / gridSpacing), gridThickness) + whenle(frac(input.uv.y / gridSpacing), gridThickness), 0);
    }
    float dist = length(input.wp - vEyePos.xyz);
    float4 color = (1 - grid) * pColor + grid * gColor;
    color.a = lerp(color.a, 0, clamp(abs(vFrustum.w * fadingFactor) / max(vFrustum.w - dist, 0), 0, 1));

    if (hasShadowMap)
    {
        float s = shadowStrength(input.sp);
        color.rgb *= s;
    }
    return color;
}
#endif