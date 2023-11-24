//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------
#include "CommonBuffers.hlsl"
float4 main(ScreenDupVS_INPUT input) : SV_Target
{
    float3 color = texAlphaMap.Sample(samplerSurface, input.Tex).rgb;
        // reinhard tone mapping
    float3 mapped = color / (color + float3(1.0, 1.0, 1.0));
    // gamma correction 
    float gamma = 1.2;
    mapped = pow(abs(mapped), float3(1.0 / gamma, 1.0 / gamma, 1.0 / gamma));
  
    return saturate(float4(mapped, 1.0));
}