#include"..\Common\CommonBuffers.hlsl"

float4 main(MeshOutlinePS_INPUT input) : SV_Target
{
    return float4(CrossSectionColors.rgb, texDiffuseMap.Sample(samplerDiffuse, input.Tex).r);
}