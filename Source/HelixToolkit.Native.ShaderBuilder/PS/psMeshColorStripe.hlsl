

#define MESH
#include"..\Common\DataStructs.hlsl"
#include"..\Common\CommonBuffers.hlsl"
//--------------------------------------------------------------------------------------
//  Render diffuse map
//--------------------------------------------------------------------------------------
float4 main(PSInput input) : SV_Target
{
    float4 c = float4(0, 0, 0, 0);
    float blend = 0;
    if (bHasDiffuseMap)
    {
        c += texColorStripe1DX.Sample(samplerSurface, input.t.x);
        blend = 0.5;
    }
    if (bHasAlphaMap)
    {
        c = c * blend + (1 - blend) * texColorStripe1DY.Sample(samplerSurface, input.t.y);
    }
    
    c *= input.cDiffuse;
    float3 dir = normalize(input.vEye.xyz);
    float f = 0.8 + abs(dot(dir, normalize(input.n))) * 0.2;
    c.rgb *= clamp(f, 0.8, 1);
    return saturate(c);
}