#ifndef PSCOMMON_HLSL
#define PSCOMMON_HLSL

#define MESH
#include"..\Common\Common.hlsl"

float lookUp(in float4 loc, in float2 offset)
{
    return texShadowMap.SampleCmpLevelZero(samplerShadow, loc.xy + offset, loc.z);
}

//--------------------------------------------------------------------------------------
// get shadow color
//--------------------------------------------------------------------------------------
float shadowStrength(float4 sp)
{
    sp = sp / sp.w;
    if (sp.x < -1.0f || sp.x > 1.0f || sp.y < -1.0f || sp.y > 1.0f || sp.z < 0.0f || sp.z > 1.0f)
    {
        return 1;
    }
    sp.x = sp.x / 2 + 0.5f;
    sp.y = sp.y / -2 + 0.5f;

	//apply shadow map bias
    sp.z -= vShadowMapInfo.z;

	//// --- not in shadow, hard cut
    //float shadowMapDepth = texShadowMap.Sample(PointSampler, sp.xy+offsets[1]).r;
    //return whengt(shadowMapDepth, sp.z);

	//// --- basic hardware PCF - single texel
    //float shadowFactor = texShadowMap.SampleCmpLevelZero(samplerShadow, sp.xy, sp.z).r;

	//// --- PCF sampling for shadow map
    float sum = 0;
    float x = 0, y = 0;
    const float range = 1.5;
    float2 scale = 1 / vShadowMapSize;

	//// ---perform PCF filtering on a 4 x 4 texel neighborhood
	[unroll]
    for (y = -range; y <= range; y += 1.0f)
    {
        for (x = -range; x <= range; x += 1.0f)
        {
            sum += lookUp(sp, float2(x, y) * scale);
        }
    }

    float shadowFactor = sum / 16;

    float fixTeil = vShadowMapInfo.x;
    float nonTeil = 1 - vShadowMapInfo.x;
	// now, put the shadow-strengh into the 0-nonTeil range
    nonTeil = shadowFactor * nonTeil;
    return (fixTeil + nonTeil);
}

#endif