#ifndef VSPOINT_HLSL
#define VSPOINT_HLSL

#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

GSInputPS main(VSInputPS input)
{
	GSInputPS output = (GSInputPS) 0;
	if (bHasInstances)
	{
		matrix mInstance =
		{
			input.mr0.x, input.mr1.x, input.mr2.x, input.mr3.x, // row 1
			input.mr0.y, input.mr1.y, input.mr2.y, input.mr3.y, // row 2
			input.mr0.z, input.mr1.z, input.mr2.z, input.mr3.z, // row 3
			input.mr0.w, input.mr1.w, input.mr2.w, input.mr3.w, // row 4
		};
		input.p = mul(mInstance, input.p);
	}

	output.p = input.p;

	//set position into clip space	
	output.p = mul(output.p, mWorld);
	output.p = mul(output.p, mViewProjection);
	output.c = input.c * vColor;
	return output;
}

#endif