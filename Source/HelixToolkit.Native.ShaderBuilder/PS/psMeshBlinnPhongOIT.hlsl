#ifndef PSMESHBLINNPHONGOIT_HLSL
#define PSMESHBLINNPHONGOIT_HLSL

#define CLIPPLANE
#define MESH

#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"
#include"psCommon.hlsl"
#include"psMeshBlinnPhong.hlsl"
//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING - BLINN-PHONG for Order Independant Transparent A-buffer rendering
// http://casual-effects.blogspot.com/2014/03/weighted-blended-order-independent.html
//--------------------------------------------------------------------------------------

PSOITOutput meshBlinnPhongOIT(PSInput input)
{    
    float4 color = main(input);
    return calculateOIT(color, input.vEye.w, input.p.z);
}
#endif