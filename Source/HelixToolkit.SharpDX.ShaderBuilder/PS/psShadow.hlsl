#ifndef PSSHADOW_HLSL
#define PSSHADOW_HLSL
#include"..\Common\DataStructs.hlsl"
//--------------------------------------------------------------------------------------
// Given Per-Vertex Color
//--------------------------------------------------------------------------------------
#ifdef DEBUG
float4 main(PSShadow input) : SV_Target
{
    return float4(input.p.z, 0, 0, 1);
}
#else
void main(PSShadow input)
{
    
}
#endif
#endif