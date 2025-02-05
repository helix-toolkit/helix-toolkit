#ifndef VSMESHDEFAULT_HLSL
#define VSMESHDEFAULT_HLSL

#define MESH
#include "..\Common\Common.hlsl"
#include "..\Common\DataStructs.hlsl"
#pragma pack_matrix( row_major )

#if !defined(INSTANCINGPARAM)
#if defined(CLIPPLANE)
PSInputClip main(VSInput input)
#endif
#if !defined(CLIPPLANE)
PSInput main(VSInput input)
#endif
#endif
#if defined(INSTANCINGPARAM)
PSInput main(VSInstancingInput input)
#endif
{
#if !defined(CLIPPLANE)
	PSInput output = (PSInput) 0;
#endif
#if defined(CLIPPLANE)
	PSInputClip output = (PSInputClip)0;
#endif
	output.wp = mul(input.p, mWorld);
	output.n = normalize(mul(input.n, (float3x3) mWorld));
	output.t1 = input.t1;
	output.t2 = input.t2;
	if (bInvertNormal)
	{
		output.n = -output.n;
	}

	if (bHasNormalMap)
	{
		if (!bAutoTengent)
		{
			// transform the tangents by the world matrix and normalize
			output.t1 = normalize(mul(output.t1, (float3x3) mWorld));
			output.t2 = normalize(mul(output.t2, (float3x3) mWorld));
		}
	}
	// compose instance matrix
	if (bHasInstances)
	{
		matrix mInstance =
		{
			input.mr0,
			input.mr1,
			input.mr2,
			input.mr3
		};
		output.wp = mul(output.wp, mInstance);
		output.n = normalize(mul(output.n, (float3x3) mInstance));
		if (bHasNormalMap)
		{
			if (!bAutoTengent)
			{
				output.t1 = mul(output.t1, (float3x3) mInstance);
				output.t2 = mul(output.t2, (float3x3) mInstance);
			}
		}
	}
	
	float3 vEye = vEyePos - output.wp.xyz;
	output.vEye = float4(normalize(vEye), length(vEye)); //Use wp for camera->vertex direction

	//set color
	output.c = input.c;

#if !defined(INSTANCINGPARAM)
	//set texture coords
	output.t = mul(float2x4(uvTransformR1, uvTransformR2), float4(input.t, 0, 1)).xy;
	output.cDiffuse = vMaterialDiffuse ;
	output.c2 = vMaterialEmissive;
#endif

#if defined(INSTANCINGPARAM)
	if (!bHasInstanceParams)
	{
		output.t = mul(float2x4(uvTransformR1, uvTransformR2), float4(input.t, 0, 1)).xy;
		output.cDiffuse = vMaterialDiffuse;
		if (!bRenderPBR)
		{            
			output.c2 = vMaterialEmissive;
		}
		else
		{
			output.c2 = vMaterialSpecular;
		}
	}
	else
	{
		//set texture coords and color
		output.t = mul(float2x4(uvTransformR1, uvTransformR2), float4(input.t, 0, 1)).xy + input.tOffset;
		output.cDiffuse = input.diffuseC;
		if (!bRenderPBR)
		{
			output.c2 = input.emissiveC; 
		}
		else
		{
			output.c2 = input.emissiveC;
		}
	}
#endif

	if (bHasDisplacementMap)
	{
		const float mipInterval = 20;
		float mipLevel = clamp((distance(output.wp.xyz, vEyePos) - mipInterval) / mipInterval, 0, 6);
		float3 h = texDisplacementMap.SampleLevel(samplerDisplace, output.t, mipLevel);
		output.wp.xyz += output.n * mul(h, displacementMapScaleMask.xyz);
	}

	//set position into clip space	
	output.p = mul(output.wp, mViewProjection);

	//set position into light-clip space
	if (bHasShadowMap)
	{
		output.sp = mul(output.wp, mul(vLightView, vLightProjection));
	}


#if defined(CLIPPLANE)
	output.clipPlane = float4(0, 0, 0, 0);
	if (EnableCrossPlane.x)
	{
		float3 p = output.wp.xyz - CrossPlane1Params.xyz * CrossPlane1Params.w;
		output.clipPlane.x = dot(CrossPlane1Params.xyz, p);
	}
	if (EnableCrossPlane.y)
	{
		float3 p = output.wp.xyz - CrossPlane2Params.xyz * CrossPlane2Params.w;
		output.clipPlane.y = dot(CrossPlane2Params.xyz, p);
	}
	if (EnableCrossPlane.z)
	{
		float3 p = output.wp.xyz - CrossPlane3Params.xyz * CrossPlane3Params.w;
		output.clipPlane.z = dot(CrossPlane3Params.xyz, p);
	}
	if (EnableCrossPlane.w)
	{
		float3 p = output.wp.xyz - CrossPlane4Params.xyz * CrossPlane4Params.w;
		output.clipPlane.w = dot(CrossPlane4Params.xyz, p);
	}
	if (EnableCrossPlane5To8.x)
	{
		float3 p = output.wp.xyz - CrossPlane5Params.xyz * CrossPlane5Params.w;
		output.clipPlane5To8.x = dot(CrossPlane5Params.xyz, p);
	}
	if (EnableCrossPlane5To8.y)
	{
		float3 p = output.wp.xyz - CrossPlane6Params.xyz * CrossPlane6Params.w;
		output.clipPlane5To8.y = dot(CrossPlane6Params.xyz, p);
	}
	if (EnableCrossPlane5To8.z)
	{
		float3 p = output.wp.xyz - CrossPlane7Params.xyz * CrossPlane7Params.w;
		output.clipPlane5To8.z = dot(CrossPlane7Params.xyz, p);
	}
	if (EnableCrossPlane5To8.w)
	{
		float3 p = output.wp.xyz - CrossPlane8Params.xyz * CrossPlane8Params.w;
		output.clipPlane5To8.w = dot(CrossPlane8Params.xyz, p);
	}
	if (CuttingOperation == 1)
	{
		output.clipPlane.x = -(whenle(-output.clipPlane.x, 0) * whenle(-output.clipPlane.y, 0) 
							 * whenle(-output.clipPlane.z, 0) * whenle(-output.clipPlane.w, 0)
							 * whenle(-output.clipPlane5To8.x, 0) * whenle(-output.clipPlane5To8.y, 0) 
							 * whenle(-output.clipPlane5To8.z, 0) * whenle(-output.clipPlane5To8.w, 0));
		output.clipPlane.yzw = float3(0, 0, 0);
		output.clipPlane5To8 = float4(0, 0, 0, 0);
	}
#endif
	return output;
}

#endif