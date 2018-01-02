#ifndef VSBILLBOARD_HLSL
#define VSBILLBOARD_HLSL

#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"
#pragma pack_matrix( row_major )

VSInputBT main(VSInputBT input)
{
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
        input.p0 = input.p0 * mInstance._m00_m11; // 2d scaling x
        input.p1 = input.p1 * mInstance._m00_m11; // 2d scaling x
        input.p2 = input.p2 * mInstance._m00_m11; // 2d scaling x
        input.p3 = input.p3 * mInstance._m00_m11; // 2d scaling x
    }
	// Translate position into clip space
    float4 ndcPosition = mul(input.p, mWorld);
    input.p = mul(ndcPosition, mView);
	return input;
}

#endif