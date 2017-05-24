#ifndef LIGHTING_FX
#define LIGHTING_FX

#include "Common.fx"
#include "Material.fx"
#include "DataStructs.fx"
//--------------------------------------------------------------------------------------
// Constant Buffer Variables
//--------------------------------------------------------------------------------------
cbuffer cbPerFrame
{
	int iLightType[LIGHTS];
	// the light direction is here the vector which looks towards the light
	float4 vLightDir[LIGHTS];
	float4 vLightPos[LIGHTS];
	float4 vLightAtt[LIGHTS];
	float4 vLightSpot[LIGHTS];        //(outer angle , inner angle, falloff, free)
	float4 vLightColor[LIGHTS];
	matrix mLightView[LIGHTS];
	matrix mLightProj[LIGHTS];

};

float4 vLightAmbient = float4(0.2f, 0.2f, 0.2f, 1.0f);

//--------------------------------------------------------------------------------------
// normal mapping
//--------------------------------------------------------------------------------------
// This function returns the normal in world coordinates.
// The input struct contains tangent (t1), bitangent (t2) and normal (n) of the
// unperturbed surface in world coordinates. The perturbed normal in tangent space
// can be read from texNormalMap.
// The RGB values in this texture need to be normalized from (0, +1) to (-1, +1).
float3 calcNormal(PSInput input)
{
	if (bHasNormalMap)
	{
		// Normalize the per-pixel interpolated tangent-space
		input.n = normalize(input.n);
		input.t1 = normalize(input.t1);
		input.t2 = normalize(input.t2);

		// Sample the texel in the bump map.
		float4 bumpMap = texNormalMap.Sample(NormalSampler, input.t);
		// Expand the range of the normal value from (0, +1) to (-1, +1).
		bumpMap = (bumpMap * 2.0f) - 1.0f;
		// Calculate the normal from the data in the bump map.
		input.n = input.n + bumpMap.x * input.t1 + bumpMap.y * input.t2;
	}
	return normalize(input.n);
}


//--------------------------------------------------------------------------------------
// Phong Lighting Reflection Model
//--------------------------------------------------------------------------------------
// Returns the sum of the diffuse and specular terms in the Phong reflection model
// The specular and diffuse reflection constants for the currently loaded material (k_d and k_s) as well
// as other material properties are defined in Material.fx.
float4 calcPhongLighting(float4 LColor, float4 vMaterialTexture, float3 N, float3 L, float3 V, float3 R)
{
	float4 Id = vMaterialTexture * vMaterialDiffuse * saturate(dot(N, L));
	float4 Is = vMaterialSpecular * pow(saturate(dot(R, V)), sMaterialShininess);
	return (Id + Is) * LColor;
}

//--------------------------------------------------------------------------------------
// Blinn-Phong Lighting Reflection Model
//--------------------------------------------------------------------------------------
// Returns the sum of the diffuse and specular terms in the Blinn-Phong reflection model.
float4 calcBlinnPhongLighting(float4 LColor, float4 vMaterialTexture, float3 N, float4 vMaterialDiffuse, float3 L, float3 H)
{
	float4 Id = vMaterialTexture * vMaterialDiffuse * saturate(dot(N, L));
	float4 Is = vMaterialSpecular * pow(saturate(dot(N, H)), sMaterialShininess);
	return (Id + Is) * LColor;
}


//--------------------------------------------------------------------------------------
// reflectance mapping
//--------------------------------------------------------------------------------------
float4 cubeMapReflection(PSInput input, float4 I)
{
	float3 v = normalize((float3)input.wp - vEyePos);
	float3 r = reflect(v, input.n);
	return (1.0f - vMaterialReflect)*I + vMaterialReflect*texCubeMap.Sample(LinearSampler, r);
}

//--------------------------------------------------------------------------------------
// get shadow color
//--------------------------------------------------------------------------------------
float2 texOffset(int u, int v)
{
	return float2(u * 1.0f / vShadowMapSize.x, v * 1.0f / vShadowMapSize.y);
}

//--------------------------------------------------------------------------------------
// get shadow color
//--------------------------------------------------------------------------------------
float shadowStrength(float4 sp)
{
	sp = sp / sp.w;
	if (sp.x < -1.0f || sp.x > 1.0f || sp.y < -1.0f || sp.y > 1.0f || sp.z < 0.0f || sp.z > 1.0f)
	{
		return 1;
	}
	sp.x = sp.x / +2.0 + 0.5;
	sp.y = sp.y / -2.0 + 0.5;

	//apply shadow map bias
	sp.z -= vShadowMapInfo.z;

	//// --- not in shadow, hard cut
	//float shadowMapDepth = texShadowMap.Sample(PointSampler, sp.xy).r;
	//if ( shadowMapDepth < sp.z) 
	//{
	//	return 0;
	//}

	//// --- basic hardware PCF - single texel
	//float shadowFactor = texShadowMap.SampleCmpLevelZero( CmpSampler, sp.xy, sp.z ).r;

	//// --- PCF sampling for shadow map
	float sum = 0;
	float x = 0, y = 0;
	float range = vShadowMapInfo.y;
	float div = 0.0000001;

	// ---perform PCF filtering on a 4 x 4 texel neighborhood
	for (y = -range; y <= range; y += 1.0)
	{
		for (x = -range; x <= range; x += 1.0)
		{
			sum += texShadowMap.SampleCmpLevelZero(CmpSampler, sp.xy + texOffset(x, y), sp.z);
			div++;
		}
	}

	float shadowFactor = sum / (float)div;
	float fixTeil = vShadowMapInfo.x;
	float nonTeil = 1 - vShadowMapInfo.x;
	// now, put the shadow-strengh into the 0-nonTeil range
	nonTeil = shadowFactor*nonTeil;
	return (fixTeil + nonTeil);
}


#endif