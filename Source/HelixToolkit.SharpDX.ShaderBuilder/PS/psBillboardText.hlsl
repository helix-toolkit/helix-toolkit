#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"
SamplerState NormalSampler
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};
Texture2D billboardTexture : register(t0); // billboard text image

float4 main(PSInputBT input) : SV_Target
{
	// Take the color off the texture, and use its red component as alpha.
    float4 pixelColor = billboardTexture.Sample(NormalSampler, input.t);
    return input.foreground * pixelColor.x + input.background * (1 - pixelColor.x);
}