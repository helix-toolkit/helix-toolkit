#ifndef PSLINE_HLSL
#define PSLINE_HLSL
#define POINTLINE
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

float4 main(PSInputPS input) : SV_Target
{
    // Compute distance of the fragment to the edges    
    //float dist = min(abs(input.t[0]), abs(input.t[1]));	
    float dist = abs(input.t.y);
    
    // Cull fragments too far from the edge.
    //if (dist > 0.5*vLineParams.x+1) discard;

    // Map the computed distance to the [0,2] range on the border of the line.
    //dist = clamp((dist - (0.5*vLineParams.x - 1)), 0, 2);

    // Alpha is computed from the function exp2(-2(x)^2).
    float sigma = 2.0f / (pfParams.y + 1e-6);
    dist *= dist;
    float alpha = exp2(-2 * dist / sigma);

	//if(alpha<0.1) discard;

    // Standard wire color
    float4 color = input.c;
	
	//color = texDiffuseMap.Sample(SSLinearSamplerWrap, input.t.xy);	
    color.a = alpha;
    if (enableDistanceFading)
    {
        color.a *= 1 - clamp((input.vEye.w - fadeNearDistance) / (fadeFarDistance - fadeNearDistance), 0, 1);
    }
    return color;
}

#endif