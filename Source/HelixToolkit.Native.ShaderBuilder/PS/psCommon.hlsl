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
    else
    {
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

#if defined(PBR)
static const float PI = 3.14159265f;
static const float EPSILON = 1e-6f;

// Shlick's approximation of Fresnel
// https://en.wikipedia.org/wiki/Schlick%27s_approximation
float3 Fresnel_Shlick(in float3 f0, in float3 f90, in float x)
{
    return f0 + (f90 - f0) * pow(1.f - x, 5);
}

float Filament_F_Schlick(float f0, float VoH) {
    float f = pow(1.0 - VoH, 5.0);
    return f + f0 * (1.0 - f);
}

// https://google.github.io/filament/Filament.md.html#toc4.4
float3 Filament_F_Schlick(float3 f0, float VoH) {
    float f = pow(1.0 - VoH, 5.0);
    return f + f0 * (1.0 - f);
}

// Burley B. "Physically Based Shading at Disney"
// SIGGRAPH 2012 Course: Practical Physically Based Shading in Film and Game Production, 2012.
float Diffuse_Burley(in float NdotL, in float NdotV, in float LdotH, in float roughness)
{
    return Filament_F_Schlick(1, NdotL).x * Filament_F_Schlick(1, NdotV).x;
    //float fd90 = 0.5f + 2.f * roughness * LdotH * LdotH;
    //return Fresnel_Shlick(1, fd90, NdotL).x * Fresnel_Shlick(1, fd90, NdotV).x;
}

// GGX specular D (normal distribution)
// https://www.cs.cornell.edu/~srm/publications/EGSR07-btdf.pdf
float Specular_D_GGX(in float alpha, in float NdotH)
{
    const float alpha2 = alpha * alpha;
    const float lower = (NdotH * NdotH * (alpha2 - 1)) + 1;
    return alpha2 / max(EPSILON, PI * lower * lower);
}

#define MEDIUMP_FLT_MAX    65504.0
#define saturateMediump(x) min(x, MEDIUMP_FLT_MAX)
// https://google.github.io/filament/Filament.md.html#toc4.4
float Filament_D_GGX(in float linearRoughness, in float NoH, in float3 n, in float3 h) {
    float3 NxH = cross(n, h);
    float a = NoH * linearRoughness;
    float k = linearRoughness / (dot(NxH, NxH) + a * a);
    float d = k * k * (1.0 / PI);
    return saturateMediump(d);
}

// Schlick-Smith specular G (visibility) with Hable's LdotH optimization
// http://www.cs.virginia.edu/~jdl/bib/appearance/analytic%20models/schlick94b.pdf
// http://graphicrants.blogspot.se/2013/08/specular-brdf-reference.html
float G_Shlick_Smith_Hable(float alpha, float LdotH)
{
    return rcp(lerp(LdotH * LdotH, 1, alpha * alpha * 0.25f));
}

float V_Kelemen(float LoH) {
    return 0.25 / (LoH * LoH);
}
// A microfacet based BRDF.
//
// alpha:           This is roughness * roughness as in the "Disney" PBR model by Burley et al.
//
// specularColor:   The F0 reflectance value - 0.04 for non-metals, or RGB for metals. This follows model 
//                  used by Unreal Engine 4.
//
// NdotV, NdotL, LdotH, NdotH: vector relationships between,
//      N - surface normal
//      V - eye normal
//      L - light normal
//      H - half vector between L & V.
float3 Specular_BRDF(in float alpha, in float3 specularColor, in float NdotV, in float NdotL, in float LdotH, in float NdotH, in float3 N, in float3 H)
{
    // Specular D (microfacet normal distribution) component
    float specular_D = Filament_D_GGX(alpha, NdotH, N, H);//Specular_D_GGX(alpha, NdotH);

    // Specular Fresnel
    float3 specular_F = Filament_F_Schlick(specularColor, LdotH);//Fresnel_Shlick(specularColor, 1, LdotH);

    // Specular G (visibility) component
    float specular_G = G_Shlick_Smith_Hable(alpha, LdotH);

    return specular_D * specular_G * specular_F;
}

// Diffuse irradiance
float3 Diffuse_IBL(in float3 N)
{
    return texIrradianceMap.Sample(samplerIBL, N).rgb;
}

// Approximate specular image based lighting by sampling radiance map at lower mips 
// according to roughness, then modulating by Fresnel term. 
float3 Specular_IBL(in float3 N, in float3 V, in float lodBias)
{
    float mip = lodBias * NumRadianceMipLevels;
    float3 dir = reflect(-V, N);
    return texCubeMap.SampleLevel(samplerIBL, dir, mip).rgb;
}
#endif
// Christian Schüler, "Normal Mapping without Precomputed Tangents", ShaderX 5, Chapter 2.6, pp. 131 – 140
// See also follow-up blog post: http://www.thetenthplanet.de/archives/1180
float3x3 CalculateTBN(float3 p, float3 n, float2 tex)
{
    float3 dp1 = ddx(p);
    float3 dp2 = ddy(p);
    float2 duv1 = ddx(tex);
    float2 duv2 = ddy(tex);

    float3x3 M = float3x3(dp1, dp2, cross(dp1, dp2));
    float2x3 inverseM = float2x3(cross(M[1], M[2]), cross(M[2], M[0]));
    float3 t = normalize(mul(float2(duv1.x, duv2.x), inverseM));
    float3 b = normalize(mul(float2(duv1.y, duv2.y), inverseM));
    return float3x3(t, b, n);
}
float3 BiasX2(float3 x)
{
    return 2.0f * x - 1.0f;
}

float3 BiasD2(float3 x)
{
    return 0.5f * x + 0.5f;
}

float3 PeturbNormal(float3 localNormal, float3 position, float3 normal, float2 texCoord)
{
    const float3x3 TBN = CalculateTBN(position, normal, texCoord);
    return normalize(mul(localNormal, TBN));
}
#endif