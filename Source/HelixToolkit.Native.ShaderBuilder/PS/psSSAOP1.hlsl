#ifndef SSAOEP1PASS
#define SSAOEP1PASS

#include"..\Common\CommonBuffers.hlsl"
#pragma pack_matrix( row_major )
struct SSAOIn
{
    float4 pos : SV_POSITION;
    float depth : TEXCOORD0;
    float3 normal : NORMAL;
};

float4 main(SSAOIn input) : SV_TARGET
{
    float fd = -input.depth / vFrustum.w;
    return float4(normalize(input.normal), fd);
}
#endif