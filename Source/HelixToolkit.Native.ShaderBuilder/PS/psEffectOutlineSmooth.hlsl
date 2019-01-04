#define BORDEREFFECTS
#include"..\Common\CommonBuffers.hlsl"
#pragma pack_matrix( row_major )

static const int KSize = 5;
static const float offset[KSize] =
{
    0, 0.6, 1.0, 1.4, 2
};
static const float weight[KSize] =
{
    0.5, 0.19648255, 0.29690696, 0.09447040, 0.01038136
};

float4 main(MeshOutlinePS_INPUT input) : SV_Target
{
    float4 color = texDiffuseMap.Sample(samplerSurface, input.Tex) * 0.5;

    [unroll]
    for (int i = 1; i < KSize; ++i)
    {
        float offX = offset[i] * vViewport.z;
        float offY = offset[i] * vViewport.w;
        float4 c = texDiffuseMap.Sample(samplerSurface, input.Tex + float2(offX, offY));
        c += texDiffuseMap.Sample(samplerSurface, input.Tex + float2(-offX, offY));
        c += texDiffuseMap.Sample(samplerSurface, input.Tex + float2(offX, -offY));
        c += texDiffuseMap.Sample(samplerSurface, input.Tex + float2(-offX, -offY));
        color += c * weight[i];
        //a = mad(weight[i], c.a, a);
    }
    //color.a = a;
    return saturate(color);
}