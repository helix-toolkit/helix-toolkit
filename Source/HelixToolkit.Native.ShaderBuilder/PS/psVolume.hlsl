//Reference https://graphicsrunner.blogspot.com/search/label/Volume%20Rendering
#ifndef PSVOLUME_HLSL
#define PSVOLUME_HLSL
#define VOLUME
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

float4 main(VolumePS_INPUT input) : SV_Target
{
    //calculate projective texture coordinates
    //used to project the front and back position textures onto the cube
    float2 texC = input.tex.xy;
    texC.x = 0.5f * texC.x + 0.5f;
    texC.y = -0.5f * texC.y + 0.5f;
    float3 front = input.mPos.xyz + float3(0.5, 0.5, 0.5);
    float3 back = texVolumeBack.Sample(samplerVolume, texC).xyz;
 
    float3 dir = back - front;
    float dirLength = length(dir);
    dir = normalize(dir);
    float4 pos = float4(front, 0);
 
    float4 dst = float4(0, 0, 0, 0);
    float4 src = 0;
 
    float value = 0;
    uint iteration = min(dirLength / stepSize, maxIterations);
 
    float3 Step = dir * stepSize;

    float lengthAccu = 0;
    [loop]
    for (uint i = 0; i < iteration; i++)
    {
        float4 color = pColor;
        pos.w = 0;
        value = texVolume.SampleLevel(samplerVolume, pos.xyz, 0).r;
        if (bHasGradientMapX)
        {
            color *= texColorStripe1DX.SampleLevel(samplerVolume, value, 0);
        }
        src = float4(color.rgb * value, color.a * value);
        //src.a *= .5f; //reduce the alpha to have a more transparent result 
         
        //Front to back blending
        // dst.rgb = dst.rgb + (1 - dst.a) * src.a * src.rgb
        // dst.a   = dst.a   + (1 - dst.a) * src.a     
        dst = (1 - dst.a) * src + dst;        
        //advance the current position
        pos.xyz += Step;
        lengthAccu += stepSize;
        //break if the position is greater than <1, 1, 1>
        if (lengthAccu > dirLength || dst.a > 0.95)
            break;
    }
 
    return saturate(dst);
}
#endif