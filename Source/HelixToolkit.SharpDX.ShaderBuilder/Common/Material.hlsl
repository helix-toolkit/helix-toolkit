#ifndef MATERIAL_FX
#define MATERIAL_FX

//--------------------------------------------------------------------------------------
// File: Materials header for HelixToolkit.Wpf.SharpDX
// Author: Przemyslaw Musialski
// Date: 11/11/12
// References & Sources: 
//--------------------------------------------------------------------------------------

//--------------------------------------------------------------------------------------
// CONSTANT BUFF VARIABLES
//--------------------------------------------------------------------------------------
cbuffer cbMaterial : register(b3)
{
    float4 vMaterialAmbient = 0.25f; //Ka := surface material's ambient coefficient
    float4 vMaterialDiffuse = 0.5f; //Kd := surface material's diffuse coefficient
    float4 vMaterialEmissive = 0.0f; //Ke := surface material's emissive coefficient
    float4 vMaterialSpecular = 0.0f; //Ks := surface material's specular coefficient
    float4 vMaterialReflect = 0.0f; //Kr := surface material's reflectivity coefficient
    float sMaterialShininess = 1.0f; //Ps := surface material's shininess
	
    bool bHasDiffuseMap = false;
    bool bHasAlphaMap = false;
    bool bHasNormalMap = false;
    bool bHasDisplacementMap = false;
    bool bHasShadowMap = false;
    bool bHasCubeMap = false;
    float paddingMaterial0;
};

cbuffer cbShadow : register(b4)
{
    float2 vShadowMapSize = float2(1024, 1024);
    float2 paddingShadow0;
    float4 vShadowMapInfo = float4(0.005, 1.0, 0.5, 0.0);
};


//--------------------------------------------------------------------------------------
// MATERIAL TEXTURES
//--------------------------------------------------------------------------------------

Texture2D texDiffuseMap : register(t0);
Texture2D texAlphaMap : register(t1);
Texture2D texNormalMap : register(t2);
Texture2D texDisplacementMap : register(t3);
Texture2D texCubeMap : register(t4);
TextureCube texShadowMap : register(t5);
//--------------------------------------------------------------------------------------
//  STATES DEFININITIONS 
//--------------------------------------------------------------------------------------
SamplerState LinearSampler
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
    MaxAnisotropy = 16;
};
SamplerState NormalSampler
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};
SamplerState PointSampler
{
    Filter = MIN_MAG_MIP_POINT;
    AddressU = Wrap;
    AddressV = Wrap;
};
SamplerComparisonState CmpSampler
{
   // sampler state
    Filter = COMPARISON_MIN_MAG_MIP_LINEAR;
    AddressU = MIRROR;
    AddressV = MIRROR;
   // sampler comparison state
    ComparisonFunc = LESS_EQUAL;
};
#endif