#ifndef VSVOLUME_HLSL
#define VSVOLUME_HLSL
#define VOLUME
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

VolumePS_INPUT main(float3 input : SV_Position)
{
    VolumePS_INPUT output = (VolumePS_INPUT) 0;
    output.mPos = float4(input, 1);
    output.wp = mul(output.mPos * scaleFactor, mWorld);
    output.pos = mul(output.wp, mViewProjection);
    output.tex = output.pos.xy / output.pos.w;
    return output;
}
#endif