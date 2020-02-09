#ifndef VSVOLUME_HLSL
#define VSVOLUME_HLSL
#define VOLUME
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

VolumePS_INPUT main(float3 input : SV_Position)
{
    VolumePS_INPUT output = (VolumePS_INPUT) 0;
    output.wp = mul(float4(input, 1), mWorld);
    output.pos = mul(output.wp, mViewProjection);
    return output;
}
#endif