

#define MESH
#include"..\Common\DataStructs.hlsl"
#include"..\Common\CommonBuffers.hlsl"

//--------------------------------------------------------------------------------------
//  Render diffuse map
//--------------------------------------------------------------------------------------
float4 main(PSInput input) : SV_Target
{
    float4 c = input.cDiffuse;
    if (bHasDiffuseMap)
    {
		c *= texDiffuseMap.Sample(samplerDiffuse, input.t);
    }
    float3 dir = normalize(vEyePos - input.wp.xyz);
    float f = clamp(dot(dir, normalize(input.n)), 0.5f, 1);
    c.rgb *= f;
    return c;
}