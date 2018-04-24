#ifndef PSWIREFRAMEOIT_HLSL
#define PSWIREFRAMEOIT_HLSL

#define MESH
#include"..\Common\CommonBuffers.hlsl"

PSOITOutput wireframeOIT(float4 pos : SV_POSITION)
{
    PSOITOutput output = (PSOITOutput) 0;
    float4 color = wireframeColor;
    // Insert your favorite weighting function here. The color-based factor
    // avoids color pollution from the edges of wispy clouds. The z-based
    // factor gives precedence to nearer surfaces.
    float weight = color.a * clamp(0.03 / (1e-5 + pow((pos.z / pos.w), 4.0)), 1e-2, 3e3);
    // Blend Func: GL_ONE, GL_ONE
    // Switch to premultiplied alpha and weight
    output.color = float4(color.rgb * color.a, color.a) * weight;
 
    // Blend Func: GL_ZERO, GL_ONE_MINUS_SRC_ALPHA
    output.alpha.a = color.a;
    return output;
}
#endif