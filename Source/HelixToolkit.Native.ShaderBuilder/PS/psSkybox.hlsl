#ifndef PSSKYBOX_HLSL
#define PSSKYBOX_HLSL
#include"..\Common\DataStructs.hlsl"
#include"..\Common\CommonBuffers.hlsl"
#include"psCommon.hlsl"
//--------------------------------------------------------------------------------------
//  Render coordinate system
//--------------------------------------------------------------------------------------
float4 main(PSInputCube input) : SV_Target
{
    //return float4(0, 0, 1, 1);
    return float4(texCubeMap.SampleLevel(samplerCube, input.t, 0), 1);

}
#endif