#define BORDEREFFECTS
#include"..\Common\CommonBuffers.hlsl"
#pragma pack_matrix( row_major )

static const int KSize = 5;
static const float offset[KSize] =
{
    0, 1, 2, 3, 4
};
static const float weight[KSize] =
{
    0.20236, 0.179044, 0.124009, 0.067234, 0.028532
};

float4 main(MeshOutlinePS_INPUT input) : SV_Target
{
    float4 color = texDiffuseMap.Sample(samplerSurface, input.Tex) * weight[0];
    float k = vResolution.z * Param._m00 * viewportScale;
    [unroll]
    for (int i = 1; i < KSize; ++i)
    {
        float offX = offset[i] * k;
        float4 c = texDiffuseMap.Sample(samplerSurface, input.Tex + float2(offX, 0));
        c += texDiffuseMap.Sample(samplerSurface, input.Tex - float2(offX, 0));
        color += c * weight[i];
    }
    return saturate(color);
}