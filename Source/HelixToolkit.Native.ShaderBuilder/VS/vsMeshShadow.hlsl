#ifndef VSMESHSHADOW_HLSL
#define VSMESHSHADOW_HLSL

#define MESHSIMPLE
#include "..\Common\Common.hlsl"
#include "..\Common\DataStructs.hlsl"
#pragma pack_matrix( row_major )

PSShadow main(VSInput input)
{
    PSShadow output = (PSShadow)0;
	output.p = mul(input.p, mWorld);
	// compose instance matrix
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
    }

	//set position into world space	
    output.p = mul(output.p, mul(vLightView, vLightProjection));
    return output;
}
#endif