//Reference https://graphicsrunner.blogspot.com/search/label/Volume%20Rendering
#ifndef PSVOLUMEDIFFUSE_HLSL
#define PSVOLUMEDIFFUSE_HLSL
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
 
    float4 value = 0;
 
    float3 Step = dir * stepSize;

    float3 L = normalize(vEyePos - input.wp.xyz);
    float lengthAccu = 0;
    [loop]
    for (uint i = 0; i < iterations; i++)
    {
        pos.w = 0;
        float4 color = pColor;
		//get the normal and iso-value for the current sample
        value = texVolume.Sample(samplerVolume, pos.xyz);
		
		//index the transfer function with the iso-value (value.a)
		//and get the rgba value for the voxel
        if (bHasGradientMapX)
        {
            color *= texColorStripe1DX.Sample(samplerVolume, value.a);
        }
        src = float4(color.rgb, color.a * value.a);
		//Oppacity correction: As sampling distance decreases we get more samples.
		//Therefore the alpha values set for a sampling distance of .5f will be too
		//high for a sampling distance of .25f (or conversely, too low for a sampling
		//distance of 1.0f). So we have to adjust the alpha accordingly.
        src.a = 1 - pow((1 - src.a), actualSampleDist / baseSampleDist);
					  
        float s = 1-dot(normalize(value.xyz), L);
				
		//diffuse shading + fake ambient lighting
        src.rgb = s * src.rgb + .1f * src.rgb;
		
		//Front to back blending
		// dst.rgb = dst.rgb + (1 - dst.a) * src.a * src.rgb
		// dst.a   = dst.a   + (1 - dst.a) * src.a		
        src.rgb *= src.a;
        dst = (1.0f - dst.a) * src + dst;
	
		
		//advance the current position
        pos.xyz += Step;
		
        lengthAccu += stepSize;
        //break if the position is greater than <1, 1, 1>
        if (lengthAccu > dirLength || dst.a >= 1)
            break;
    }
 
    return dst;
}
#endif