#ifndef VSSKYBOX_HLSL
#define VSSKYBOX_HLSL

#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

PSInputCube main(float3 input : SV_Position)
{
    PSInputCube output = (PSInputCube) 0;
    float4x4 viewNoTranslate = mView;
    viewNoTranslate._m30_m31_m32 = 0;
    output.p = mul(mul(float4(input, 1), viewNoTranslate), mProjection);
    output.t = input;
    output.c = float4(1, 1, 1, 1);
    return output;
}

#endif