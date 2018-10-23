#ifndef PSSPRITE_HLSL
#define PSSPRITE_HLSL

#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

float4 main(SpritePS_INPUT input) : SV_Target
{
    float4 out_col = input.Color * texSprite.Sample(samplerSprite, input.UV);
    return out_col;
}
#endif