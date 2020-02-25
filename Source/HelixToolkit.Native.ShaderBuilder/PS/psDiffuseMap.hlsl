

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
		c *= texDiffuseMap.Sample(samplerSurface, input.t);
    }
    c = lerp(c, input.c, vertColorBlending);
    float3 N = bRenderFlat ? normalize(cross(ddy(input.wp.xyz), ddx(input.wp.xyz))) : normalize(input.n);
    if (!bHasNormalMap)//Used for Unlit
    {
        float3 dir = normalize(vEyePos - input.wp.xyz);
        float f = clamp(0.5 + 0.5 * abs(dot(dir, N)), 0, 1);
        c.rgb *= f;
    }
    return c;
}