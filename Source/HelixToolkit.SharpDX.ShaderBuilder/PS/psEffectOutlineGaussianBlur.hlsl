#define BORDEREFFECTS
#include"..\Common\CommonBuffers.hlsl"
#pragma pack_matrix( row_major )

static const float offset[5] = { 0.0, 1.0, 2.0, 3.0, 4.0 };
static const float weight[5] = { 0.2, 0.2, 0.2, 0.2, 0.2 };
static const float scale = 1;

float4 main(MeshOutlinePS_INPUT input) : SV_Target
{
    float a = texDiffuseMap.Sample(samplerDiffuse, input.Tex).r * weight[0];
    [unroll]
    for (int i = 1; i < 5; ++i)
    {
        float off = offset[i];
        float offX = off / vViewport.x * Param.x;
        float offY = off / vViewport.y * Param.y;
        a += (texDiffuseMap.Sample(samplerDiffuse, input.Tex + float2(0, offY)).r * weight[i]);
        a += (texDiffuseMap.Sample(samplerDiffuse, input.Tex - float2(0, offY)).r * weight[i]);
        a += (texDiffuseMap.Sample(samplerDiffuse, input.Tex + float2(offX, 0)).r * weight[i]);
        a += (texDiffuseMap.Sample(samplerDiffuse, input.Tex - float2(offX, 0)).r * weight[i]);

        a += (texDiffuseMap.Sample(samplerDiffuse, input.Tex + float2(offX, offY)).r * weight[i]);
        a += (texDiffuseMap.Sample(samplerDiffuse, input.Tex - float2(offX, offY)).r * weight[i]);
        a += (texDiffuseMap.Sample(samplerDiffuse, input.Tex + float2(offX, -offY)).r * weight[i]);
        a += (texDiffuseMap.Sample(samplerDiffuse, input.Tex - float2(offX, -offY)).r * weight[i]);
    }
    return float4(clamp(a, 0, 1), 1, 1, 1);

}