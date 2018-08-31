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
    float4 color = texDiffuseMap.Sample(samplerSurface, input.Tex);
    float a = color.a * weight[0];
    float k = vViewport.w * Param._m01;
    [unroll]
    for (int i = 1; i < KSize; ++i)
    {
        float offY = offset[i] * k;
        float4 c = texDiffuseMap.Sample(samplerSurface, input.Tex + float2(0, offY));
        c += texDiffuseMap.Sample(samplerSurface, input.Tex - float2(0, offY));
        color += c;
        a = mad(weight[i], c.a, a);
    }
    color.a = a;
    return saturate(color);
}