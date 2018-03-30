#ifndef PSMESHDEFAULT_HLSL
#define PSMESHDEFAULT_HLSL

#define MESH
#include"..\Common\CommonBuffers.hlsl"

float4 main(float4 pos : SV_POSITION) : SV_Target
{
    return wireframeColor;
}
#endif