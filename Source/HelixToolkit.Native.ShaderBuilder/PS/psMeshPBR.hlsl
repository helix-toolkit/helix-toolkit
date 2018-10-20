#ifndef PSMESHPBR_HLSL
#define PSMESHPBR_HLSL

#define MESH
#define PBR
#define CLEARCOAT

#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"
#include"psCommon.hlsl"

float3 calcNormal(PSInput input)
{
    float3 normal = normalize(input.n);
    if (bHasNormalMap)
    {
        if (bAutoTengent)
        {
            float3 localNormal = BiasX2(texNormalMap.Sample(samplerSurface, input.t).xyz);
            normal = PeturbNormal(localNormal, input.wp.xyz, normal, input.t);
        }
        else
        {
		    // Normalize the per-pixel interpolated tangent-space
            float3 tangent = normalize(input.t1);
            float3 biTangent = normalize(input.t2);

		    // Sample the texel in the bump map.
            float3 bumpMap = texNormalMap.Sample(samplerSurface, input.t);
		    // Expand the range of the normal value from (0, +1) to (-1, +1).
            bumpMap = mad(2.0f, bumpMap, -1.0f);
		    // Calculate the normal from the data in the bump map.
            normal += mad(bumpMap.x, tangent, bumpMap.y * biTangent);
            normal = normalize(normal);
        }
    }
    return normal;
}

float3 LightSurface(in float4 wp,
    in float3 V, in float3 N, in float3 albedo, in float roughness, in float metallic, in float ambientOcclusion, in float reflectance, in float clearCoat, in float clearCoatRoughness)
{
    const float NdotV = saturate(dot(N, V));

    // Burley roughness bias
    const float alpha = roughness * roughness;

    // Blend base colors
    const float3 c_diff = lerp(albedo, float3(0, 0, 0), metallic) * ambientOcclusion;
    const float3 c_spec = 0.16 * reflectance * reflectance * (1 - metallic) + albedo * metallic; //lerp(reflectance, albedo, metallic) * ambientOcclusion;
#if defined(CLEARCOAT)
    // remapping and linearization of clear coat roughness
    clearCoatRoughness = lerp(0.089, 0.6, clearCoatRoughness);
    float clearCoatLinearRoughness = clearCoatRoughness * clearCoatRoughness;
#endif
    // Output color
    float3 acc_color = 0;

    // Accumulate light values
    for (int i = 0; i < NumLights; i++)
    {


        if (Lights[i].iLightType == 1)
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
            float3 diffuse = c_diff * diffuse_factor;
            float3 specular = Specular_BRDF(alpha, c_spec, NdotV, NdotL, LdotH, NdotH, N, H);
          
#if defined(CLEARCOAT)
            float Dc = Filament_D_GGX(clearCoatLinearRoughness, NdotH, N, H);
            float Vc = V_Kelemen(LdotH);
            float Fc = Filament_F_Schlick(0.04, LdotH) * clearCoat; // clear coat strength
            float Frc = (Dc * Vc) * Fc;
            // Directional light
            acc_color += NdotL * Lights[i].vLightColor.rgb * ((diffuse + specular * (1 - Fc)) * (1 - Fc) + Frc);
#endif
#if !defined(CLEARCOAT)
            acc_color += NdotL * Lights[i].vLightColor.rgb * (diffuse + specular);
#endif
        }
        else if (Lights[i].iLightType == 2)
        {
            float3 L = (float3) (Lights[i].vLightPos - wp); // light dir
            float dl = length(L); // light distance
            if (Lights[i].vLightAtt.w < dl)
            {
                continue;
            }
            L = L / dl; // normalized light dir						
            const float3 H = normalize(V + L); // half direction for specular
            // products
            const float NdotL = saturate(dot(N, L));
            const float LdotH = saturate(dot(L, H));
            const float NdotH = saturate(dot(N, H));
            // Diffuse & specular factors
            float diffuse_factor = Diffuse_Burley(NdotL, NdotV, LdotH, roughness);
            float3 diffuse = c_diff * diffuse_factor;
            float3 specular = Specular_BRDF(alpha, c_spec, NdotV, NdotL, LdotH, NdotH, N, H);
            float att = 1.0f / (Lights[i].vLightAtt.x + Lights[i].vLightAtt.y * dl + Lights[i].vLightAtt.z * dl * dl);
#if defined(CLEARCOAT)
            float Dc = Filament_D_GGX(clearCoatLinearRoughness, NdotH, N, H);
            float Vc = V_Kelemen(LdotH);
            float Fc = Filament_F_Schlick(0.04, LdotH) * clearCoat; // clear coat strength
            float Frc = (Dc * Vc) * Fc;
            acc_color = mad(att, NdotL * Lights[i].vLightColor.rgb * ((diffuse + specular * (1 - Fc)) * (1 - Fc) + Frc), acc_color);
#endif
#if !defined(CLEARCOAT)
            acc_color = mad(att, NdotL * Lights[i].vLightColor.rgb * (diffuse + specular), acc_color);
#endif
        }
        else if (Lights[i].iLightType == 3)
        {
            float3 L = (float3) (Lights[i].vLightPos - wp); // light dir
            float dl = length(L); // light distance
            if (Lights[i].vLightAtt.w < dl)
            {
                continue;
            }
            L = L / dl; // normalized light dir					
            float3 H = normalize(V + L); // half direction for specular
            float3 sd = normalize((float3) Lights[i].vLightDir); // missuse the vLightDir variable for spot-dir

            const float NdotL = saturate(dot(N, L));
            const float LdotH = saturate(dot(L, H));
            const float NdotH = saturate(dot(N, H));
            // Diffuse & specular factors
            float diffuse_factor = Diffuse_Burley(NdotL, NdotV, LdotH, roughness);
            float3 diffuse = c_diff * diffuse_factor;
            float3 specular = Specular_BRDF(alpha, c_spec, NdotV, NdotL, LdotH, NdotH, N, H);
		    /* --- this is the OpenGL 1.2 version (not so nice) --- */
		    //float spot = (dot(-d, sd));
		    //if(spot > cos(vLightSpot[i].x))
		    //	spot = pow( spot, vLightSpot[i].y );
		    //else
		    //	spot = 0.0f;	
		    /* --- */

		    /* --- this is the  DirectX9 version (better) --- */
            float rho = dot(-L, sd);
            float spot = pow(saturate((rho - Lights[i].vLightSpot.x) / (Lights[i].vLightSpot.y - Lights[i].vLightSpot.x)), Lights[i].vLightSpot.z);
            float att = spot / (Lights[i].vLightAtt.x + Lights[i].vLightAtt.y * dl + Lights[i].vLightAtt.z * dl * dl);
#if defined(CLEARCOAT)
            float Dc = Filament_D_GGX(clearCoatLinearRoughness, NdotH, N, H);
            float Vc = V_Kelemen(LdotH);
            float Fc = Filament_F_Schlick(0.04, LdotH) * clearCoat; // clear coat strength
            float Frc = (Dc * Vc) * Fc;
            acc_color = mad(att, NdotL * Lights[i].vLightColor.rgb * ((diffuse + specular * (1 - Fc)) * (1 - Fc) + Frc), acc_color);
#endif
#if !defined(CLEARCOAT)
            acc_color = mad(att, NdotL * Lights[i].vLightColor.rgb * (diffuse + specular), acc_color);
#endif
        }
    }
    if (bHasIrradianceMap)
    {
        // Add diffuse irradiance
        float3 diffuse_env = Diffuse_IBL(N);
        acc_color += c_diff * diffuse_env;
    }
    float3 specular_env = vLightAmbient.rgb * ambientOcclusion;
