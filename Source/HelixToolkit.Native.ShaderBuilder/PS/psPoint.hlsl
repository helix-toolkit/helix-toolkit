#ifndef PSPOINT_HLSL
#define PSPOINT_HLSL
#define POINTLINE
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

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
    float alpha = 1;
    if (enableDistanceFading)
    {
        input.c.a = 1 - clamp((input.vEye.w - fadeNearDistance) / (fadeFarDistance - fadeNearDistance), 0, 1);
    }
    return input.c;
}

#endif