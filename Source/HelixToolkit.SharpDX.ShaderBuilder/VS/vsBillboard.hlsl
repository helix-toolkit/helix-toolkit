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
		input.p0.x *= input.mr0.x; // 2d scaling x
		input.p0.y *= input.mr1.y; // 2d scaling y

        input.p1.x *= input.mr0.x; // 2d scaling x
        input.p1.y *= input.mr1.y; // 2d scaling y

        input.p2.x *= input.mr0.x; // 2d scaling x
        input.p2.y *= input.mr1.y; // 2d scaling y

        input.p3.x *= input.mr0.x; // 2d scaling x
        input.p3.y *= input.mr1.y; // 2d scaling y
	}
	// Translate position into clip space
    float4 ndcPosition = mul(input.p, mWorld);
    input.p = mul(ndcPosition, mView);
	return input;
}