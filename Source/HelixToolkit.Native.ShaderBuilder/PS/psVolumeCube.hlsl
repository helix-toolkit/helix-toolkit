#ifndef PSVOLUMECUBE_HLSL
#define PSVOLUMECUBE_HLSL

#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

float4 main(VolumePS_INPUT input) : SV_Target
{
    return input.mPos + float4(0.5, 0.5, 0.5, 0);
}
#endif