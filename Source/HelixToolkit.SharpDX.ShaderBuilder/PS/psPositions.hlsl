#ifndef PSPOSITION_HLSL
#define PSPOSITION_HLSL

#include"..\Common\DataStructs.hlsl"

//--------------------------------------------------------------------------------------
// Render Positions as Color
//--------------------------------------------------------------------------------------
float4 main(PSInput input) : SV_Target
{
    return float4(input.wp.xyz, 1);
}

#endif