#ifndef VSBONESKINNING_HLSL
#define VSBONESKINNING_HLSL
#define MESH
#include"..\Common\Common.hlsl"

VSSkinnedOutput main(VSSkinnedInput input)
{
    VSSkinnedOutput output = (VSSkinnedOutput) 0;
    float4 inputp = input.p;
    float3 inputn = input.n;
    output.p = inputp;
    output.n = inputn;
    float3 inputt1 = input.t1;
    float3 inputt2 = input.t2;

    //if (bHasBones)
    {
        int4 bones = clamp(input.bones, minBoneV, maxBoneV);

        output.p = mul(inputp, skinMatrices[bones.x]) * input.boneWeights.x;
        output.n = mul(inputn, (float3x3) skinMatrices[bones.x]) * input.boneWeights.x;


        output.p += mul(inputp, skinMatrices[bones.y]) * input.boneWeights.y;
        output.n += mul(inputn, (float3x3) skinMatrices[bones.y]) * input.boneWeights.y;


        output.p += mul(inputp, skinMatrices[bones.z]) * input.boneWeights.z;
        output.n += mul(inputn, (float3x3) skinMatrices[bones.z]) * input.boneWeights.z;


        output.p += mul(inputp, skinMatrices[bones.w]) * input.boneWeights.w;
        output.n += mul(inputn, (float3x3) skinMatrices[bones.w]) * input.boneWeights.w;
        output.n = normalize(output.n);

        //if (bHasNormalMap)
        {
            output.t1 = mul(inputt1, (float3x3) skinMatrices[bones.x]) * input.boneWeights.x;
            output.t2 = mul(inputt2, (float3x3) skinMatrices[bones.x]) * input.boneWeights.x;
            output.t1 += mul(inputt1, (float3x3) skinMatrices[bones.y]) * input.boneWeights.y;
            output.t2 += mul(inputt2, (float3x3) skinMatrices[bones.y]) * input.boneWeights.y;
            output.t1 += mul(inputt1, (float3x3) skinMatrices[bones.z]) * input.boneWeights.z;
            output.t2 += mul(inputt2, (float3x3) skinMatrices[bones.z]) * input.boneWeights.z;
            output.t1 += mul(inputt1, (float3x3) skinMatrices[bones.w]) * input.boneWeights.w;
            output.t2 += mul(inputt2, (float3x3) skinMatrices[bones.w]) * input.boneWeights.w;

            output.t1 = normalize(output.t1);
            output.t2 = normalize(output.t2);
        }
    }
    return output;
}

#endif