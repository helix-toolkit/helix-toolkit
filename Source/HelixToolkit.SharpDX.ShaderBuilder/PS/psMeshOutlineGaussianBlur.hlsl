#define MESH
#include"..\Common\CommonBuffers.hlsl"

static const float offset[5] = { 0.0, 1.0, 2.0, 3.0, 4.0 };
static const float weight[5] = { 0.2, 0.2, 0.2, 0.2, 0.2 };
static const float scale = 1;

float4 main(MeshOutlinePS_INPUT input) : SV_Target
{
    float4 color = texDiffuseMap.Sample(samplerDiffuse, input.Tex) * weight[0];
    float a = color.a;
    [unroll]
    for (int i = 1; i < 5; ++i)
    {
        float off = offset[i];
        float offX = off / vViewport.x * scale;
        float offY = off / vViewport.y * scale;
        a += (texDiffuseMap.Sample(samplerDiffuse, input.Tex + float2(0, offY)) * weight[i]).a;
        a += (texDiffuseMap.Sample(samplerDiffuse, input.Tex - float2(0, offY)) * weight[i]).a;
        a += (texDiffuseMap.Sample(samplerDiffuse, input.Tex + float2(offX, 0)) * weight[i]).a;
        a += (texDiffuseMap.Sample(samplerDiffuse, input.Tex - float2(offX, 0)) * weight[i]).a;

        a += (texDiffuseMap.Sample(samplerDiffuse, input.Tex + float2(offX, offY)) * weight[i]).a;
        a += (texDiffuseMap.Sample(samplerDiffuse, input.Tex - float2(offX, offY)) * weight[i]).a;
        a += (texDiffuseMap.Sample(samplerDiffuse, input.Tex + float2(offX, -offY)) * weight[i]).a;
        a += (texDiffuseMap.Sample(samplerDiffuse, input.Tex - float2(offX, -offY)) * weight[i]).a;
    }
    return float4(CrossSectionColors.xyz, a);

}