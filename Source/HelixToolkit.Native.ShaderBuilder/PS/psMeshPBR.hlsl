#ifndef PSMESHPBR_HLSL
#define PSMESHPBR_HLSL

#define MESH
#define PBR

#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"
#include"psCommon.hlsl"
//--------------------------------------------------------------------------------------
// normal mapping
//--------------------------------------------------------------------------------------
// This function returns the normal in world coordinates.
// The input struct contains tangent (t1), bitangent (t2) and normal (n) of the
// unperturbed surface in world coordinates. The perturbed normal in tangent space
// can be read from texNormalMap.
// The RGB values in this texture need to be normalized from (0, +1) to (-1, +1).
float3 calcNormal(PSInput input)
{
    if (bHasNormalMap)
    {
		// Normalize the per-pixel interpolated tangent-space
        input.n = normalize(input.n);
        input.t1 = normalize(input.t1);
        input.t2 = normalize(input.t2);

		// Sample the texel in the bump map.
        float4 bumpMap = texNormalMap.Sample(SurfaceSampler, input.t);
		// Expand the range of the normal value from (0, +1) to (-1, +1).
        bumpMap = mad(2.0f, bumpMap, -1.0f);
		// Calculate the normal from the data in the bump map.
        input.n += mad(bumpMap.x, input.t1, bumpMap.y * input.t2);
    }
    return normalize(input.n);
}

float3 LightSurface(
    in float3 V, in float3 N, in float3 albedo, in float roughness, in float metallic, in float ambientOcclusion)
{
    // Specular coefficiant - fixed reflectance value for non-metals
    static const float kSpecularCoefficient = 0.04;

    const float NdotV = saturate(dot(N, V));

    // Burley roughness bias
    const float alpha = roughness * roughness;

    // Blend base colors
    const float3 c_diff = lerp(albedo, float3(0, 0, 0), metallic) * ambientOcclusion;
    const float3 c_spec = lerp(kSpecularCoefficient, albedo, metallic) * ambientOcclusion;

    // Output color
    float3 acc_color = 0;

    // Accumulate light values
    for (int i = 0; i < NumLights; i++)
    {
        // light vector (to light)
        const float3 L = normalize(Lights[i].vLightDir.xyz);

        // Half vector
        const float3 H = normalize(L + V);

        // products
        const float NdotL = saturate(dot(N, L));
        const float LdotH = saturate(dot(L, H));
        const float NdotH = saturate(dot(N, H));

        // Diffuse & specular factors
        float diffuse_factor = Diffuse_Burley(NdotL, NdotV, LdotH, roughness);
        float3 specular = Specular_BRDF(alpha, c_spec, NdotV, NdotL, LdotH, NdotH);

        // Directional light
        acc_color += NdotL * Lights[i].vLightColor.rgb * (((c_diff * diffuse_factor) + specular));
    }
    if (bHasIrradianceMap)
    {
        // Add diffuse irradiance
        float3 diffuse_env = Diffuse_IBL(N);
        acc_color += c_diff * diffuse_env;
    }

    if (bHasCubeMap)
    {
        // Add specular radiance 
        float3 specular_env = Specular_IBL(N, V, roughness);
        acc_color += c_spec * specular_env;
    }
    return acc_color;
}

float4 main(PSInput input) : SV_Target
{
        // vectors
    const float3 V = normalize(input.vEye.xyz); // view vector

    float3 N = calcNormal(input);
    
    const float AO = 1; // ambient term
    float alpha = ConstantAlbedo.a;
    float3 color = (float3) 0;

    float4 albedo = ConstantAlbedo;
    // glTF2 defines metalness as B channel, roughness as G channel, and occlusion as R channel
    float3 RMA = float3(AO, ConstantRoughness, ConstantMetallic);
    if (bHasAlbedoMap)
    {
        albedo = texDiffuseMap.Sample(SurfaceSampler, input.t);
    }
    if (bHasRMAMap)
    {
        RMA = texRMAMap.Sample(SurfaceSampler, input.t).rgb;
    }

    color = LightSurface(V, N, ConstantAlbedo.rgb, RMA.g, RMA.b, RMA.r);
    float s = 1;
    if (bHasShadowMap)
    {
        if (bRenderShadowMap)
            s = shadowStrength(input.sp);
    }
    color.rgb *= s;
    if (bHasEmissiveMap)
    {
        color += texEmissiveMap.Sample(SurfaceSampler, input.t).rgb;
    }
    return float4(color, albedo.a * alpha);
}
#endif