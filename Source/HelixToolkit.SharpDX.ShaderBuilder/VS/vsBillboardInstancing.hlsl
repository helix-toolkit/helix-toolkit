#ifndef VSBILLBOARDINSTANCING_HLSL
#define VSBILLBOARDINSTANCING_HLSL
#define POINTLINE
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"
#pragma pack_matrix( row_major )

VSInputBT main(VSInputBTInstancing input)
{
    VSInputBT output = (VSInputBT) 0;
    output.p = input.p;
    output.offBR = input.offBR;
    output.offTL = input.offTL;
	if (pHasInstances)
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

        if (pHasInstanceParams)
        {
            output.t0 = input.t0 * input.tScale + input.tOffset;
            output.t3 = input.t3 * input.tScale + input.tOffset;
            output.background = input.background * input.diffuseC;
        }
    }
	// Translate position into clip space
    float4 ndcPosition = mul(output.p, pWorld);
    output.p = mul(ndcPosition, mView);
	return output;
}

#endif