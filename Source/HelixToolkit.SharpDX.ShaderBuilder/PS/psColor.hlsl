#include"..\Common\DataStructs.hlsl"

//--------------------------------------------------------------------------------------
// Given Per-Vertex Color
//--------------------------------------------------------------------------------------
float4 main(PSInput input) : SV_Target
{
    return input.c;
}