#define BORDEREFFECTS
#include"..\Common\CommonBuffers.hlsl"
#pragma pack_matrix( row_major )
static const int SIZE = 2;
static const float offset[SIZE] = { 0, 1 };
static const float weight[SIZE] =
{
    0.5, 0.5
};
static const float scale = 1;

float4 main(MeshOutlinePS_INPUT input) : SV_Target
{
    float4 color = texDiffuseMap.Sample(samplerSurface, input.Tex) * weight[0];
    float x = vViewport.z * Param._m00 * viewportScale;
    float y = vViewport.w * Param._m01 * viewportScale;
    [unroll]
    for (int i = 1; i < SIZE; ++i)
    {
        float off = offset[i];
        float offX = off * x;
        float offY = off * y;
        float4 c = texDiffuseMap.Sample(samplerSurface, input.Tex + float2(offX, offY));
        c = max(c, texDiffuseMap.Sample(samplerSurface, input.Tex - float2(offX, offY)));
        c = max(c, texDiffuseMap.Sample(samplerSurface, input.Tex + float2(-offX, offY)));
        c = max(c, texDiffuseMap.Sample(samplerSurface, input.Tex - float2(-offX, offY)));
        color += max(color, c) * weight[i];
    }
    return saturate(color);

}