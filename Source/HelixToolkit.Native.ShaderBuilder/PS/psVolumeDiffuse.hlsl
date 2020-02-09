//Reference 
// https://graphicsrunner.blogspot.com/search/label/Volume%20Rendering
// https://shaderbits.com/blog/creating-volumetric-ray-marcher
#ifndef PSVOLUMEDIFFUSE_HLSL
#define PSVOLUMEDIFFUSE_HLSL
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
 
    float4 value = 0;
 
    float3 stepV = dir * stepSize;

    float3 pos = float3(front + stepV * iterationOffset);

    uint iteration = min(dirLength / stepSize, maxIterations);
    float lengthAccu = iterationOffset * stepSize;
    float corr = clamp(actualSampleDist / baseSampleDist, 1, 4);
    dirLength -= stepSize;
    [loop]
    for (uint i = iterationOffset; i < iteration; i++)
    {
        float4 color = pColor;
		//get the normal and iso-value for the current sample
        value = texVolume.SampleLevel(samplerVolume, pos.xyz, 0);
        if (value.a > isoValue)
        {
		//index the transfer function with the iso-value (value.a)
		//and get the rgba value for the voxel
            if (bHasGradientMapX)
            {
                color *= texColorStripe1DX.SampleLevel(samplerVolume, value.a, 0);
            }
            src = float4(color.rgb, color.a * value.a);
		//Oppacity correction: As sampling distance decreases we get more samples.
		//Therefore the alpha values set for a sampling distance of .5f will be too
		//high for a sampling distance of .25f (or conversely, too low for a sampling
		//distance of 1.0f). So we have to adjust the alpha accordingly.
            src.a = 1 - pow(abs(1 - src.a), corr);
					  
            float s = 1 - dot(normalize(value.xyz), -dir);
				
		//diffuse shading + fake ambient lighting
            src.rgb = s * src.rgb + .1f * src.rgb;
		
		//Front to back blending
		// dst.rgb = dst.rgb + (1 - dst.a) * src.a * src.rgb
		// dst.a   = dst.a   + (1 - dst.a) * src.a		
            src.rgb *= src.a;
            dst = (1.0f - dst.a) * src + dst;
        }
		
		//advance the current position
        pos += stepV;
		
        lengthAccu += stepSize;
        //break if the position is greater than <1, 1, 1>
        if (lengthAccu >= dirLength || dst.a > 0.95)
            break;
    }
 
    return dst;
}
#endif