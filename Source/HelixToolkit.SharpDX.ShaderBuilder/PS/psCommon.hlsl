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
    float2 xy = abs(sp).xy - float2(1, 1);
    
    if (xy.x > 0 || xy.y > 0 || sp.z < 0 || sp.z > 1)
    {
        return 1;
    }
    sp.x = mad(0.5, sp.x, 0.5f);
    sp.y = mad(-0.5, sp.y, 0.5f);

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

#define WeightModes_LinearA 0
#define WeightModes_LinearB 1
#define WeightModes_LinearC 2
#define WeightModes_NonLinear 3

//Ref http://jcgt.org/published/0002/02/09/
PSOITOutput calculateOIT(in float4 color, float z, float zw)
{
    PSOITOutput output = (PSOITOutput) 0;
    float weight = 1;
    z = z - vFrustum.z;
    if (OITWeightMode == WeightModes_LinearA)
        weight = max(0.01f, min(3000.0f, 100 / (0.00001f + pow(abs(z) / 5.0f, abs(OITPower)) + pow(abs(z) / 200.0f, abs(OITPower) * 2))));
    else if (OITWeightMode == WeightModes_LinearB)
        weight = max(0.01f, min(3000.0f, 100 / (0.00001f + pow(abs(z) / 10.0f, abs(OITPower)) + pow(abs(z) / 200.0f, abs(OITPower) * 2))));
    else if (OITWeightMode == WeightModes_LinearC)
        weight = max(0.01f, min(3000.0f, 0.3f / (0.00001f + pow(abs(z) / 200.0f, abs(OITPower)))));
    else if (OITWeightMode == WeightModes_NonLinear)
        weight = max(0.01f, 3e3 * pow(clamp(1.0f - zw * max(OITSlope, 1), 0, 1), abs(OITPower)));

    output.color = float4(color.rgb * color.a, color.a) * (color.a * weight);
        // Blend Func: GL_ZERO, GL_ONE_MINUS_SRC_ALPHA
    output.alpha.a = color.a;
    return output;
}
#endif