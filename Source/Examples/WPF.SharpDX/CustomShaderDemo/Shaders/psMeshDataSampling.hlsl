#include"Common.hlsl"
#include"CommonBuffers.hlsl"
Texture1D texData : register(t0);
SamplerState texDataSampler : register(s0);


float4 main(PSInput input) : SV_Target
{    
    float4 vMaterialTexture = texData.Sample(texDataSampler, input.t.x);
    return vMaterialTexture;
}