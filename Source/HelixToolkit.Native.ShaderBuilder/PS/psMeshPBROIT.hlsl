#ifndef PSMESHPBROIT_HLSL
#define PSMESHPBROIT_HLSL

#define MESH
#define PBR
#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"
#include"psCommon.hlsl"
#include"psMeshPBR.hlsl"
//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING - BLINN-PHONG for Order Independant Transparent A-buffer rendering
// http://casual-effects.blogspot.com/2014/03/weighted-blended-order-independent.html
//--------------------------------------------------------------------------------------

PSOITOutput meshPBROIT(PSInput input)
{
    float4 color = main(input);
    return calculateOIT(color, input.vEye.w, input.p.z);
}
#endif