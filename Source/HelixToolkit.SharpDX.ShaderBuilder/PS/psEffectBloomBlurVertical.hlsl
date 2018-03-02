#define BORDEREFFECTS
#include"..\Common\CommonBuffers.hlsl"
#pragma pack_matrix( row_major )

static const int KSize = 4;
static const float offset[KSize] =
{
    0.0, 1.4117647059, 3.2941176471, 5.176470588
};
static const float weight[KSize] =
{
    0.19648255, 0.29690696, 0.09447040, 0.01038136
};

float4 main(MeshOutlinePS_INPUT input) : SV_Target
{
    float4 color = texDiffuseMap.Sample(samplerDiffuse, input.Tex)* weight[0];
    [unroll]
    for (int i = 1; i < KSize; ++i)
    {
        float offY = offset[i] / vViewport.y;
        float4 c = texDiffuseMap.Sample(samplerDiffuse, input.Tex + float2(0, offY));
        c += texDiffuseMap.Sample(samplerDiffuse, input.Tex - float2(0, offY));
        color += c * weight[i];
    }
    return color * Param._m01;
}