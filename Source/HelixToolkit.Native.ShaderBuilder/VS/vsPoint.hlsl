#ifndef VSPOINT_HLSL
#define VSPOINT_HLSL
#define POINTLINE
#include "..\Common\DataStructs.hlsl"
#include "..\Common\CommonBuffers.hlsl"
#include "..\Common\Common.hlsl"

GSInputPS main(VSInputPS input)
{
	GSInputPS output = (GSInputPS) 0;
	
	output.wp = mul(input.p, mWorld);

	if (bHasInstances)
	{
		matrix mInstance =
		{
			input.mr0,
			input.mr1,
			input.mr2,
			input.mr3
		};
		output.wp = mul(output.wp, mInstance);
	}	
	
    float3 vEye = vEyePos - output.wp.xyz;
    output.vEye = float4(normalize(vEye), length(vEye)); //Use wp for camera->vertex direction
	output.p = mul(output.wp, mViewProjection);
    // Allow to quickly change blending mode and do linear blending
    output.c = (1 - pEnableBlending) * input.c * pColor + pEnableBlending * (pBlendingFactor * input.c + (1 - pBlendingFactor) * pColor);

	
	return output;
}

#endif