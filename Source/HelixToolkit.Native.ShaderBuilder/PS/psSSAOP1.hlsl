//Ref: https://learnopengl.com/Advanced-Lighting/SSAO
//Ref: https://mynameismjp.wordpress.com/2009/03/10/reconstructing-position-from-depth/
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
    return float4(normalize(input.normal), 1);
}
#endif