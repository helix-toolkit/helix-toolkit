#ifndef PSBILLBOARDTEXT_HLSL
#define PSBILLBOARDTEXT_HLSL

#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

Texture2D billboardTexture; // billboard text image

static const uint BillboardSingleText = 1;
static const uint BillboardMultiText = 2;
static const uint BillboardImage = 4;

float4 main(PSInputBT input) : SV_Target
{
	// Take the color off the texture, and use its red component as alpha.
    float4 pixelColor = billboardTexture.Sample(NormalSampler, input.t);
    float4 blend = input.foreground * pixelColor.x + input.background * (1 - pixelColor.x);
    return blend * whengt(((BillboardMultiText & (uint) vParams.x) | (BillboardSingleText & (uint) vParams.x)), 0) + pixelColor * whengt((BillboardImage & (uint) vParams.x), 0);
}

#endif