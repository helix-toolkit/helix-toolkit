#ifndef PSSCREENQUAD
#define PSSCREENQUAD
#include"..\Common\DataStructs.hlsl"

float4 psScreenQuad(PSInputScreenQuad input) : SV_Target
{
    return input.c;
}

#endif