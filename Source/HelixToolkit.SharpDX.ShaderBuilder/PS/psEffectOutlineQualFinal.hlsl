#define BORDEREFFECTS
#include"..\Common\CommonBuffers.hlsl"

float4 main(MeshOutlinePS_INPUT input) : SV_Target
{
    return float4(Color.rgb, texDiffuseMap.Sample(samplerDiffuse, input.Tex).r);
}