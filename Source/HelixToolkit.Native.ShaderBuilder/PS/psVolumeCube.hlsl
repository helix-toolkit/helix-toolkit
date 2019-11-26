#ifndef PSVOLUMECUBE_HLSL
#define PSVOLUMECUBE_HLSL

#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

float4 main(VolumePS_INPUT input) : SV_Target
{
    return input.wp;
}
#endif