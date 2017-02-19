#ifndef VERTEXSHADERS_FX
#define VERTEXSHADERS_FX

#include "./Shaders/Lighting.fx"
#include "./Shaders/Common.fx"

//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING  - Vertex Shader
//--------------------------------------------------------------------------------------
PSInput VShaderDefault(VSInput input)
{
	PSInput output = (PSInput)0;
	float4 inputp = input.p;
	float4 inputn = float4(input.n, 1);
	// compose instance matrix
	if (bHasInstances)
	{
		matrix mInstance =
		{
			input.mr0.x, input.mr1.x, input.mr2.x, input.mr3.x, // row 1
			input.mr0.y, input.mr1.y, input.mr2.y, input.mr3.y, // row 2
			input.mr0.z, input.mr1.z, input.mr2.z, input.mr3.z, // row 3
			input.mr0.w, input.mr1.w, input.mr2.w, input.mr3.w, // row 4
		};
		inputp = mul(mInstance, input.p);
		inputn = mul(mInstance, input.n);
	}

	//set position into camera clip space	
	output.p = mul(inputp, mWorld);
	output.wp = output.p;
	output.p = mul(output.p, mView);
	output.p = mul(output.p, mProjection);

	//set position into light-clip space
	if (bHasShadowMap)
	{
		//for (int i = 0; i < 1; i++)
		{
			output.sp = mul(inputp, mWorld);
			output.sp = mul(output.sp, mLightView[0]);
			output.sp = mul(output.sp, mLightProj[0]);
		}
	}

	//set texture coords and color
	output.t = input.t;
	output.c = input.c;

	//set normal for interpolation	
	output.n = normalize(mul(inputn.xyz, (float3x3)mWorld));


	if (bHasNormalMap)
	{
		// transform the tangents by the world matrix and normalize
		output.t1 = normalize(mul(input.t1, (float3x3)mWorld));
		output.t2 = normalize(mul(input.t2, (float3x3)mWorld));
	}
	else
	{
		output.t1 = 0.0f;
		output.t2 = 0.0f;
	}

	return output;
}

//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING  - Vertex Shader
//--------------------------------------------------------------------------------------
PSInput VInstancingShader(VSInstancingInput input)
{
	PSInput output = (PSInput)0;
	float4 inputp = input.p;
	float4 inputn = float4(input.n, 1);
	// compose instance matrix
	if (bHasInstances)
	{
		matrix mInstance =
		{
			input.mr0.x, input.mr1.x, input.mr2.x, input.mr3.x, // row 1
			input.mr0.y, input.mr1.y, input.mr2.y, input.mr3.y, // row 2
			input.mr0.z, input.mr1.z, input.mr2.z, input.mr3.z, // row 3
			input.mr0.w, input.mr1.w, input.mr2.w, input.mr3.w, // row 4
		};
		inputp = mul(mInstance, input.p);
		inputn = mul(mInstance, input.n);
	}

	//set position into camera clip space	
	output.p = mul(inputp, mWorld);
	output.wp = output.p;
	output.p = mul(output.p, mView);
	output.p = mul(output.p, mProjection);

	//set position into light-clip space
	if (bHasShadowMap)
	{
		//for (int i = 0; i < 1; i++)
		{
			output.sp = mul(inputp, mWorld);
			output.sp = mul(output.sp, mLightView[0]);
			output.sp = mul(output.sp, mLightProj[0]);
		}
	}

	if (!bHasInstanceParams)
	{
		output.t = input.t;
		output.c = vMaterialDiffuse;
		output.c2 = vMaterialEmissive + vMaterialAmbient * vLightAmbient;
	}
	else
	{
		//set texture coords and color
		output.t = input.t + input.tOffset;
		output.c = input.diffuseC;
		output.c2 = input.emissiveC + input.ambientC * vLightAmbient;
	}

	//set normal for interpolation	
	output.n = normalize(mul(inputn.xyz, (float3x3)mWorld));


	if (bHasNormalMap)
	{
		// transform the tangents by the world matrix and normalize
		output.t1 = normalize(mul(input.t1, (float3x3)mWorld));
		output.t2 = normalize(mul(input.t2, (float3x3)mWorld));
	}
	else
	{
		output.t1 = 0.0f;
		output.t2 = 0.0f;
	}

	return output;
}


