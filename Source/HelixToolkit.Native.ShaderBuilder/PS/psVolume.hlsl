#ifndef PSVOLUME_HLSL
#define PSVOLUME_HLSL
#define VOLUME
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

float4 main(VolumePS_INPUT input) : SV_Target
{
    //calculate projective texture coordinates
    //used to project the front and back position textures onto the cube
    float2 texC = input.tex.xy / input.tex.w;
    texC.x = 0.5f * texC.x + 0.5f;
    texC.y = -0.5f * texC.y + 0.5f;
    float3 front = texVolumeFront.Sample(samplerVolume, texC).xyz;
    float3 back = texVolumeBack.Sample(samplerVolume, texC).xyz;
 
    float3 dir = normalize(back - front);

    float4 pos = float4(front, 0);
 
    float4 dst = float4(0, 0, 0, 0);
    float4 src = 0;
 
    float value = 0;
 
    float3 Step = dir * stepSize;
    [loop]
    for (int i = 0; i < iterations; i++)
    {
        pos.w = 0;
        value = texVolume.Sample(samplerVolume, pos.xyz).r;
             
        src = float4(pColor.rgb * value, pColor.a * value);
        //src.a *= .5f; //reduce the alpha to have a more transparent result 
         
        //Front to back blending
        // dst.rgb = dst.rgb + (1 - dst.a) * src.a * src.rgb
        // dst.a   = dst.a   + (1 - dst.a) * src.a     
        dst = (1.0f - dst.a) * src + dst;
     
        //break from the loop when alpha gets high enough
        if (dst.a >= .95f)
            break;
     
        //advance the current position
        pos.xyz += Step;
     
        //break if the position is greater than <1, 1, 1>
        if (pos.x > 1.0f &&
            pos.y > 1.0f &&
            pos.z > 1.0f)
            break;
    }
 
    return dst;
}
#endif