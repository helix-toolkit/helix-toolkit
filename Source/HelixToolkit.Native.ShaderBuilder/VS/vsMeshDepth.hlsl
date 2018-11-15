#ifndef VSMESHDEPTH_HLSL
#define VSMESHDEPTH_HLSL

#define MESHSIMPLE
#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"

float4 main(VSInput input) : SV_Position
{
    float4 output = (float4) 0;
    output = input.p;
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
        output = mul(input.p, mInstance);
    }

	//set position into world space	
    output = mul(output, mul(mWorld, mViewProjection));
    return output;
}
#endif