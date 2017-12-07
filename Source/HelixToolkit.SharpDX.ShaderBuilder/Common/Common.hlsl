#ifndef COMMON_HLSL
#define COMMON_HLSL
#pragma pack_matrix( row_major )
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
	// [w,h,0,0]
    float4 vViewport;		
	// camera position
    float3 vEyePos;
    float padding0;
};

//Per model
cbuffer cbModel : register(b1)
{
    float4x4 mWorld;
    bool bInvertNormal = false;
    bool bHasInstances = false;
    bool bHasInstanceParams = false;   
    bool padding1;
};

//--------------------------------------------------------------------------------------
// pre-processor defines
//--------------------------------------------------------------------------------------
#define LIGHTS 8
//--------------------------------------------------------------------------------------
// Light Buffer
//--------------------------------------------------------------------------------------
struct LightStruct
{
    int iLightType; //4
    float3 paddingL;
	// the light direction is here the vector which looks towards the light
    float4 vLightDir;//8
    float4 vLightPos;//12
    float4 vLightAtt;//16
    float4 vLightSpot; //(outer angle , inner angle, falloff, free), 20
    float4 vLightColor;//24
    matrix mLightView;//40
    matrix mLightProj;//56
};

cbuffer cbLights : register(b2)
{
    LightStruct Lights[LIGHTS];
    float4 vLightAmbient = float4(0.2f, 0.2f, 0.2f, 1.0f);
};

//--------------------------------------------------------------------------------------
// GLOBAL FUNCTIONS
//--------------------------------------------------------------------------------------

//--------------------------------------------------------------------------------------
// From projection frame to window pixel pos.
//--------------------------------------------------------------------------------------
float2 projToWindow(in float4 pos)
{
    return float2(vViewport.x * 0.5 * (1.0 + (pos.x / pos.w)),
                  vViewport.y * 0.5 * (1.0 - (pos.y / pos.w)));
}

//--------------------------------------------------------------------------------------
// From window pixel pos to projection frame at the specified z (view frame). 
//--------------------------------------------------------------------------------------
float4 windowToProj(in float2 pos, in float z, in float w)
{
    return float4(((pos.x * 2.0 / vViewport.x) - 1.0) * w,
                  ((pos.y * 2.0 / vViewport.y) - 1.0) * -w,
                  z, w);
}

int whengt(float l, float cmp)
{
    return max(sign(l - cmp), 0);
}

int whenlt(float l, float cmp)
{
    return max(sign(cmp - l), 0);
}

int whenge(float l, float cmp)
{
    return 1 - whenlt(l, cmp);
}

int whenle(float l, float cmp)
{
    return 1 - whengt(l, cmp);
}


#endif