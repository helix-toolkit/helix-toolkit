#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

float4 main(PSInputPS input) : SV_Target
{
    if (vParams[2] == 1)
    {
        float len = length(input.t);
        if (len > 1.4142)
            discard;
    }
    else if (vParams[2] == 2)
    {
        float arrowScale = 1 / (vParams[3] + (input.t[2] > 0.9) * (-input.t[2] + 1) * 10);
        float alpha = min(abs(input.t[0]), abs(input.t[1]));
        float dist = arrowScale * alpha;
        alpha = exp2(-4 * dist * dist * dist * dist);
        if (alpha < 0.1)
            discard;
    }

    return input.c;
}