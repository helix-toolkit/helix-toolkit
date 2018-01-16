#ifndef PSBILLBOARDTEXT_HLSL
#define PSBILLBOARDTEXT_HLSL
#define POINTLINE
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

static const uint BillboardSingleText = 1;
static const uint BillboardMultiText = 2;
static const uint BillboardImage = 4;

float4 main(PSInputBT input) : SV_Target
{
	// Take the color off the texture, and use its red component as alpha.
    float4 pixelColor = billboardTexture.Sample(samplerBillboard, input.t);
    float4 blend = input.foreground * pixelColor.x + input.background * (1 - pixelColor.x);
    return blend * whengt(((BillboardMultiText & (uint) pfParams.x) | (BillboardSingleText & (uint) pfParams.x)), 0) + pixelColor * whengt((BillboardImage & (uint) pfParams.x), 0);
}

#endif