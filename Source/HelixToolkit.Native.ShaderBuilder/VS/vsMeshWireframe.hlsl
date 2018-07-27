#ifndef VSMESHWIREFRAME_HLSL
#define VSMESHWIREFRAME_HLSL

#define MESH
#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"
#pragma pack_matrix( row_major )

PSWireframeInput main(VSInput input)
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
        float2 t = mul(float2x4(uvTransformR1, uvTransformR2), float4(input.t, 0, 1)).xy;
	    //set normal for interpolation	
        inputn = normalize(mul(inputn, (float3x3) mWorld));
        const float mipInterval = 20;
        float mipLevel = clamp((distance(inputp.xyz, vEyePos) - mipInterval) / mipInterval, 0, 6);
        float4 h = texDisplacementMap.SampleLevel(samplerDisplace, t, mipLevel);
        inputp.xyz += inputn * mul(h, displacementMapScaleMask);
    }
    PSWireframeInput output = (PSWireframeInput)0;
	//set position into clip space	
    output.p = mul(inputp, mViewProjection);
    output.z = length(vEyePos - inputp.xyz);
    return output;
}

#endif