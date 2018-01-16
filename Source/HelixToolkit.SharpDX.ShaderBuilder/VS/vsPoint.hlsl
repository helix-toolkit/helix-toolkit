#ifndef VSPOINT_HLSL
#define VSPOINT_HLSL
#define POINTLINE
#include"..\Common\DataStructs.hlsl"
#include"..\Common\CommonBuffers.hlsl"

GSInputPS main(VSInputPS input)
{
	GSInputPS output = (GSInputPS) 0;
	if (pHasInstances)
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
	output.p = mul(output.p, pWorld);
	output.p = mul(output.p, mViewProjection);
	output.c = input.c * pColor;
	return output;
}

#endif