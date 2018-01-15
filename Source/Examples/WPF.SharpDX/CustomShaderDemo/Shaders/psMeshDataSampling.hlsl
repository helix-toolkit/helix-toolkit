#include"Common.hlsl"

Texture1D texData : register(t0);
SamplerState texDataSampler : register(s0);

float4 main(PSInput input) : SV_Target
{
    return texData.Sample(texDataSampler, input.t.x);
}