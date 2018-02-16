#ifndef VSMESHWIREFRAME_HLSL
#define VSMESHWIREFRAME_HLSL

#define MESH
#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"
#pragma pack_matrix( row_major )

float4 main(VSInput input) : SV_Position
{
    float4 inputp = input.p;
    float3 inputn = input.n;

    if (bInvertNormal)
    {
        inputn = -inputn;
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
        inputp = mul(input.p, mInstance);
        inputn = mul(inputn, (float3x3) mInstance);
    }

	//set position into world space	
    inputp = mul(inputp, mWorld);
	
    if (bHasDisplacementMap)
    {
	    //set normal for interpolation	
        inputn = normalize(mul(inputn, (float3x3) mWorld));
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