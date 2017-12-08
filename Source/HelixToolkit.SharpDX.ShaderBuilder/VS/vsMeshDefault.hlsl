#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Material.hlsl"

PSInput main(VSInput input)
{
	PSInput output = (PSInput) 0;
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
			input.mr0.x, input.mr1.x, input.mr2.x, input.mr3.x, // row 1
			input.mr0.y, input.mr1.y, input.mr2.y, input.mr3.y, // row 2
			input.mr0.z, input.mr1.z, input.mr2.z, input.mr3.z, // row 3
			input.mr0.w, input.mr1.w, input.mr2.w, input.mr3.w, // row 4
		};
		inputp = mul(mInstance, input.p);
		inputn = mul((float3x3) mInstance, inputn);
		if (bHasNormalMap)
		{
			inputt1 = mul((float3x3) mInstance, inputt1);
			inputt2 = mul((float3x3) mInstance, inputt2);
		}
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
			output.sp = mul(output.sp, Lights[0].mLightView);
			output.sp = mul(output.sp, Lights[0].mLightProj);
		}
	}

	//set texture coords and color
	output.t = input.t;
	output.c = input.c;
	output.c2 = vMaterialEmissive + vMaterialAmbient * vLightAmbient;
	//set normal for interpolation	
	output.n = normalize(mul(inputn, (float3x3) mWorld));


	if (bHasNormalMap)
	{
		// transform the tangents by the world matrix and normalize
		output.t1 = normalize(mul(inputt1, (float3x3) mWorld));
		output.t2 = normalize(mul(inputt2, (float3x3) mWorld));
	}
	else
	{
		output.t1 = 0.0f;
		output.t2 = 0.0f;
	}

	return output;
}