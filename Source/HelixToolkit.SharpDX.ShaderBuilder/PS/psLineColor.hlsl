#ifndef PSLINE_HLSL
#define PSLINE_HLSL

#include"..\Common\DataStructs.hlsl"

float4 main(PSInputPS input) : SV_Target
{
    return input.c;
}

#endif