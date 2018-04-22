#ifndef PSBILLBOARDTEXTOIT_HLSL
#define PSBILLBOARDTEXTOIT_HLSL
#define POINTLINE
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"
#include"psBillboardText.hlsl"

PSOITOutput billboardTextOIT(PSInputBT input)
{
    PSOITOutput output = (PSOITOutput) 0;
    float4 color = main(input);
    // Insert your favorite weighting function here. The color-based factor
    // avoids color pollution from the edges of wispy clouds. The z-based
    // factor gives precedence to nearer surfaces.
    float weight = color.a * clamp(0.03 / (1e-5 + pow((input.p.z / input.p.w), 4.0)), 1e-2, 3e3);
    // Blend Func: GL_ONE, GL_ONE
    // Switch to premultiplied alpha and weight
    output.color = float4(color.rgb * color.a, color.a) * weight;
 
    // Blend Func: GL_ZERO, GL_ONE_MINUS_SRC_ALPHA
    output.alpha.a = color.a;
    return output;
}

#endif