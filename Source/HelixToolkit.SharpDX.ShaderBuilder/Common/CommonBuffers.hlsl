#ifndef CBUFFERS_HLSL
#define CBUFFERS_HLSL

#pragma pack_matrix( row_major )

//--------------------------------------------------------------------------------------
// Perframe Buffers
//--------------------------------------------------------------------------------------
cbuffer cbTransforms
{
    float4x4 mView;
    float4x4 mProjection;
    float4x4 mViewProjection;
	// camera frustum: 
	// [fov,asepct-ratio,near,far]
    float4 vFrustum;
	// viewport:
	// [w,h,0,0]
    float4 vViewport;
	// camera position
    float3 vEyePos;
    float padding0;
};

//Per model
cbuffer cbModel
{
    float4x4 mWorld;
    bool bInvertNormal = false;
    bool bHasInstances = false;
    bool bHasInstanceParams = false;
    bool bHasBones = false;
    float4 vParams = float4(0, 0, 0, 0); //Shared with line, points and billboard
    float4 vColor = float4(1, 1, 1, 1); //Shared with line, points and billboard
};



#ifdef MATERIAL
#define LIGHTS 8
//--------------------------------------------------------------------------------------
// Light Buffer
//--------------------------------------------------------------------------------------
struct LightStruct
{
    int iLightType; //4
    bool bLightEnable;
    float2 paddingL;
	// the light direction is here the vector which looks towards the light
    float4 vLightDir; //8
    float4 vLightPos; //12
    float4 vLightAtt; //16
    float4 vLightSpot; //(outer angle , inner angle, falloff, free), 20
    float4 vLightColor; //24
    matrix mLightView; //40
    matrix mLightProj; //56
};

cbuffer cbLights
{
    LightStruct Lights[LIGHTS];
    float4 vLightAmbient = float4(0.2f, 0.2f, 0.2f, 1.0f);
};

// File: Materials header for HelixToolkit.Wpf.SharpDX
// Author: Przemyslaw Musialski
// Date: 11/11/12
// References & Sources: 
//--------------------------------------------------------------------------------------
// CONSTANT BUFF FOR MATERIAL
//--------------------------------------------------------------------------------------
cbuffer cbMaterial
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

Texture2D texDiffuseMap;
Texture2D texAlphaMap;
Texture2D texNormalMap;
Texture2D texDisplacementMap);
TextureCube texCubeMap;
Texture2D texShadowMap;

cbuffer cbShadow
{
    float2 vShadowMapSize = float2(1024, 1024);
    float2 paddingShadow0;
    float4 vShadowMapInfo = float4(0.005, 1.0, 0.5, 0.0);
};
#endif

#ifdef CLIPPLANE
cbuffer cbClipping
{
	bool4 EnableCrossPlane;
    float4 CrossSectionColors;
	// Format:
	// M00M01M02 PlaneNormal1 M03 Plane1 Distance to origin
	// M10M11M12 PlaneNormal2 M13 Plane2 Distance to origin
	// M20M21M22 PlaneNormal3 M23 Plane3 Distance to origin
	// M30M31M32 PlaneNormal4 M33 Plane4 Distance to origin
	float4x4 CrossPlaneParams;
}
#endif

#endif