#define BORDEREFFECTS
#include"..\Common\CommonBuffers.hlsl"
#pragma pack_matrix( row_major )

float4 main(MeshOutlinePS_INPUT input) : SV_Target
{
    float4 color = texDiffuseMap.Sample(samplerSurface, input.Tex);
    float x = vResolution.z * Param._m00 * viewportScale;
    float y = vResolution.w * Param._m01 * viewportScale;

    float off = 0.7;
    float offX = off * x;
    float offY = off * y;
    float4 c = texDiffuseMap.Sample(samplerSurface, input.Tex + float2(offX, offY));
    c = max(c, texDiffuseMap.Sample(samplerSurface, input.Tex - float2(offX, offY)));
    c = max(c, texDiffuseMap.Sample(samplerSurface, input.Tex + float2(-offX, offY)));
    c = max(c, texDiffuseMap.Sample(samplerSurface, input.Tex - float2(-offX, offY)));
    color = max(color, c);
    return saturate(color);

}