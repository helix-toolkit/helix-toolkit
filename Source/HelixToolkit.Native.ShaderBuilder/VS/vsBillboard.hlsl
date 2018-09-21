#ifndef VSBILLBOARD_HLSL
#define VSBILLBOARD_HLSL
#define POINTLINE
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"
#pragma pack_matrix( row_major )
#if !defined(INSTANCINGPARAM)
GSInputBT main(VSInputBT input)
#endif
#if defined(INSTANCINGPARAM)
GSInputBT main(VSInputBTInstancing input)
#endif
{
    GSInputBT output = (GSInputBT) 0;
    output.p = input.p;
    output.background = input.background;
    output.foreground = input.foreground;
    output.t0 = input.t0;
    output.t3 = input.t3;
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
        output.p = mul(input.p, mInstance);
        output.offTL = input.offTL * mInstance._m00_m11; // 2d scaling x
        output.offBR = input.offBR * mInstance._m00_m11; // 2d scaling x
#if defined(INSTANCINGPARAM)
        if (bHasInstanceParams)
        {
            output.t0 = mad(input.tScale, input.t0, input.tOffset);
            output.t3 = mad(input.tScale, input.t3, input.tOffset);
            output.background = input.background * input.diffuseC;
        }
#endif
    }
	// Translate position into clip space
    float4 ndcPosition = mul(output.p, mWorld);
    output.p = mul(ndcPosition, mView);
    return output;
}

#endif