
#define MESH
#include"Common.hlsl"
#include"CommonBuffers.hlsl"

Texture1D texData : register(t0);
SamplerState texDataSampler : register(s0);

//--------------------------------------------------------------------------------------
// Blinn-Phong Lighting Reflection Model
//--------------------------------------------------------------------------------------
// Returns the sum of the diffuse and specular terms in the Blinn-Phong reflection model.
float4 calcBlinnPhongLighting(float4 LColor, float4 vMaterialTexture, float3 N, float4 diffuse, float3 L, float3 H)
{
    return LColor * pow(0.5- saturate(dot(N, H)), 4);
}

float4 main(PSInput input) : SV_Target
{    
    float4 vMaterialTexture = texData.Sample(texDataSampler, input.t.x);
    // renormalize interpolated vectors
    input.n = normalize(input.n);

    // get per pixel vector to eye-position
    float3 eye = normalize(vEyePos - input.wp.xyz);
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
    }
    DI.a = 1; 
    return DI + vMaterialTexture;
}