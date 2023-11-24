#ifndef PSWIREFRAMEOIT_HLSL
#define PSWIREFRAMEOIT_HLSL

#define MESH
#include "..\Common\CommonBuffers.hlsl"
#include "psCommon.hlsl"

PSOITOutput wireframeOIT(PSWireframeInput input)
{
    return calculateOIT(wireframeColor, input.z, input.p.z);
}
#endif