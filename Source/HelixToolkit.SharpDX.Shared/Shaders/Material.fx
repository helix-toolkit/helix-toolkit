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
cbuffer cbMaterial : register(b2)
{
	float4 vMaterialAmbient		= 0.25f;  //Ka := surface material's ambient coefficient
	float4 vMaterialDiffuse		= 0.5f;   //Kd := surface material's diffuse coefficient
	float4 vMaterialEmissive	= 0.0f;   //Ke := surface material's emissive coefficient
	float4 vMaterialSpecular	= 0.0f;   //Ks := surface material's specular coefficient
	float4 vMaterialReflect 	= 0.0f;   //Kr := surface material's reflectivity coefficient
	float  sMaterialShininess	= 1.0f;	  //Ps := surface material's shininess
	
    bool   bHasDiffuseMap       = false;
	bool   bHasAlphaMap			= false;	
	bool   bHasNormalMap		= false;
	bool   bHasDisplacementMap  = false;	
    bool bHasShadowMap = false;
    float2 padding;
};

bool bHasCubeMap = false;

float2 vShadowMapSize		= float2(1024, 1024);
float4 vShadowMapInfo		= float4(0.005, 1.0, 0.5, 0.0);


		
//--------------------------------------------------------------------------------------
// GLOBAL Variables (Varing)
//--------------------------------------------------------------------------------------


//--------------------------------------------------------------------------------------
// TEXTURES
//--------------------------------------------------------------------------------------

Texture2D texDiffuseMap : register(t0);
Texture2D texAlphaMap : register(t1);
Texture2D texNormalMap : register(t2);
Texture2D texDisplacementMap : register(t3);
Texture2D texShadowMap : register(t4);
TextureCube texCubeMap : register(t5);

#endif