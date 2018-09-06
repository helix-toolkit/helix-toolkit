#include"..\Common\DataStructs.hlsl"
#include"..\Common\CommonBuffers.hlsl"

float4 main(MeshOutlinePS_INPUT Input) : SV_Target
{
    float4 color = texDiffuseMap.Sample(samplerSurface, Input.Tex.xy);
    //color.rgb *= color.a;
    color.a = sqrt(dot(color.rgb, float3(0.299, 0.587, 0.114))); // compute luma
    return color;
}