#define BORDEREFFECTS
#include"..\Common\CommonBuffers.hlsl"
#pragma pack_matrix( row_major )

static const float offset[4] = { 0.0, 1.0, 2.0, 3.0 };
static const float weight[4] = { 0.2, 0.2, 0.2, 0.2 };
static const float scale = 1;

float4 main(MeshOutlinePS_INPUT input) : SV_Target
{
    float4 color = texDiffuseMap.Sample(samplerDiffuse, input.Tex);
    float a = color.a * weight[0];
    float x = vViewport.z * Param._m00;
    float y = vViewport.w * Param._m01;
    [unroll]
    for (int i = 1; i < 4; ++i)
    {
        float off = offset[i];
        float offX = off * x;
        float offY = off * y;
        float4 c = texDiffuseMap.Sample(samplerDiffuse, input.Tex + float2(offX, offY));
        c += texDiffuseMap.Sample(samplerDiffuse, input.Tex - float2(offX, offY));
        c += texDiffuseMap.Sample(samplerDiffuse, input.Tex + float2(-offX, offY));
        c += texDiffuseMap.Sample(samplerDiffuse, input.Tex - float2(-offX, offY));
        color += c;
        a = mad(weight[i], c.a, a);
    }
    color.a = a;
    return saturate(color);

}