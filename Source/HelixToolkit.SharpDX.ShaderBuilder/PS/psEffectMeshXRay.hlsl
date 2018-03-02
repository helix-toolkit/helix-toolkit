#define BORDEREFFECTS
#include"..\Common\DataStructs.hlsl"
#include"..\Common\CommonBuffers.hlsl"


float4 main(PSInputXRay input) : SV_Target
{
    float ndotv = 1 - dot(normalize(input.n), normalize(input.vEye.xyz)) * Param._m00; //reuse model cb parameter fields
    return float4(Color.xyz * ndotv, 0);
}
