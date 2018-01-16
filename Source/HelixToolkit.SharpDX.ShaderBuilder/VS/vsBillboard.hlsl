#ifndef VSBILLBOARD_HLSL
#define VSBILLBOARD_HLSL
#define POINTLINE
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"
#pragma pack_matrix( row_major )

VSInputBT main(VSInputBT input)
{
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
        input.offTL *= mInstance._m00_m11; // 2d scaling x
        input.offBR *= mInstance._m00_m11; // 2d scaling x
    }
	// Translate position into clip space
    float4 ndcPosition = mul(input.p, pWorld);
    input.p = mul(ndcPosition, mView);
	return input;
}

#endif