PSInputCube VShaderCubeMap(float4 p : POSITION)
{
	PSInputCube output = (PSInputCube)0;

	//set position into clip space		
	output.p = mul(p, mWorld);
	output.p = mul(output.p, mView);
	output.p = mul(output.p, mProjection).xyww;

	//set texture coords and color
	//output.t = input.t;	
	//output.c = p;

	//Set Pos to xyww instead of xyzw, so that z will always be 1 (furthest from camera)	
	output.t = p.xyz;

	return output;
}


PSInput VShaderBoneSkin(VSBoneSkinInput input)
{
	PSInput output = (PSInput)0;
	float4 inputp = input.p;
	float4 inputn = float4(input.n, 1);

	output.p = inputp;
	output.n = inputn;
	if (bHasBones && input.bones.x >= 0 && input.bones.y >= 0 && input.bones.z >= 0 && input.bones.w >= 0
		&& input.bones.x < MaxBones && input.bones.y < MaxBones && input.bones.z < MaxBones && input.bones.w < MaxBones)
	{
		matrix m = SkinMatrices[0];// { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 };
		output.p = mul(inputp, SkinMatrices[input.bones.x]) * input.boneWeights.x;
		output.p += mul(inputp, SkinMatrices[input.bones.y]) * input.boneWeights.y;
		output.p += mul(inputp, SkinMatrices[input.bones.z]) * input.boneWeights.z;
		output.p += mul(inputp, SkinMatrices[input.bones.w]) * input.boneWeights.w;

		output.n = mul(inputn, SkinMatrices[input.bones.x]) * input.boneWeights.x;
		output.n += mul(inputn, SkinMatrices[input.bones.y]) * input.boneWeights.y;
		output.n += mul(inputn, SkinMatrices[input.bones.z]) * input.boneWeights.z;
		output.n += mul(inputn, SkinMatrices[input.bones.w]) * input.boneWeights.w;
	}

	// compose instance matrix
	if (bHasInstances)
	{
		matrix mInstance =
		{
			input.mr0.x, input.mr1.x, input.mr2.x, input.mr3.x, // row 1
			input.mr0.y, input.mr1.y, input.mr2.y, input.mr3.y, // row 2
			input.mr0.z, input.mr1.z, input.mr2.z, input.mr3.z, // row 3
			input.mr0.w, input.mr1.w, input.mr2.w, input.mr3.w, // row 4
		};
		output.p = mul(mInstance, output.p);
		output.n = mul(mInstance, output.n);
	}
		
	//set position into camera clip space	
	output.p = mul(output.p, mWorld);
	output.wp = output.p;
	output.p = mul(output.p, mView);
	output.p = mul(output.p, mProjection);

	//set position into light-clip space
	if (bHasShadowMap)
	{
		//for (int i = 0; i < 1; i++)
		{
			output.sp = mul(inputp, mWorld);
			output.sp = mul(output.sp, mLightView[0]);
			output.sp = mul(output.sp, mLightProj[0]);
		}
	}

	//set texture coords and color
	output.t = input.t;
	output.c = input.c;

	//set normal for interpolation	
	output.n = normalize(mul(output.n.xyz, (float3x3)mWorld));


	if (bHasNormalMap)
	{
		// transform the tangents by the world matrix and normalize
		output.t1 = normalize(mul(input.t1, (float3x3)mWorld));
		output.t2 = normalize(mul(input.t2, (float3x3)mWorld));
	}
	else
	{
		output.t1 = 0.0f;
		output.t2 = 0.0f;
	}

	return output;
}

#endif