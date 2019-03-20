#ifndef CBUFFERS_HLSL
#define CBUFFERS_HLSL
#pragma pack_matrix( row_major )
#include"DataStructs.hlsl"


///------------------Constant Buffers-----------------------
//--------------------------------------------------------------------------------------
// Perframe Buffers
//--------------------------------------------------------------------------------------
cbuffer cbTransforms : register(b0)
{
    float4x4 mView;
    float4x4 mProjection;
    float4x4 mViewProjection;
	// camera frustum: 
	// [fov,asepct-ratio,near,far]
    float4 vFrustum;
	// viewport:
	// [w,h,1/w,1/h]
    float4 vViewport;
	// camera position
    float3 vEyePos;
    bool SSAOEnabled;
    float SSAOBias;
    float SSAOIntensity;
    float TimeStamp;
    float padding0;
    float OITPower;
    float OITSlope;
    int OITWeightMode;
    int OITReserved;
};

#if defined(MESHSIMPLE)
cbuffer cbMeshSimple : register(b1)
{
// Common Parameters
    float4x4 mWorld;
    bool bHasInstances = false;
    float3 padding1;
};
#endif

#if defined(MESH)
//Per model shares between Phong material and PBR material
cbuffer cbMesh : register(b1) 
{
// Common Parameters
    float4x4 mWorld;
    bool bInvertNormal = false;
    bool bHasInstances = false;
    bool bHasInstanceParams = false;
    bool bHasBones = false;
    float4 vParams = float4(0, 0, 0, 0); //Shared with models
    float4 vColor = float4(1, 1, 1, 1); //Shared with models
    float4 wireframeColor;
    bool3 bParams; // Shared with models for enable/disable features
    bool bBatched = false;

// Material Parameters changable
	float minTessDistance = 1;
	float maxTessDistance = 100;
	float minTessFactor = 4;
	float maxTessFactor = 1;

    float4 vMaterialDiffuse = 0.5f; //Kd := surface material's diffuse coefficient
    float4 vMaterialAmbient = 0.25f; //Ka := surface material's ambient coefficient.
    float4 vMaterialEmissive = 0.0f; //Ke := surface material's emissive coefficient
#if !defined(PBR)
    float4 vMaterialSpecular = 0.0f; //Ks := surface material's specular coefficient. If using PBR, vMaterialReflect = float4(ConstantAO, ConstantRoughness, ConstantMetallic, ConstantReflectance);
    float4 vMaterialReflect = 0.0f; //Kr := surface material's reflectivity coefficient. If using PBR, vMaterialSpecular = float4(ClearCoat, ClearCoatRoughness, 0, 0)
#endif
#if defined(PBR)
    float ConstantAO;
    float ConstantRoughness;
    float ConstantMetallic;
    float ConstantReflectance;
    float ClearCoat;
    float ClearCoatRoughness;
    float padding1;
    bool bHasAOMap;
#endif
    bool bHasDiffuseMap = false;
    bool bHasNormalMap = false;
    bool bHasCubeMap = false;
    bool bRenderShadowMap = false;
    bool bHasEmissiveMap = false;
#if !defined(PBR)
    bool bHasAlphaMap = false; // If using PBR, this is used as HasRMAMap.
    bool bHasSpecularMap;    
#endif
#if defined(PBR)
    bool bHasRMMap;    
    bool bHasIrradianceMap; 
#endif
    bool bAutoTengent;
    bool bHasDisplacementMap = false;
    bool bRenderPBR = false;  
    float padding12;
    float sMaterialShininess = 1.0f; //Ps := surface material's shininess

    float4 displacementMapScaleMask = float4(0, 0, 0, 1);
    float4 uvTransformR1;
    float4 uvTransformR2;
};
#endif

#if defined(SCREENDUPLICATION)
    cbuffer cbScreenClone : register(b9)
    {
        float4 VertCoord[4];
        float4 TextureCoord[4];
        float4 CursorVertCoord[4];
    };
#endif
#if defined(SCREENQUAD)
    cbuffer cbScreenQuad : register(b9)
    {
        float4x4 mWorld;
        float4 VertCoord[4];
        float4 TextureCoord[4];
    };
#endif
cbuffer cbLights : register(b3)
{
    LightStruct Lights[LIGHTS];
    float4 vLightAmbient = float4(0.2f, 0.2f, 0.2f, 1.0f);
    int NumLights;
    bool bHasEnvironmentMap;
    int NumEnvironmentMapMipLevels;
    float padding;
};

