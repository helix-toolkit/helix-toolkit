#ifndef SSAOEP1PASS
#define SSAOEP1PASS

#include"..\Common\CommonBuffers.hlsl"
#pragma pack_matrix( row_major )

float4 main(float depth : TEXCOORD0, float3 normal : NORMAL) : SV_TARGET
{
    float fd = -depth / vFrustum.w;
    float4 output = (float4) 0;
    output.a = float4(fd, 1, 1, 1);
    output.rgb = float4(normalize(normal), 1);
    return output;
}
#endif