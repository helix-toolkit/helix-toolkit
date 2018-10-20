#ifndef VSPOINT_HLSL
#define VSPOINT_HLSL
#define POINTLINE
#include"..\Common\DataStructs.hlsl"
#include"..\Common\CommonBuffers.hlsl"

GSInputPS main(VSInputPS input)
{
	GSInputPS output = (GSInputPS) 0;
	if (bHasInstances)
	{
		matrix mInstance =
		{
			input.mr0,
			input.mr1,
			input.mr2,
			input.mr3
		};
		input.p = mul(input.p, mInstance);
	}

	output.p = input.p;

	//set position into clip space	
	output.p = mul(output.p, mWorld);
    float3 vEye = vEyePos - output.p.xyz;
    output.vEye = float4(normalize(vEye), length(vEye)); //Use wp for camera->vertex direction
	output.p = mul(output.p, mViewProjection);
	output.c = input.c * pColor;
	return output;
}

#endif