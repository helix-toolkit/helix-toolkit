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
			input.mr0.x, input.mr1.x, input.mr2.x, input.mr3.x, // row 1
			input.mr0.y, input.mr1.y, input.mr2.y, input.mr3.y, // row 2
			input.mr0.z, input.mr1.z, input.mr2.z, input.mr3.z, // row 3
			input.mr0.w, input.mr1.w, input.mr2.w, input.mr3.w, // row 4
		};
		input.p = mul(mInstance, input.p);
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