#ifndef VSBILLBOARDINSTANCING_HLSL
#define VSBILLBOARDINSTANCING_HLSL

#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"
#pragma pack_matrix( row_major )

VSInputBT main(VSInputBTInstancing input)
{
    VSInputBT output = (VSInputBT) 0;
    output.p = input.p;
	if (bHasInstances)
	{
		matrix mInstance =
		{
			input.mr0.x, input.mr1.x, input.mr2.x, input.mr3.x, // row 1
			input.mr0.y, input.mr1.y, input.mr2.y, input.mr3.y, // row 2
			input.mr0.z, input.mr1.z, input.mr2.z, input.mr3.z, // row 3
			input.mr0.w, input.mr1.w, input.mr2.w, input.mr3.w, // row 4
		};
		output.p = mul(mInstance, output.p);		
        output.p0 = input.p0 * mInstance._m00_m11; // 2d scaling x
        output.p1 = input.p1 * mInstance._m00_m11; // 2d scaling x
        output.p2 = input.p2 * mInstance._m00_m11; // 2d scaling x
        output.p3 = input.p3 * mInstance._m00_m11; // 2d scaling x

        if (bHasInstanceParams)
        {
            output.t0 = input.t0 * input.tScale + input.tOffset;
            output.t1 = input.t1 * input.tScale + input.tOffset;
            output.t2 = input.t2 * input.tScale + input.tOffset;
            output.t3 = input.t3 * input.tScale + input.tOffset;
            output.background = input.background * input.diffuseC;
        }
    }
	// Translate position into clip space
    float4 ndcPosition = mul(output.p, mWorld);
    output.p = mul(ndcPosition, mView);
	return output;
}

#endif