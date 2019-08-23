#ifndef VSPOINTSHADOW_HLSL
#define VSPOINTSHADOW_HLSL

#define MESHSIMPLE
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

GSInputPS main(VSInputPS input)
{
    GSInputPS output = (GSInputPS) 0;
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
    }

    output.wp = mul(mWorld, input.p);

	//set position into clip space	
    output.p = mul(output.wp, vLightViewProjection);
    return output;
}

#endif