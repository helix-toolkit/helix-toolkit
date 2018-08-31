#define BORDEREFFECTS
#include"..\Common\CommonBuffers.hlsl"
#pragma pack_matrix( row_major )

static const float offset[2] = { 0, 1 };
static const float scale = 1;

float4 main(MeshOutlinePS_INPUT input) : SV_Target
{
    float4 color = texDiffuseMap.Sample(samplerSurface, input.Tex);
    float x = vViewport.z * Param._m00;
    float y = vViewport.w * Param._m01;
    [unroll]
    for (int i = 1; i < 2; ++i)
    {
        float off = offset[i];
        float offX = off * x;
        float offY = off * y;
        float4 c = texDiffuseMap.Sample(samplerSurface, input.Tex + float2(offX, offY));
        c = max(c, texDiffuseMap.Sample(samplerSurface, input.Tex - float2(offX, offY)));
        c = max(c, texDiffuseMap.Sample(samplerSurface, input.Tex + float2(-offX, offY)));
        color = max(c, texDiffuseMap.Sample(samplerSurface, input.Tex - float2(-offX, offY)));
    }
    return saturate(color);

}