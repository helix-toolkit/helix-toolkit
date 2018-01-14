#ifndef PSNORMAL_HLSL
#define PSNORMAL_HLSL
#include"..\Common\DataStructs.hlsl"

//--------------------------------------------------------------------------------------
//  Render Normals as Color
//--------------------------------------------------------------------------------------
float4 main(PSInput input) : SV_Target
{
    return float4(input.n * 0.5 + 0.5, 1);
}
#endif