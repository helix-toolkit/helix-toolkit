#ifndef PSVIEWBOX_HLSL
#define PSVIEWBOX_HLSL

#define MESH
#include"..\Common\DataStructs.hlsl"
#include"..\Common\CommonBuffers.hlsl"
#include"psCommon.hlsl"
//--------------------------------------------------------------------------------------
//  Render coordinate system
//--------------------------------------------------------------------------------------
float4 main(PSInput input) : SV_Target
{
    float4 I = input.cDiffuse;
    float4 vMaterialTexture = 1.0f;
    //float3 eye = normalize(vEyePos - input.wp.xyz);
    if (bHasDiffuseMap)
    {
	    // SamplerState is defined in Common.fx.
        vMaterialTexture *= texDiffuseMap.Sample(samplerSurface, input.t);
    }
    //float3 d = normalize(mView._m02_m12_m22); // fixed look dir	as light dir
    //float3 h = normalize(vEyePos + d);
    //const float4 lightColor = float4(1, 1, 1, 1);
    //I += calcBlinnPhongLighting(lightColor, vMaterialTexture, input.n, input.cDiffuse, d, h);
    I *= vMaterialTexture;
    I = I * input.c;
    return I;
}
#endif