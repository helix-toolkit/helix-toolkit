#include"..\Common\CommonBuffers.hlsl"

float4 main(MeshOutlinePS_INPUT input) : SV_Target
{
    return texDiffuseMap.Sample(samplerSurface, input.Tex);
}