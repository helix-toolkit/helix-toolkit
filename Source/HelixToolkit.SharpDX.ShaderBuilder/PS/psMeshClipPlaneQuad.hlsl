#ifndef PSSCREENQUAD
#define PSSCREENQUAD
#define CLIPPLANE
#include"..\Common\DataStructs.hlsl"
#include"..\Common\CommonBuffers.hlsl"

float4 main(float4 input : SV_POSITION) : SV_Target
{
    //return CrossSectionColors;
    return float4(0, 0, 1, 1);

}

#endif