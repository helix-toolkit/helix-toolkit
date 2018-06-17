#define BORDEREFFECTS
#include"..\Common\CommonBuffers.hlsl"

float4 main(MeshOutlinePS_INPUT input) : SV_Target
{
    return saturate(texDiffuseMap.Sample(samplerDiffuse, input.Tex));
}