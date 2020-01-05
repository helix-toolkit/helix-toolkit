//Reference 
// https://graphicsrunner.blogspot.com/search/label/Volume%20Rendering
// https://shaderbits.com/blog/creating-volumetric-ray-marcher
#ifndef PSVOLUME_HLSL
#define PSVOLUME_HLSL
#define VOLUME
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

float4 main(VolumePS_INPUT input) : SV_Target
{
    float3 localCamPos;
    float3 localCamVec;
    if (IsPerspective)
    {
        localCamPos = vEyePos;
        localCamVec = input.wp.xyz - vEyePos;
    }
    else
    {
        localCamVec = -getLookDir(mView);
        float3 v = input.wp.xyz - vEyePos;
        float k = dot(v, localCamVec);
        float3 w = localCamVec * k;
        localCamPos = vEyePos - w + v;
    }
    localCamPos = mul(float4(localCamPos, 1), mWorldInv).xyz;
    localCamVec = normalize(mul(float4(localCamVec, 0), mWorldInv).xyz);
    float4 entry = getBoxEntryPoint(localCamPos, localCamVec, input);
    float3 front = entry.xyz;
    float3 dir = localCamVec;
    float3 back = front + dir * entry.w;
    float dirLength = entry.w;
 
    float4 dst = float4(0, 0, 0, 0);
    float4 src = 0;
 
    float value = 0;
    uint iteration = min(dirLength / stepSize, maxIterations);
 
    float3 stepV = dir * stepSize;

    float3 pos = float3(front + stepV * iterationOffset);

    float lengthAccu = iterationOffset * stepSize;
    dirLength -= stepSize;
    [loop]
    for (uint i = iterationOffset; i < iteration; i++)
    {
        float4 color = pColor;
        value = texVolume.SampleLevel(samplerVolume, pos.xyz, 0).r;
        if (value > isoValue)
        {
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
        }
  
        //advance the current position
        pos += stepV;
        lengthAccu += stepSize;
        //break if the position is greater than <1, 1, 1>
        if (lengthAccu >= dirLength || dst.a > 0.95)
            break;
    }
 
    return saturate(dst);
}
#endif