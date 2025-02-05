#ifndef VSMESHDEPTH_HLSL
#define VSMESHDEPTH_HLSL

#define MESHSIMPLE
#include "..\Common\Common.hlsl"
#include "..\Common\DataStructs.hlsl"

float4 main(float4 pos : POSITION0,
float4 mr0 : TEXCOORD1,
float4 mr1 : TEXCOORD2,
float4 mr2 : TEXCOORD3,
float4 mr3 : TEXCOORD4) : SV_Position
{
	float4 output = mul(pos, mWorld);
	// compose instance matrix
    if (bHasInstances)
    {
        matrix mInstance =
        {
            mr0,
			mr1,
			mr2,
			mr3
        };
		output = mul(output, mInstance);
	}

	//set position into world space	
    output = mul(output, mViewProjection);
    return output;
}
#endif