#ifndef VSBONESKINNINGSHADOW_HLSL
#define VSBONESKINNINGSHADOW_HLSL
#define MESH
#include"..\Common\Common.hlsl"

float4 main(VSBoneSkinInput input) : SV_Position
{
    float4 inputp = input.p;

    if (bHasBones)
    {
        int4 bones = clamp(input.bones, minBoneV, maxBoneV);

        inputp = mul(input.p, skinMatrices[bones.x]) * input.boneWeights.x;

        inputp += mul(input.p, skinMatrices[bones.y]) * input.boneWeights.y;

        inputp += mul(input.p, skinMatrices[bones.z]) * input.boneWeights.z;

        inputp += mul(input.p, skinMatrices[bones.w]) * input.boneWeights.w;
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
    }
		
	//set position into world space	
    inputp = mul(inputp, mWorld);
    inputp = mul(inputp, vLightViewProjection);
    return inputp;
}
#endif