#ifndef VSBONESKINNINGWIREFRAME_HLSL
#define VSBONESKINNINGWIREFRAME_HLSL
#define MESH
#include"..\Common\Common.hlsl"

float4 main(VSBoneSkinInput input) : SV_Position
{
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
        inputn = mul(inputn, (float3x3) mInstance);
    }
		
	//set position into world space	
    inputp = mul(inputp, mWorld);

    if (bHasDisplacementMap)
    {
	    //set normal for interpolation	
        inputn = normalize(mul(inputn.xyz, (float3x3) mWorld));
        const float mipInterval = 20;
        float mipLevel = clamp((distance(inputp.xyz, vEyePos) - mipInterval) / mipInterval, 0, 6);
        float4 h = texDisplacementMap.SampleLevel(samplerDisplace, input.t, mipLevel);
        inputp.xyz += inputn * mul(h, displacementMapScaleMask);
    }

	//set position into clip space	
    inputp = mul(inputp, mViewProjection);

    return inputp;
}

#endif