//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------
#include"..\Common\CommonBuffers.hlsl"
float4 main(ScreenDupVS_INPUT input) : SV_Target
{
    return texDiffuseMap.Sample(samplerSurface, input.Tex);
}