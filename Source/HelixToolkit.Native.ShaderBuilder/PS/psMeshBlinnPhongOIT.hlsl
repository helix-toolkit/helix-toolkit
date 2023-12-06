#ifndef PSMESHBLINNPHONGOIT_HLSL
#define PSMESHBLINNPHONGOIT_HLSL

#define CLIPPLANE
#define MESH

#include "..\Common\Common.hlsl"
#include "..\Common\DataStructs.hlsl"
#include "psCommon.hlsl"
#include "psMeshBlinnPhong.hlsl"
//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING - BLINN-PHONG for Order Independant Transparent A-buffer rendering
// http://casual-effects.blogspot.com/2014/03/weighted-blended-order-independent.html
//--------------------------------------------------------------------------------------

PSOITOutput meshBlinnPhongOIT(PSInput input)
{    
    float4 color = main(input);
    return calculateOIT(color, input.vEye.w, input.p.z);
}

//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING - BLINN-PHONG for Order Independant Transparent from Microsoft DX11 SDK
//--------------------------------------------------------------------------------------
RWTexture2D<uint> fragmentCount : register(u1);
RWBuffer<float> deepBufferDepth : register(u2);
RWBuffer<uint4> deepBufferColor : register(u3);
RWBuffer<uint> prefixSum : register(u4);

void FragmentCountPS(PSInput input)
{
    // Increments need to be done atomically
    InterlockedAdd(fragmentCount[input.p.xy], 1);
}

void meshBlinnPhongOITSort(PSInput input)
{
    float4 color = main(input);
    
    uint x = input.p.x;
    uint y = input.p.y;

    // Atomically allocate space in the deep buffer
    uint fc;
    InterlockedAdd(fragmentCount[input.p.xy], 1, fc);

    uint nPrefixSumPos = y * vViewport.x + x;
    uint nDeepBufferPos;
    if (nPrefixSumPos == 0)
        nDeepBufferPos = fc;
    else
        nDeepBufferPos = prefixSum[nPrefixSumPos - 1] + fc;

    // Store fragment data into the allocated space
    deepBufferDepth[nDeepBufferPos] = input.p.z;
    deepBufferColor[nDeepBufferPos] = clamp(color, 0, 1) * 255;
}
#endif