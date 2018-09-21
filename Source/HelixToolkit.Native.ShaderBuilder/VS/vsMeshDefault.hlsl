#ifndef VSMESHDEFAULT_HLSL
#define VSMESHDEFAULT_HLSL

#define MESH
#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"
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
	float4 inputp = input.p;
	float3 inputn = input.n;
	float3 inputt1 = input.t1;
	float3 inputt2 = input.t2;
	if (bInvertNormal)
	{
		inputn = -inputn;
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
		inputp = mul(input.p, mInstance);
		inputn = mul(inputn, (float3x3) mInstance);
		if (bHasNormalMap)
		{
            if (!bAutoTengent)
            {
			    inputt1 = mul(inputt1, (float3x3) mInstance);
			    inputt2 = mul(inputt2, (float3x3) mInstance);
            }
		}
	}

	//set position into world space	
	output.p = mul(inputp, mWorld);
    float3 vEye = vEyePos - output.p.xyz;
    output.vEye = float4(normalize(vEye), length(vEye)); //Use wp for camera->vertex direction
	//set normal for interpolation	
    output.n = normalize(mul(inputn, (float3x3) mWorld));
    //set color
	output.c = input.c;

#if !defined(INSTANCINGPARAM)
	//set texture coords
    output.t = mul(float2x4(uvTransformR1, uvTransformR2), float4(input.t, 0, 1)).xy;
    output.cDiffuse = vMaterialDiffuse;
    if (!bRenderPBR)
    {
        output.c2 = mad(vMaterialAmbient, vLightAmbient, vMaterialEmissive);
    }
    else
    {
        output.c2 = vMaterialSpecular;
    }
#endif

#if defined(INSTANCINGPARAM)
    if (!bHasInstanceParams)
    {
        output.t = mul(float2x4(uvTransformR1, uvTransformR2), float4(input.t, 0, 1)).xy;
        output.cDiffuse = vMaterialDiffuse;
        if (!bRenderPBR)
        {
            output.c2 = mad(vMaterialAmbient, vLightAmbient, vMaterialEmissive);
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
            output.c2 = mad(input.ambientC, vLightAmbient, input.emissiveC);
        }
        else
        {
            output.c2 = input.ambientC;
        }
    }
#endif

    if (bHasDisplacementMap)
    {
        const float mipInterval = 20;
        float mipLevel = clamp((distance(output.p.xyz, vEyePos) - mipInterval) / mipInterval, 0, 6);
        float3 h = texDisplacementMap.SampleLevel(samplerDisplace, output.t, mipLevel);
        output.p.xyz += output.n * mul(h, displacementMapScaleMask.xyz);
    }
	output.wp = output.p;
	//set position into clip space	
    output.p = mul(output.p, mViewProjection);

	//set position into light-clip space
	if (bHasShadowMap)
	{
        output.sp = mul(output.wp, vLightViewProjection);
	}

	if (bHasNormalMap)
	{
        if (!bAutoTengent)
        {
		    // transform the tangents by the world matrix and normalize
            output.t1 = normalize(mul(inputt1, (float3x3) mWorld));
		    output.t2 = normalize(mul(inputt2, (float3x3) mWorld));
        }
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
    if (CuttingOperation == 1)
    {
        output.clipPlane.x = -(whenle(-output.clipPlane.x, 0) * whenle(-output.clipPlane.y, 0) * whenle(-output.clipPlane.z, 0) * whenle(-output.clipPlane.w, 0));
        output.clipPlane.yzw = float3(0, 0, 0);
    }
#endif
	return output;
}

#endif