#if defined(POINTLINE) // model for line, point and billboard
//Per model
cbuffer cbPointLineModel : register(b4)
{
    float4x4 mWorld;
    bool bHasInstances = false;
    bool bHasInstanceParams = false;
	float2 padding1;
    float4 pfParams = float4(0, 0, 0, 0); //Shared with line, points and billboard
    float4 pColor = float4(1, 1, 1, 1); //Shared with line, points and billboard
    bool fixedSize;
	bool3 pbParams;
    bool enableDistanceFading;
    float fadeNearDistance;
    float fadeFarDistance;
    float padding2;
};
#endif
#if defined(VOLUME) // model for line, point and billboard
//Per model
cbuffer cbVolumeModel : register(b4)
{
    float4x4 mWorld;
    float4 pColor;
    float stepSize;
    uint iterationOffset;
    float padding1;
    uint maxIterations;
    bool bHasGradientMapX;
    float isoValue;
    float baseSampleDist = .5f;
    float actualSampleDist = .5f;
    float4 scaleFactor;
};
#endif
#if defined(PARTICLE) // model for line, point and billboard
//Per model
cbuffer cbParticleModel : register(b4)
{
    float4x4 mWorld;
    bool bHasInstances = false;
    bool bHasInstanceParams = false;
    bool bHasTexture = false;
	float padding1;
};
#endif
#if defined(PLANEGRID) 
cbuffer cbPlaneGridModel : register(b4)
{
    float4x4 mWorld;
    float gridSpacing; 
    float gridThickness;
    float fadingFactor;
    float planeD;
    float4 pColor;
    float4 gColor;
    bool hasShadowMap;
    int axis;
    int type;
    float padding3;
};
#endif
cbuffer cbShadow : register(b5)
{
    float2 vShadowMapSize = float2(1024, 1024);
    bool bHasShadowMap = false;
    float paddingShadow0;
    float4 vShadowMapInfo = float4(0.005, 1.0, 0.5, 0.0);
    float4x4 vLightViewProjection;
};
#if defined(CLIPPLANE)
cbuffer cbClipping : register(b6)
{
    bool4 EnableCrossPlane;
    float4 CrossSectionColors;
    int CuttingOperation;
    float3 paddingClipping;
	// Format:
	// M00M01M02 PlaneNormal1 M03 Plane1 Distance to origin
	// M10M11M12 PlaneNormal2 M13 Plane2 Distance to origin
	// M20M21M22 PlaneNormal3 M23 Plane3 Distance to origin
	// M30M31M32 PlaneNormal4 M33 Plane4 Distance to origin
    float4 CrossPlane1Params;
    float4 CrossPlane2Params;
    float4 CrossPlane3Params;
    float4 CrossPlane4Params;
}
#endif

#if defined(BORDEREFFECTS)

cbuffer cbBorderEffect : register(b6)
{
    float4 Color;
    float4x4 Param;
    float viewportScale; // Used to handle if using lower resolution render target for bluring. Scale = Full Res / Low Res;
    float3 padding9;
};
#endif

#if defined(PARTICLE)
cbuffer cbParticleFrame : register(b7)
{
    uint NumParticles;
    float3 ExtraAccelation;

    float TimeFactors;
    float3 DomainBoundsMax;

    float3 DomainBoundsMin;
    uint CumulateAtBound;

    float3 ConsumerLocation;
    float ConsumerGravity;

    float ConsumerRadius;
    float3 RandomVector;

    uint RandomSeed;
    uint NumTexCol;
    uint NumTexRow;
    bool AnimateByEnergyLevel;
    float2 ParticleSize;
    float Turbulance;
    float pad0;
};

cbuffer cbParticleCreateParameters : register(b8)
{
    float3 EmitterLocation;
    float InitialEnergy;

    float EmitterRadius;
    float2 pad2;
    float InitialVelocity;

    float4 ParticleBlendColor;

    float EnergyDissipationRate; //Energy dissipation rate per second
    float3 InitialAcceleration;
};
#endif

#if defined(SSAO)
static const uint SSAOKernalSize = 32;
cbuffer cbSSAO : register(b1)
{
    float4 kernel[SSAOKernalSize];
    float2 noiseScale;
    int texScale; // Used when viewport size does not match texture size
    float radius;    
    float4x4 invProjection;
}
#endif

///------------------Textures---------------------
Texture2D texDiffuseMap : register(t0);
Texture2D<float3> texNormalMap : register(t1);

#if !defined(PBR)
Texture2D texAlphaMap : register(t2);
Texture2D texSpecularMap : register(t3);
Texture2D<float3> texEmissiveMap : register(t5);
#endif
#if defined(PBR)
Texture2D<float3> texRMMap    : register(t2);
Texture2D<float> texAOMap : register(t3);
Texture2D<float3> texEmissiveMap : register(t5);
TextureCube<float3> texIrradianceMap : register(t21);
#endif
Texture2D<float3> texDisplacementMap : register(t4);
TextureCube<float3> texCubeMap : register(t20); // Radiance Map

Texture2D<float> texShadowMap : register(t30);

Texture2D texSSAOMap : register(t31);
#if defined(SSAO)
Texture2D<float3> texSSAONoise : register(t32);
Texture2D<float> texSSAODepth : register(t33);
#endif

Texture2D texParticle : register(t0);
StructuredBuffer<Particle> SimulationState : register(t0);
Texture2D billboardTexture : register(t0);; // billboard text image

Texture2D texOITColor : register(t10);
Texture2D texOITAlpha : register(t11);

Texture1D texColorStripe1DX : register(t12);
Texture1D texColorStripe1DY : register(t13);

StructuredBuffer<matrix> skinMatrices : register(t40);

Texture2D texSprite : register(t50);

Texture3D texVolume : register(t0);
Texture2D texVolumeFront : register(t1);
Texture2D texVolumeBack : register(t2);
///------------------Samplers-------------------
SamplerState samplerSurface : register(s0);
SamplerState samplerIBL : register(s1);

SamplerState samplerDisplace : register(s3);

SamplerState samplerCube : register(s4);

SamplerComparisonState samplerShadow : register(s5);

SamplerState samplerParticle : register(s6);

SamplerState samplerBillboard : register(s7);

SamplerState samplerSprite : register(s8);

SamplerState samplerVolume : register(s9);

#if defined(SSAO)
SamplerState samplerNoise : register(s1);
#endif
///---------------------UAV-----------------------------

ConsumeStructuredBuffer<Particle> CurrentSimulationState : register(u0);
AppendStructuredBuffer<Particle> NewSimulationState : register(u1);


#endif