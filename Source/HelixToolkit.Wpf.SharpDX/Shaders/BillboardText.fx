#ifndef BILLBOARDTEXT_FX
#define BILLBOARDTEXT_FX
#include "Common.fx"
#include "DataStructs.fx"
#include "Material.fx"
Texture2D billboardTexture; // billboard text image
Texture2D billboardAlphaTexture;
bool   bHasAlphaTexture = false;
bool   bHasTexture = false;
bool   bBillboardFixedSize = true;

//--------------------------------------------------------------------------------------
// GLOBAL FUNCTIONS
//--------------------------------------------------------------------------------------

//--------------------------------------------------------------------------------------
// From window pixel pos to normalized device coordinates.
//--------------------------------------------------------------------------------------
float2 windowToNdc(in float2 pos)
{
	return float2((pos.x / vViewport.x) * 2.0, (pos.y / vViewport.y) * 2.0);
}

//--------------------------------------------------------------------------------------
// BILLBOARD TEXT SHADER
//--------------------------------------------------------------------------------------

PSInputBT VShaderBillboardText( VSInputBT input )
{
	PSInputBT output = (PSInputBT)0;
	float4 ndcPosition = float4( input.p.xyz, 1.0 );

	// Translate position into clip space
	ndcPosition = mul( ndcPosition, mWorld );
	ndcPosition = mul( ndcPosition, mView );

	if (!bBillboardFixedSize) {
		ndcPosition.xy += input.t.zw;
	}
	ndcPosition = mul( ndcPosition, mProjection );
	float4 ndcTranslated = ndcPosition / ndcPosition.w;

	if (bBillboardFixedSize) {
		// Translate offset into normalized device coordinates.
		float2 offset = windowToNdc( input.t.zw );	
		ndcTranslated.xy += offset;
	} 

	output.p = float4( ndcTranslated.xyz, 1.0 );

	output.c = input.c;
	output.t = input.t.xy;
	return output;
}

PSInputBT VShaderBillboardInstancing(VSInputBTInstancing input)
{
	PSInputBT output = (PSInputBT)0;
	float4 inputp = input.p;
	float4 inputt = input.t;
	float4 inputc = input.c;
	if (bHasInstances)
	{
		inputp.xyz += input.mr3.xyz; //Translation
		if (bHasInstanceParams)
		{
			inputt.x *= input.tScale.x;
			inputt.y *= input.tScale.y;
			inputt.xy += input.tOffset;
			inputc *= input.diffuseC;
		}
	}

	float4 ndcPosition = float4(inputp.xyz, 1.0);
	input.t.z *= input.mr0.x; // 2d scaling x
	input.t.w *= input.mr1.y; // 2d scaling y
	// Translate position into clip space
	ndcPosition = mul(ndcPosition, mWorld);
	ndcPosition = mul(ndcPosition, mView);

	if (!bBillboardFixedSize) {
		ndcPosition.xy += input.t.zw;
	}

	ndcPosition = mul(ndcPosition, mProjection);

	float4 ndcTranslated = ndcPosition / ndcPosition.w;

	if (bBillboardFixedSize) {
		// Translate offset into normalized device coordinates.

		float2 offset = windowToNdc(input.t.zw);
		ndcTranslated.xy += offset;
	}

	output.p = float4(ndcTranslated.xyz, 1.0);

	output.c = inputc;
	output.t = inputt.xy;
	return output;
}

float4 PShaderBillboardText( PSInputBT input ) : SV_Target
{
	// Take the color off the texture, and use its red component as alpha.
	float4 pixelColor = billboardTexture.Sample(NormalSampler, input.t);
	float4 intermediateColor = float4(1.0, 1.0, 1.0, pixelColor.x);
	return intermediateColor * input.c;
}

float4 PShaderBillboardBackground(PSInputBT input) : SV_Target
{
	return input.c;
}

float4 PShaderBillboardImage(PSInputBT input) : SV_Target
{
	// Take the color off the texture using mask color
	float4 pixelColor = 1;
	if (bHasTexture)
	{
		pixelColor *= billboardTexture.Sample(PointSampler, input.t);
	}

	if (bHasAlphaTexture) 
	{
		pixelColor *= billboardAlphaTexture.Sample(PointSampler, input.t);
	}

	if(input.c.w != 0 && length(pixelColor - input.c) < 0.00001)
	{
		return float4(0.0, 0.0, 0.0, 0.0);
	}
	else
	{
		return pixelColor;	
	}
}
#endif