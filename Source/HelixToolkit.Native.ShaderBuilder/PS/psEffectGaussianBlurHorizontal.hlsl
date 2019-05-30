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
    0.2270270270, 0.1945945946, 0.1216216216, 0.0540540541, 0.0162162162
};

float4 main(MeshOutlinePS_INPUT input) : SV_Target
{
    float4 color = texDiffuseMap.Sample(samplerSurface, input.Tex) * weight[0];
    float k = vViewport.z * Param._m00 * viewportScale;
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