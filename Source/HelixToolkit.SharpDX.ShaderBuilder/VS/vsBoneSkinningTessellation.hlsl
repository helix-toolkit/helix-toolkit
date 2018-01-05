#ifndef VSBONESKINNINGTESSELLATION_HLSL
#define VSBONESKINNINGTESSELLATION_HLSL
#define MESH
#include"..\Common\Common.hlsl"


HSInput main(VSBoneSkinInput input)
{
	HSInput output = (HSInput) 0;
	float4 inputp = input.p;
	if (bInvertNormal)
	{
        input.n = -input.n;
    }
	float3 inputn = input.n;
	if (bHasBones)
	{
		int4 bones = clamp(input.bones, minBoneV, maxBoneV);

        inputp = mul(input.p, skinMatrices[bones.x]) * input.boneWeights.x;
        inputn = mul(input.n, (float3x3) skinMatrices[bones.x]) * input.boneWeights.x;

        inputp += mul(input.p, skinMatrices[bones.y]) * input.boneWeights.y;
        inputn += mul(input.n, (float3x3) skinMatrices[bones.y]) * input.boneWeights.y;

        inputp += mul(input.p, skinMatrices[bones.z]) * input.boneWeights.z;
        inputn += mul(input.n, (float3x3) skinMatrices[bones.z]) * input.boneWeights.z;

        inputp += mul(input.p, skinMatrices[bones.w]) * input.boneWeights.w;
        inputn += mul(input.n, (float3x3) skinMatrices[bones.w]) * input.boneWeights.w;
    }
	float3 inputt1 = input.t1;
	float3 inputt2 = input.t2;
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
        inputp = mul(inputp, mInstance);
        inputn = mul(inputn, (float3x3)mInstance);
		if (bHasNormalMap)
		{
			inputt1 = mul(inputt1, (float3x3) mInstance);
			inputt2 = mul(inputt2, (float3x3) mInstance);
		}
	}
    output.p = inputp.xyz;
    output.n = inputn;
    output.t = input.t;
    output.t1 = inputt1;
    output.t2 = inputt2;
    output.c = input.c;
    output.c2 = vMaterialEmissive + vMaterialAmbient * vLightAmbient;
	return output;
}

#endif