#ifndef PSEFFECTMESHDIFFUSEXRAYGRID_HLSL
#define PSEFFECTMESHDIFFUSEXRAYGRID_HLSL

#define CLIPPLANE
#define MESH
#define BORDEREFFECTS

#include"..\Common\Common.hlsl"
#include"psDiffuseMap.hlsl"

//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING - BLINN-PHONG
//--------------------------------------------------------------------------------------
float4 mainXRayGrid(PSInput input) : SV_Target
{
    float4 I = main(input);
    float dimming = Param._m01;
    I.rgb *= dimming;
    int density = Param._m00;
    float2 pixel = floor(input.p.xy);
    float a = 1;
    float b = fmod(abs(pixel.x - pixel.y), density);
    float c = fmod(abs(pixel.x + pixel.y), density);
    b = when_eq(b, 0);
    c = when_eq(c, 0);
    b = clamp(b + c, 0, 1);
    I = I * (1 - b) + (I * (1 - Param._m02) + Color * Param._m02) * b;
    return saturate(I);
}

#endif