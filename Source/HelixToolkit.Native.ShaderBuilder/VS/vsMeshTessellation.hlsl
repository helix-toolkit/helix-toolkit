#ifndef VSMESHTESSELLATION_HLSL
#define VSMESHTESSELLATION_HLSL

#define MESH
#include "..\Common\CommonBuffers.hlsl"
#include "..\Common\DataStructs.hlsl"
#pragma pack_matrix( row_major )

//--------------------------------------------------------------------------------------
// VERTEX SHADER function
// called per incoming vertex/control point
//--------------------------------------------------------------------------------------
#if !defined(INSTANCINGPARAM)
HSInput main(VSInput input)
#endif
#if defined(INSTANCINGPARAM)
HSInput main(VSInstancingInput input)
#endif
{
    HSInput output = (HSInput) 0;
	float4 inputp = mul(input.p, mWorld);
	float3 inputn = mul(input.n, (float3x3) mWorld);
	float3 inputt1 = mul(input.t1, (float3x3) mWorld);
	float3 inputt2 = mul(input.t2, (float3x3) mWorld);
    if (bInvertNormal)
    {
        inputn = -inputn;
    }

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
        if (bHasNormalMap)
        {
            if (!bAutoTengent)
            {
                inputt1 = mul(inputt1, (float3x3) mInstance);
                inputt2 = mul(inputt2, (float3x3) mInstance);
            }
        }
    }

#if !defined(INSTANCINGPARAM)
    output.c = input.c;
    output.t = mul(float2x4(uvTransformR1, uvTransformR2), float4(input.t, 0, 1)).xy;
    output.c2 = vMaterialEmissive;
#endif

#if defined(INSTANCINGPARAM)
    if (!bHasInstanceParams)
    {
        output.t = mul(float2x4(uvTransformR1, uvTransformR2), float4(input.t, 0, 1)).xy;
        output.c = vMaterialDiffuse;
        output.c2 = vMaterialEmissive;
    }
    else
    {
		//set texture coords and color
        output.t = mul(float2x4(uvTransformR1, uvTransformR2), float4(input.t, 0, 1)).xy + input.tOffset;
        output.c = input.diffuseC;
        output.c2 = input.emissiveC;
    }
#endif

    output.p = inputp.xyz;
    output.n = inputn;
    output.t1 = inputt1;
    output.t2 = inputt2;
    float tess = saturate((minTessDistance - distance(output.p, vEyePos)) / (minTessDistance - maxTessDistance));
    output.tessF = mad(tess, (maxTessFactor - minTessFactor), minTessFactor);
    return output;
}
#endif