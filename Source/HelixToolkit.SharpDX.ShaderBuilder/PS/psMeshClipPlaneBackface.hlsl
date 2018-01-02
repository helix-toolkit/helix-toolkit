#ifndef PSCLIPPLANEBACKFACE_HLSL
#define PSCLIPPLANEBACKFACE_HLSL

#include"psMeshClipPlane.hlsl"

float4 psClipPlaneBackFace(PSInput input) : SV_Target
{
    DetermineCutPlane(input.wp.xyz);
    return float4(0, 0, 0, 0);
}

#endif