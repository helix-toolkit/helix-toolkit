#ifndef PSMESHXRAY_HLSL
#define PSMESHXRAY_HLSL
#define MESH
#include"..\Common\DataStructs.hlsl"
#include"..\Common\CommonBuffers.hlsl"


float4 main(PSInputXRay input) : SV_Target
{
    float ndotv = 1 - dot(normalize(input.n), normalize(input.vEye.xyz)) * vParams.y;//reuse model cb parameter fields
    return float4(vColor.xyz * ndotv, 0);
}

#endif