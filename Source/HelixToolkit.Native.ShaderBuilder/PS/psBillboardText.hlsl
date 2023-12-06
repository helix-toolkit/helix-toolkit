#ifndef PSBILLBOARDTEXT_HLSL
#define PSBILLBOARDTEXT_HLSL
#define POINTLINE
#include "..\Common\DataStructs.hlsl"
#include "..\Common\Common.hlsl"

static const uint BillboardSingleText = 1;
static const uint BillboardMultiText = 2;
static const uint BillboardImage = 4;

float4 main(PSInputBT input) : SV_Target
{
	// Take the color off the texture, and use its red component as alpha.
    float4 pixelColor = billboardTexture.Sample(samplerBillboard, input.t);
    float4 blend = input.foreground * pixelColor.r + input.background * (1 - pixelColor.r);

    float4 color = blend * whengt((when_eq(BillboardMultiText, (int) pfParams.x) + when_eq(BillboardSingleText, (int) pfParams.x)), 0)
    + pixelColor * any(when_neq(pixelColor, input.background)) * when_eq(BillboardImage, (int) pfParams.x);
    clip(color.a - 0.1);
    return color;
}

#endif