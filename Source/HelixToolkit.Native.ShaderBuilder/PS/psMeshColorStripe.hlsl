

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
        c += texColorStripe1DX.Sample(samplerDiffuse, input.t.x);
        blend = 0.5;
    }
    if (bHasAlphaMap)
    {
        c = c * blend + (1 - blend) * texColorStripe1DY.Sample(samplerDiffuse, input.t.y);
    }
    
    c *= input.cDiffuse;
    float3 dir = normalize(vEyePos - input.wp.xyz);
    float f = clamp(dot(dir, normalize(input.n)), 0.5f, 1);
    c.rgb *= f;
    return saturate(c);
}