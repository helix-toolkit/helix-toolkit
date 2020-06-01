#ifndef PSPOINT_HLSL
#define PSPOINT_HLSL
#define POINTLINE
#include"DataStructs.hlsl"
#include"Common.hlsl"

cbuffer CustomBuffer
{
    float3 random_color;
    float custom_buffer_padding;
};


float4 main(PSInputPS input) : SV_Target
{
    if (pfParams[2] == 1)
    {
        float len = length(input.t);
        if (len > 1.4142)
            discard;
    }
    else if (pfParams[2] == 2)
    {
        float arrowScale = 1 / (pfParams[3] + (input.t[2] > 0.9) * (-input.t[2] + 1) * 10);
        float alpha = min(abs(input.t[0]), abs(input.t[1]));
        float dist = arrowScale * alpha;
        alpha = exp2(-4 * dist * dist * dist * dist);
        if (alpha < 0.1)
            discard;
    }
    float3 color = input.c.rgb * random_color;
    return float4(saturate(color), 1);
}

#endif