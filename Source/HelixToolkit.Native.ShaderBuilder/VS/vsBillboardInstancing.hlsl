#ifndef VSBILLBOARDINSTANCING_HLSL
#define VSBILLBOARDINSTANCING_HLSL
#define POINTLINE
#define INSTANCINGPARAM
#include "vsBillboard.hlsl"
#pragma pack_matrix( row_major )

GSInputBT mainInstancing(VSInputBTInstancing input)
{
    GSInputBT output = (GSInputBT) 0;
	output.p = mul(input.p, mWorld);
	output.t0 = input.t0;
	output.t3 = input.t3;
    output.offBR = input.offBR;
    output.offTL = input.offTL;
    output.offBL = input.offBL;
    output.offTR = input.offTR;

	
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
        output.offTL *= mInstance._m00_m11; // 2d scaling x
        output.offBR *= mInstance._m00_m11; // 2d scaling x
        output.offBL *= mInstance._m00_m11;
        output.offTR *= mInstance._m00_m11;
        if (bHasInstanceParams)
        {
            output.t0 = mad(input.tScale, input.t0, input.tOffset);
            output.t3 = mad(input.tScale, input.t3, input.tOffset);
            output.background = input.background * input.diffuseC;
        }
    }

	output.p = mul(output.p, mView);
	return output;
}

#endif