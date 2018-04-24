#ifndef PSWIREFRAMEOIT_HLSL
#define PSWIREFRAMEOIT_HLSL

#define MESH
#include"..\Common\CommonBuffers.hlsl"
#include"psCommon.hlsl"

PSOITOutput wireframeOIT(float4 pos : SV_POSITION)
{
    return calculateOIT(wireframeColor, pos);
}
#endif