#if defined(CLEARCOAT)
    float3 clearCoatColor = (float3) 0;
    float Fc = Filament_F_Schlick(0.04, NdotV) * clearCoat;
#endif
    if (bHasCubeMap)
    {
        // Add specular radiance 
        specular_env = Specular_IBL(N, V, roughness);
#if defined(CLEARCOAT)
        clearCoatColor = Specular_IBL(N, V, clearCoatRoughness) * Fc;
#endif
    }

    acc_color += c_spec * specular_env;

#if defined(CLEARCOAT)
    acc_color *= sqrt(1 - Fc);
    acc_color += clearCoatColor;
#endif
    return acc_color;
}

float4 main(PSInput input) : SV_Target
{
        // vectors
    const float3 V = normalize(input.vEye.xyz); // view vector

    float3 N = calcNormal(input);
    
    float3 color = (float3) 0;

    float4 albedo = float4(input.cDiffuse.xyz, 1);
    // glTF2 defines occlusion as R channel, roughness as G channel, metalness as B channel 
    float3 RMA = input.c2.rgb;
    if (bHasDiffuseMap)
    {
        albedo *= texDiffuseMap.Sample(samplerSurface, input.t);
    }
    if (bHasRMAMap)
    {
        float3 rmaSample = texRMAMap.Sample(samplerSurface, input.t).rgb;
        RMA.r = min(RMA.r, rmaSample.r);
        RMA.gb = max(RMA.gb, rmaSample.gb);
    }

    color = LightSurface(input.wp, V, N, albedo.rgb, RMA.g, RMA.b, RMA.r, input.c2.a, ClearCoat, ClearCoatRoughness);
    float s = 1;
    if (bHasShadowMap)
    {
        if (bRenderShadowMap)
            s = shadowStrength(input.sp);
    }
    color.rgb *= s;
    if (bHasEmissiveMap)
    {
        color += texEmissiveMap.Sample(samplerSurface, input.t).rgb;
    }
    color += vMaterialEmissive.rgb;
    return float4(color, albedo.a * input.cDiffuse.a);
}
#endif