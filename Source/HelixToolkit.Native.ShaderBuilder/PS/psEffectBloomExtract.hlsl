#define BORDEREFFECTS
#include"..\Common\CommonBuffers.hlsl"
#pragma pack_matrix( row_major )

// Pixel shader: bloom (extract)
float4 psBloomExtract(MeshOutlinePS_INPUT pin) : SV_Target0
{
    float4 c = texDiffuseMap.Sample(samplerSurface, pin.Tex);
    return saturate((c - Color) / (1 - Color) * Param._m00); //Color as threshold
}