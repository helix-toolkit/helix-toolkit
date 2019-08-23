#ifndef PSMESHDIFFUSEMAPOIT_HLSL
#define PSMESHDIFFUSEMAPOIT_HLSL

#define CLIPPLANE
#define MESH

#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"
#include"psCommon.hlsl"
#include"psDiffuseMap.hlsl"
//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING - BLINN-PHONG for Order Independant Transparent A-buffer rendering
// http://casual-effects.blogspot.com/2014/03/weighted-blended-order-independent.html
//--------------------------------------------------------------------------------------

PSOITOutput meshDiffuseOIT(PSInput input)
{
    float4 color = main(input);
    return calculateOIT(color, input.vEye.w, input.p.z);
}
#endif