//Ref: https://learnopengl.com/Advanced-Lighting/SSAO
//Ref: https://mynameismjp.wordpress.com/2009/03/10/reconstructing-position-from-depth/
#ifndef SSAOEFFECTS
#define SSAOEFFECTS
#define SSAO
#include"..\Common\Common.hlsl"
#pragma pack_matrix( row_major )
struct SSAOPS_INPUT
{
    float4 Pos : SV_POSITION;
    noperspective
    float2 Tex : TEXCOORD0;
};

float3 getPos(in float2 tex, in float depth)
{
    float x = tex.x * 2 - 1;
    float y = 1 - tex.y * 2;
    float4 v = mul(float4(x, y, depth, 1), invProjection);
    return (v / v.w).xyz;
}

float4 main(SSAOPS_INPUT input) : SV_Target
{
    float depth = texSSAODepth.Sample(samplerSurface, input.Tex); 
    if (depth == 1)
    {
        return float4(1, 0, 0, 0);
    }
    float4 value = texSSAOMap.Sample(samplerSurface, input.Tex);
    float3 normal = normalize(value.rgb);
    float3 position = getPos(input.Tex, depth); 
    
    float3 randomVec = texSSAONoise.Sample(samplerNoise, input.Tex * noiseScale);

    float3 tangent = normalize(randomVec - normal * dot(randomVec, normal));
    float3 bitangent = cross(normal, tangent);
    float3x3 TBN = mul(float3x3(tangent, bitangent, normal), (float3x3) mView);
    float occlusion = 0;
    const float inv = 1.0 / SSAOKernalSize;

    [loop]
    for (uint i = 0; i < SSAOKernalSize; ++i)
    {
        float3 sample = mul(kernel[i].xyz, TBN);
        sample = mad(sample, radius, position);
        float4 offset = float4(sample, 1);
        offset = mul(offset, mProjection);
        offset.xy /= offset.w;
        offset.xy = mad(offset.xy, float2(0.5, -0.5), 0.5f);
        float sampleDepth = texSSAODepth.SampleLevel(samplerSurface, offset.xy, 0); 
        if (sampleDepth == 1)
        {
            continue;
        }
        sampleDepth = getPos(offset.xy, sampleDepth).z; //*= input.Corner.z;
        float rangeCheck = smoothstep(0.0, 1.0, radius / abs(position.z - sampleDepth)) * SSAOIntensity;
        //float rangeCheck = whenlt(abs(position.z - sampleDepth), radius); 
        occlusion += whenle(abs(sampleDepth), abs(sample.z - SSAOBias)) * rangeCheck;
    }
    occlusion = 1.0 - occlusion * inv;
    return float4(occlusion, 0, 0, 0);

}
#endif