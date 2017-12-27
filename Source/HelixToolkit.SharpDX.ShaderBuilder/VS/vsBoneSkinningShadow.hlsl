#define MATERIAL
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

#pragma pack_matrix( row_major )
#define MaxBones 128

static const int4 minBoneV = { 0, 0, 0, 0 };
static const int4 maxBoneV = { MaxBones - 1, MaxBones - 1, MaxBones - 1, MaxBones - 1 };

cbuffer cbBoneSkinning
{
    matrix cbSkinMatrices[MaxBones];
};

float4 main(VSBoneSkinInput input) : SV_Position
{
    float4 inputp = input.p;

    if (bHasBones)
    {
        int4 bones = clamp(input.bones, minBoneV, maxBoneV);

        inputp = mul(input.p, cbSkinMatrices[bones.x]) * input.boneWeights.x;

        inputp += mul(input.p, cbSkinMatrices[bones.y]) * input.boneWeights.y;

        inputp += mul(input.p, cbSkinMatrices[bones.z]) * input.boneWeights.z;

        inputp += mul(input.p, cbSkinMatrices[bones.w]) * input.boneWeights.w;
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