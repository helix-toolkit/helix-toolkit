#include"..\Common\CommonBuffers.hlsl"

float4 main(MeshOutlinePS_INPUT input) : SV_Target
{
    return float4(texDiffuseMap.Sample(samplerDiffuse, input.Tex).r, 1, 1, 1);
}