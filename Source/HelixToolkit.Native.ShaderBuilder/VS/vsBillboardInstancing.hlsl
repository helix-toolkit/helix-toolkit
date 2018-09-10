#ifndef VSBILLBOARDINSTANCING_HLSL
#define VSBILLBOARDINSTANCING_HLSL
#define POINTLINE
#define INSTANCINGPARAM
#include"vsBillboard.hlsl"
#pragma pack_matrix( row_major )

GSInputBT mainInstancing(VSInputBTInstancing input)
{
    GSInputBT output = (GSInputBT) 0;
    output.p = input.p;
    output.offBR = input.offBR;
    output.offTL = input.offTL;
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

        if (bHasInstanceParams)
        {
            output.t0 = mad(input.tScale, input.t0, input.tOffset);
            output.t3 = mad(input.tScale, input.t3, input.tOffset);
            output.background = input.background * input.diffuseC;
        }
    }
	// Translate position into clip space
    float4 ndcPosition = mul(output.p, mWorld);
    output.p = mul(ndcPosition, mView);
	return output;
}

#endif