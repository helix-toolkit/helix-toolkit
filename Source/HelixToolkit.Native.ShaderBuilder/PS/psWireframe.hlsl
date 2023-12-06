#ifndef PSWIREFRAME_HLSL
#define PSWIREFRAME_HLSL

#define MESH
#include "..\Common\CommonBuffers.hlsl"

float4 main(float4 pos : SV_POSITION) : SV_Target
{
    return wireframeColor;
}
#endif