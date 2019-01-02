#ifndef VSSKYBOX_HLSL
#define VSSKYBOX_HLSL

#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"
const static float d = 1 / tan(3.1415926 / 4);

PSInputCube main(float3 input : SV_Position)
{
    PSInputCube output = (PSInputCube) 0;
    float4x4 viewNoTranslate = mView;
    viewNoTranslate._m30_m31_m32 = 0;
    float aspect = vViewport.x / vViewport.y;   
    float4x4 proj = float4x4(d / aspect, 0, 0, 0, 0, d, 0, 0, 0, 0, -1, -1, 0, 0, -1, 0);
    //Set w = 0 to make sure depth is infinite. Must disable depth clipping
    output.p = mul(mul(float4(input, 0), viewNoTranslate), proj);
    output.t = input;
    output.c = float4(1, 1, 1, 1);
    return output;
}

#endif