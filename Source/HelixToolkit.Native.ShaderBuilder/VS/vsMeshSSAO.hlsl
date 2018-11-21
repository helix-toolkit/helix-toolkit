#ifndef VSMESHSSAO_HLSL
#define VSMESHSSAO_HLSL

#define MESHSIMPLE
#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"

struct SSAOIn
{
    float4 pos : SV_POSITION;
    float depth : TEXCOORD0;
    float3 normal : NORMAL;
};
#if defined(BATCHED)
SSAOIn main(VSInputBatched input)
#else
SSAOIn main(VSInput input)
#endif
{
    #if !defined(BATCHED)
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
        input.p = mul(input.p, mInstance);
        input.n = mul(input.n, (float3x3) mInstance);
    }
    #endif

    float4 pv = mul(input.p, mul(mWorld, mView));
    SSAOIn output = (SSAOIn) 0;
	//set position into world space	
    output.pos = mul(input.p, mul(mWorld, mViewProjection));
    output.normal = normalize(mul(input.n, (float3x3) mWorld));
    output.depth = pv.z;
    return output;
}
#endif