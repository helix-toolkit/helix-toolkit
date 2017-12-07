#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Material.hlsl"
#include"psCommon.hlsl"

//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING - BLINN-PHONG
//--------------------------------------------------------------------------------------
float4 main(PSInput input) : SV_Target
{    
	// renormalize interpolated vectors
    input.n = calcNormal(input);

    // get per pixel vector to eye-position
    float3 eye = normalize(vEyePos - input.wp.xyz);

    // light emissive intensity and add ambient light
    float4 I = vMaterialEmissive + vMaterialAmbient * vLightAmbient;

    // get shadow color
    float s = 1;
    if (bHasShadowMap)
    {
        s = shadowStrength(input.sp);
    }

    // add diffuse sampling
    float4 vMaterialTexture = 1.0f;
    if (bHasDiffuseMap)
    {
	    // SamplerState is defined in Common.fx.
        vMaterialTexture *= texDiffuseMap.Sample(LinearSampler, input.t);
    }

    float alpha = 1;
    if (bHasAlphaMap)
    {
        float4 color = texAlphaMap.Sample(LinearSampler, input.t);
        alpha = color[3];
        color[3] = 1;
        vMaterialTexture *= color;
    }

    // compute lighting
    for (int i = 0; i < LIGHTS; ++i)
    {
        if (Lights[i].iLightType == 0)
        {
            break;
        }
	    ///If light color is not enable, skip	
		// Same as for the Phong PixelShader, but use
	    // calcBlinnPhongLighting instead.	
        if (!Lights[i].bLightEnable)
        {
            continue;
        }
        else if (Lights[i].iLightType == 1) // directional
        {
            float3 d = normalize((float3) Lights[i].vLightDir); // light dir	
            float3 h = normalize(eye + d);
            I += s * calcBlinnPhongLighting(Lights[i].vLightColor, vMaterialTexture, input.n, vMaterialDiffuse, d, h);
        }
        else if (Lights[i].iLightType == 2)  // point
        {
            float3 d = (float3) (Lights[i].vLightPos - input.wp); // light dir
            float dl = length(d); // light distance
            d = d / dl; // normalized light dir						
            float3 h = normalize(eye + d); // half direction for specular
            float att = 1.0f / (Lights[i].vLightAtt.x + Lights[i].vLightAtt.y * dl + Lights[i].vLightAtt.z * dl * dl);
            I += att * calcBlinnPhongLighting(Lights[i].vLightColor, vMaterialTexture, input.n, vMaterialDiffuse, d, h);
        }
        else if (Lights[i].iLightType == 3)  // spot
        {
            float3 d = (float3) (Lights[i].vLightPos - input.wp); // light dir
            float dl = length(d); // light distance
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
            I += att * calcBlinnPhongLighting(Lights[i].vLightColor, vMaterialTexture, input.n, vMaterialDiffuse, d, h);
        }
    }
    I.a = vMaterialDiffuse.a;
    if (bHasAlphaMap)
    {
        I.a *= alpha;
    }
	// multiply by vertex colors
    I = I * input.c;
    // get reflection-color
    if (bHasCubeMap)
    {
        I = cubeMapReflection(input, I);
    }

    return I;
}

