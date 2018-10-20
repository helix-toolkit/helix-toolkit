#ifndef PSEFFECTMESHXRAYGRID_HLSL
#define PSEFFECTMESHXRAYGRID_HLSL

#define CLIPPLANE
#define MESH
#define BORDEREFFECTS

#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"
#include"psCommon.hlsl"
//--------------------------------------------------------------------------------------
// Blinn-Phong Lighting Reflection Model
//--------------------------------------------------------------------------------------
// Returns the sum of the diffuse and specular terms in the Blinn-Phong reflection model.
float4 calcBlinnPhongLighting(float4 LColor, float4 vMaterialTexture, float3 N, float4 diffuse, float3 L, float3 H)
{
    float4 Id = vMaterialTexture * diffuse * saturate(dot(N, L));
    return Id * LColor;
}

//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING - BLINN-PHONG
//--------------------------------------------------------------------------------------
float4 main(PSInput input) : SV_Target
{
	// renormalize interpolated vectors
    input.n = normalize(input.n);

    // get per pixel vector to eye-position
    float3 eye = input.vEye.xyz;

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
            DI = mad(att, calcBlinnPhongLighting(Lights[i].vLightColor, vMaterialTexture, input.n, input.cDiffuse, d, h), DI);
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
            DI = mad(att, calcBlinnPhongLighting(Lights[i].vLightColor, vMaterialTexture, input.n, input.cDiffuse, d, h), DI);
        }
    }
    DI.rgb *= s;
    I += DI;
    I.a = input.cDiffuse.a;
    float dimming = Param._m01;
    I.rgb *= dimming;
    int density = Param._m00;   
    float2 pixel = floor(input.p.xy);
    float a = 1;
    float b = fmod(abs(pixel.x - pixel.y), density);
    float c = fmod(abs(pixel.x + pixel.y), density);
    b = when_eq(b, 0);
    c = when_eq(c, 0);
    b = clamp(b + c, 0, 1);
    I = I * (1 - b) + (I * (1 - Param._m02) + Color * Param._m02) * b;
    return saturate(I);
}

#endif