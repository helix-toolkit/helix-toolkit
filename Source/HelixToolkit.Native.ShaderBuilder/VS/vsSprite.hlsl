#ifndef VSSPRITE_HLSL
#define VSSPRITE_HLSL

#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

SpritePS_INPUT main(SpriteVS_INPUT input)
{
    SpritePS_INPUT output;
    output.Pos = mul(float4(input.Pos.xy, 0, 1), mProjection);
    output.Color = input.Color;
    output.UV = input.UV;
    return output;
}
#endif