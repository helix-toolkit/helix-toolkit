#ifndef PSMESHPBR_HLSL
#define PSMESHPBR_HLSL

#define MESH
#define PBR

#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"
#include"psCommon.hlsl"

float3 calcNormal(PSInput input)
{
    float3 normal = normalize(input.n);
    if (bHasNormalMap)
    {
        float3 localNormal = BiasX2(texNormalMap.Sample(samplerSurface, input.t).xyz);
        normal = PeturbNormal(localNormal, input.wp.xyz, normal, input.t);
    }
    return normal;
}

float3 LightSurface(in float4 wp,
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
            float3 specular = Specular_BRDF(alpha, c_spec, NdotV, NdotL, LdotH, NdotH, N, H);
            // Directional light
            acc_color += NdotL * Lights[i].vLightColor.rgb * (((c_diff * diffuse_factor) + specular));
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
            float3 specular = Specular_BRDF(alpha, c_spec, NdotV, NdotL, LdotH, NdotH, N, H);
            float att = 1.0f / (Lights[i].vLightAtt.x + Lights[i].vLightAtt.y * dl + Lights[i].vLightAtt.z * dl * dl);
            acc_color = mad(att, NdotL * Lights[i].vLightColor.rgb * (((c_diff * diffuse_factor) + specular)), acc_color);
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
            acc_color = mad(att, NdotL * Lights[i].vLightColor.rgb * (((c_diff * diffuse_factor) + specular)), acc_color);
        }
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
    float3 color = (float3) 0;

    float4 albedo = float4(ConstantAlbedo.xyz, 1);
    // glTF2 defines metalness as B channel, roughness as G channel, and occlusion as R channel
    float3 RMA = float3(AO, ConstantRoughness, ConstantMetallic);
    if (bHasAlbedoMap)
    {
        albedo = texDiffuseMap.Sample(samplerSurface, input.t);
    }
    if (bHasRMAMap)
    {
        RMA = texRMAMap.Sample(samplerSurface, input.t).rgb;
    }

    color = LightSurface(input.wp, V, N, albedo.rgb, RMA.g, RMA.b, RMA.r);
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
    return float4(color, albedo.a * ConstantAlbedo.a);
}
#endif