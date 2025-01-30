#ifndef VSPOINTSHADOW_HLSL
#define VSPOINTSHADOW_HLSL

#define MESHSIMPLE
#include "..\Common\DataStructs.hlsl"
#include "..\Common\Common.hlsl"

GSInputPS main(VSInputPS input)
{
    GSInputPS output = (GSInputPS) 0;
    output.wp = mul(input.p, mWorld);	
	
    if (bHasInstances)
    {
        matrix mInstance =
        {
            input.mr0,
			input.mr1,
			input.mr2,
			input.mr3
        };
		output.wp = mul(output.wp, mInstance);
	}


	//set position into clip space	
    output.p = mul(output.wp, mul(vLightView, vLightProjection));
    return output;
}

#endif