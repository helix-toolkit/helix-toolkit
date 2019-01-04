#define SSAO
#include"..\Common\CommonBuffers.hlsl"
#pragma pack_matrix( row_major )

static const float div = 1.0 / 16;
float4 main(MeshOutlinePS_INPUT input) : SV_Target
{
    float2 texSize = vViewport.zw * texScale;
    float result = 0;
    [unroll]
    for (int x = -2; x < 2; ++x)
    {
        [unroll]
        for (int y = -2; y < 2; ++y)
        {
            float2 off = float2(x, y) * texSize;
            result += texSSAOMap.Sample(samplerSurface, input.Tex + off).r;
        }
    }
    return float4(result * div, 0, 0, 0);
}