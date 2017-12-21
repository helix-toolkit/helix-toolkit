#ifndef VSMESHTESSELLATION_HLSL
#define VSMESHTESSELLATION_HLSL
#define MATERIAL
#include"..\Common\CommonBuffers.hlsl"
#include"..\Common\DataStructs.hlsl"
#pragma pack_matrix( row_major )

//--------------------------------------------------------------------------------------
// VERTEX SHADER function
// called per incoming vertex/control point
//--------------------------------------------------------------------------------------
HSInput main(VSInput input)
{
    HSInput output = (HSInput) 0;
    float4 inputp = input.p;
    float3 inputn = input.n;
    float3 inputt1 = input.t1;
    float3 inputt2 = input.t2;
    if (bInvertNormal)
    {
        inputn = -inputn;
    }
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
    output.p = inputp.xyz;
    output.t = input.t;
    output.n = inputn;
    output.t1 = inputt1;
    output.t2 = inputt2;
    output.c = input.c;
    output.c2 = vMaterialEmissive + vMaterialAmbient * vLightAmbient;
    return output;
}
#endif