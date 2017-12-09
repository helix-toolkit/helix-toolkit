#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

PSInputBT main(VSInputBT input)
{
	PSInputBT output = (PSInputBT) 0;
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
		input.t.z *= input.mr0.x; // 2d scaling x
		input.t.w *= input.mr1.y; // 2d scaling y
	}
	float4 ndcPosition = float4(input.p.xyz, 1.0);

	// Translate position into clip space
	ndcPosition = mul(ndcPosition, mWorld);
	ndcPosition = mul(ndcPosition, mView);

    if (vParams.x == 0)// if not fixed size billboard
	{
		ndcPosition.xy += input.t.zw;
	}
	ndcPosition = mul(ndcPosition, mProjection);
	float4 ndcTranslated = ndcPosition / ndcPosition.w;

	if (vParams.x == 1)// if fixed sized billboard
	{
		// Translate offset into normalized device coordinates.
		float2 offset = windowToNdc(input.t.zw);
		ndcTranslated.xy += offset;
	}

	output.p = float4(ndcTranslated.xyz, 1.0);

	output.foreground = input.foreground;
    output.background = input.background;
	output.t = input.t.xy;
	return output;
}