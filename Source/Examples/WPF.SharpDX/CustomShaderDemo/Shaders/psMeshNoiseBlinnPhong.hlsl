#ifndef PSMESHBLINNPHONG_HLSL
#define PSMESHBLINNPHONG_HLSL

#define CLIPPLANE
#define MESH

#include"Common.hlsl"
#include"DataStructs.hlsl"
#include"PerlinNoise3D.hlsl"

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
        float3 bumpMap = texNormalMap.Sample(samplerSurface, input.t);
		// Expand the range of the normal value from (0, +1) to (-1, +1).
        bumpMap = (bumpMap * 2.0f) - 1.0f;
		// Calculate the normal from the data in the bump map.
        input.n = input.n + bumpMap.x * input.t1 + bumpMap.y * input.t2;
    }
    return normalize(input.n);
}


//--------------------------------------------------------------------------------------
// Blinn-Phong Lighting Reflection Model
//--------------------------------------------------------------------------------------
// Returns the sum of the diffuse and specular terms in the Blinn-Phong reflection model.
float4 calcBlinnPhongLighting(float4 LColor, float4 vMaterialTexture, float3 N, float4 diffuse, float3 L, float3 H)
{
    float4 Id = vMaterialTexture * diffuse * saturate(dot(N, L));
    float4 Is = vMaterialSpecular * pow(saturate(dot(N, H)), sMaterialShininess);
    return (Id + Is) * LColor;
}


//--------------------------------------------------------------------------------------
// reflectance mapping
//--------------------------------------------------------------------------------------
float3 cubeMapReflection(PSInput input, const in float3 I, const in float3 reflectColor)
{
    float3 v = normalize((float3) input.wp - vEyePos);
    float3 r = reflect(v, input.n);
    return (1.0f - reflectColor) * I + reflectColor * texCubeMap.Sample(samplerCube, r);
}

static const float epsilon = 0.0001f;

float3 noiseVector(float3 v)
{
    	// renormalize interpolated vectors
    float ns = cnoise(v);
    float nsx = cnoise(v + float3(epsilon, 0, 0));
    float nsy = cnoise(v + float3(0, epsilon, 0));
    float nsz = cnoise(v + float3(0, 0, epsilon));
    return float3((nsx - ns) / epsilon, (nsy - ns) / epsilon, (nsz - ns) / epsilon);
}
//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING - BLINN-PHONG
//--------------------------------------------------------------------------------------
float4 main(PSInput input) : SV_Target
{    
    // renormalize interpolated vectors
    input.n = normalize(input.n - noiseVector(input.wp.xyz) * 0.1f);

    input.n = calcNormal(input);

    // get per pixel vector to eye-position
    float3 eye = normalize(vEyePos - input.wp.xyz);

    // light emissive intensity and add ambient light
    float4 I = input.c2;

    // get shadow color
    float s = 1;

    // add diffuse sampling
    float4 vMaterialTexture = 1.0f;
    if (bHasDiffuseMap)
    {
	    // SamplerState is defined in Common.fx.
        vMaterialTexture *= texDiffuseMap.Sample(samplerSurface, input.t);
    }
    float4 reflectColor = vMaterialReflect;
    float alpha = 1;
    if (bHasAlphaMap)
    {
        float4 color = texAlphaMap.Sample(samplerSurface, input.t);
        alpha = color[3];
        color[3] = 1;
        vMaterialTexture *= color;
    }
    float4 DI = float4(0, 0, 0, 0);
    // compute lighting
    for (int i = 0; i < NumLights; ++i)
    {
        if (Lights[i].iLightType == 1) // directional
        {
            float3 d = normalize((float3) Lights[i].vLightDir); // light dir	
            float3 h = normalize(eye + d);
            DI += calcBlinnPhongLighting(Lights[i].vLightColor, vMaterialTexture, input.n, input.cDiffuse, d, h);
        }
        else if (Lights[i].iLightType == 2)  // point
        {
            float3 d = (float3) (Lights[i].vLightPos - input.wp); // light dir
            float dl = length(d); // light distance
            if (Lights[i].vLightAtt.w < dl)
            {
                continue;
            }
            d = d / dl; // normalized light dir						
            float3 h = normalize(eye + d); // half direction for specular
            float att = 1.0f / (Lights[i].vLightAtt.x + Lights[i].vLightAtt.y * dl + Lights[i].vLightAtt.z * dl * dl);
            DI += att * calcBlinnPhongLighting(Lights[i].vLightColor, vMaterialTexture, input.n, input.cDiffuse, d, h);
        }
        else if (Lights[i].iLightType == 3)  // spot
        {
            float3 d = (float3) (Lights[i].vLightPos - input.wp); // light dir
            float dl = length(d); // light distance
            if (Lights[i].vLightAtt.w < dl)
            {
                continue;
            }
            d = d / dl; // normalized light dir					
            float3 h = normalize(eye + d); // half direction for specular
            float3 sd = normalize((float3) Lights[i].vLightDir); // missuse the vLightDir variable for spot-dir

														    /* --- this is the OpenGL 1.2 version (not so nice) --- */
														    //float spot = (dot(-d, sd));
														    //if(spot > cos(vLightSpot[i].x))
														    //	spot = pow( spot, vLightSpot[i].y );
														    //else
														    //	spot = 0.0f;	
														    /* --- */

														    /* --- this is the  DirectX9 version (better) --- */
            float rho = dot(-d, sd);
            float spot = pow(saturate((rho - Lights[i].vLightSpot.x) / (Lights[i].vLightSpot.y - Lights[i].vLightSpot.x)), Lights[i].vLightSpot.z);
            float att = spot / (Lights[i].vLightAtt.x + Lights[i].vLightAtt.y * dl + Lights[i].vLightAtt.z * dl * dl);
            DI += att * calcBlinnPhongLighting(Lights[i].vLightColor, vMaterialTexture, input.n, input.cDiffuse, d, h);
        }
    }
    DI.rgb *= s;
    I += DI;
    I.a = input.cDiffuse.a;
    if (bHasAlphaMap)
    {
        I.a *= alpha;
    }
	// multiply by vertex colors
    //I = I * input.c;
    // get reflection-color
    if (bHasCubeMap)
    {
        I.rgb = cubeMapReflection(input, I.rgb, reflectColor.rgb);
    }

    return I;
}

#endif