#ifndef VSMESHOUTLINEP1_HLSL
#define VSMESHOUTLINEP1_HLSL

#define MESH
#include "..\Common\Common.hlsl"
#include "..\Common\DataStructs.hlsl"
#include "vsMeshDefault.hlsl"
#pragma pack_matrix( row_major )

float4 VSMeshOutlineP1(VSInput input) : SV_POSITION
{
    PSInput output = (PSInput) 0;
	output.p = mul(input.p + float4(input.n * 0.1f, 0), mWorld);
	output.n = normalize(mul(input.n, (float3x3) mWorld));

    if (bInvertNormal)
    {
		output.n = -output.n;
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
		output.p = mul(output.p, mInstance);
		output.n = normalize(mul(output.n, (float3x3) mInstance));
	}
	

    if (bHasDisplacementMap)
    {
        float2 t = mul(float2x4(uvTransformR1, uvTransformR2), float4(input.t, 0, 1)).xy;
        const float mipInterval = 20;
        float mipLevel = clamp((distance(output.p.xyz, vEyePos) - mipInterval) / mipInterval, 0, 6);
        float3 h = texDisplacementMap.SampleLevel(samplerDisplace, t, mipLevel);
        output.p.xyz += output.n * mul(h, displacementMapScaleMask.xyz);
    }
	//set position into clip space	
    output.p = mul(output.p, mViewProjection);
    return output.p;
}

#endif