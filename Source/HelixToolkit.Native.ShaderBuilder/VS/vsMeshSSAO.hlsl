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

SSAOIn main(float4 pos : POSITION0,
float3 normal : NORMAL,
float4 mr0 : TEXCOORD1,
float4 mr1 : TEXCOORD2,
float4 mr2 : TEXCOORD3,
float4 mr3 : TEXCOORD4)
{
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
        pos = mul(pos, mInstance);
        normal = mul(float4(normal, 0), mInstance).xyz;
    }
    float4 pv = mul(pos, mul(mWorld, mView));
    SSAOIn output = (SSAOIn) 0;
	//set position into world space	
    output.pos = mul(pos, mul(mWorld, mViewProjection));
    output.normal = normalize(mul(float4(normal, 0), mul(mWorld, mViewProjection)).xyz);
    output.depth = pv.z;
    return output;
}
#endif