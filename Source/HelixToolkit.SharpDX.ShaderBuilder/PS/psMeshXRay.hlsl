#include"..\Common\DataStructs.hlsl"

cbuffer cbXRay : register(b0)
{
    float4 XRayObjectColor = float4(1, 1, 1, 1);
    float XRayBorderFadingFactor = 1.5f;
}

float4 main(PSInputXRay input) : SV_Target
{
    float ndotv = 1 - dot(normalize(input.n), normalize(input.vEye.xyz)) * XRayBorderFadingFactor;
    return float4(XRayObjectColor.xyz * ndotv, 0);
}