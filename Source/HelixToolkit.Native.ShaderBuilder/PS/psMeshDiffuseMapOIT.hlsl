#ifndef PSMESHDIFFUSEMAPOIT_HLSL
#define PSMESHDIFFUSEMAPOIT_HLSL

#define CLIPPLANE
#define MESH

#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"
#include"psCommon.hlsl"
#include"psDiffuseMap.hlsl"
//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING - BLINN-PHONG for Order Independant Transparent A-buffer rendering
// http://casual-effects.blogspot.com/2014/03/weighted-blended-order-independent.html
//--------------------------------------------------------------------------------------

PSOITOutput meshDiffuseOIT(PSInput input)
{
    float4 color = main(input);
    if (bRenderOIT)
    {
        return calculateOIT(color, input.vEye.w, input.p.z);
        //// Insert your favorite weighting function here. The color-based factor
        //// avoids color pollution from the edges of wispy clouds. The z-based
        //// factor gives precedence to nearer surfaces.
        //float weight = color.a * clamp(0.03 / (1e-5 + pow((input.p.z/input.p.w), 4.0)), 1e-2, 3e3);
        //// Blend Func: GL_ONE, GL_ONE
        //// Switch to premultiplied alpha and weight
        //output.color = float4(color.rgb * color.a, color.a) * weight;
 
        //// Blend Func: GL_ZERO, GL_ONE_MINUS_SRC_ALPHA
        //output.alpha.a = color.a;
    }
    else
    {
        PSOITOutput output = (PSOITOutput) 0;
        output.color = color;
        return output;
    }
}
#endif