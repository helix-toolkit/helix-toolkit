#define MATERIAL
#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"
#pragma pack_matrix( row_major )

float4 main(VSInput input) : SV_Position
{
    float4 inputp = input.p;
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
        inputp = mul(input.p, mInstance);
    }

	//set position into world space	
    inputp = mul(inputp, mWorld);
    inputp = mul(inputp, vLightViewProjection);
    return inputp;
}