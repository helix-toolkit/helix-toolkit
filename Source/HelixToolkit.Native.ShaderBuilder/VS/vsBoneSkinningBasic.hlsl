#ifndef VSBONESKINNING_HLSL
#define VSBONESKINNING_HLSL
#define MESH
#include"..\Common\Common.hlsl"

VSSkinnedOutput main(VSSkinnedInput input)
{
    VSSkinnedOutput output = (VSSkinnedOutput) 0;
    float4 inputp = input.p;
    float3 inputn = input.n;
    float3 inputt1 = input.t1;
    float3 inputt2 = input.t2;

    [unroll]
    for (int i = 0; i < 4; ++i)
    {
        matrix b = skinMatrices[input.bones[i]];
        output.p += mul(b, inputp) * input.boneWeights[i];
        float3x3 b33 = (float3x3) b;

        output.n += mul(b33, inputn) * input.boneWeights[i];
        output.t1 += mul(b33, inputt1) * input.boneWeights[i];
        output.t2 += mul(b33, inputt2) * input.boneWeights[i];
    }
    //matrix b = skinMatrices[bones.x];
    //output.p = mul(b, inputp) * input.boneWeights.x;
    //float3x3 b33 = (float3x3) b;

    //output.n = mul(b33, inputn) * input.boneWeights.x;
    //output.t1 = mul(b33, inputt1) * input.boneWeights.x;
    //output.t2 = mul(b33, inputt2) * input.boneWeights.x;

    //b = skinMatrices[bones.y];
    //output.p += mul(b, inputp) * input.boneWeights.y;
    //b33 = (float3x3) b;
    //output.n += mul(b33, inputn) * input.boneWeights.y;
    //output.t1 += mul(b33, inputt1) * input.boneWeights.y;
    //output.t2 += mul(b33, inputt2) * input.boneWeights.y;

    //b = skinMatrices[bones.z];
    //output.p += mul(b, inputp) * input.boneWeights.z;
    //b33 = (float3x3) b;
    //output.n += mul(b33, inputn) * input.boneWeights.z;
    //output.t1 += mul(b33, inputt1) * input.boneWeights.z;
    //output.t2 += mul(b33, inputt2) * input.boneWeights.z;

    //b = skinMatrices[bones.w];
    //output.p += mul(b, inputp) * input.boneWeights.w;
    //b33 = (float3x3) b;
    //output.n += mul(b33, inputn) * input.boneWeights.w;
    //output.t1 += mul(b33, inputt1) * input.boneWeights.w;
    //output.t2 += mul(b33, inputt2) * input.boneWeights.w;

    output.n = normalize(output.n);
    output.t1 = normalize(output.t1);
    output.t2 = normalize(output.t2);
    return output;
}

#endif