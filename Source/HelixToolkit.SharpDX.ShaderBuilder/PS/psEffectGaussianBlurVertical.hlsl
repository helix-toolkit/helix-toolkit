#define BORDEREFFECTS
#include"..\Common\CommonBuffers.hlsl"
#pragma pack_matrix( row_major )

static const float offset[3] = { 0.0, 1.3846153846, 3.2307692308 };
static const float weight[3] = { 0.2270270270, 0.3162162162, 0.0702702703 };

float4 main(MeshOutlinePS_INPUT input) : SV_Target
{
    float4 color = texDiffuseMap.Sample(samplerDiffuse, input.Tex) * weight[0];
    [unroll]
    for (int i = 1; i < 3; ++i)
    {
        float offY = offset[i] / vViewport.y * Param.y;
        color += (texDiffuseMap.Sample(samplerDiffuse, input.Tex + float2(0, offY)) * weight[i]);
        color += (texDiffuseMap.Sample(samplerDiffuse, input.Tex - float2(0, offY)) * weight[i]);
    }
    return saturate(color);

}