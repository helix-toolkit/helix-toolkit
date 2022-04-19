/**
Classic depth peeling implementation for order independent transparency rendering.
Ref: Nvidia Direct3D SDK 11 Samples: https://developer.download.nvidia.com/gameworks/samples/NVIDIA_SDK11_Direct3D_11.00.0328.2105.exe
*/
#ifndef PSOITDEPTHPEELINGCOMMON_HLSL
#define PSOITDEPTHPEELINGCOMMON_HLSL
#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"
#include"psCommon.hlsl"
#define MAX_DEPTH_FLOAT 1.0f

Texture2DMS<float4> tLayerColor : register(t100);
Texture2D<float2> tDepthBlender : register(t100);
Texture2D<float4> tFrontBlender : register(t101);
Texture2D<float4> tBackBlender : register(t102);

struct DDPOutputMRT
{
    float2 Depths : SV_Target0;
    float4 FrontColor : SV_Target1;
    float4 BackColor : SV_Target2;
};

float4 firstPassPS(float4 p : SV_POSITION) : SV_TARGET
{
    float z = p.z;
    return float4(-z, z, 0, 0);
}

DDPOutputMRT depthPeelPS(in float4 pos, in float4 color)
{
    DDPOutputMRT OUT = (DDPOutputMRT) 0;

    // Window-space depth interpolated linearly in screen space
    float fragDepth = pos.z;

    OUT.Depths.xy = tDepthBlender.Load(int3(pos.xy, 0)).xy;
    float nearestDepth = -OUT.Depths.x;
    float farthestDepth = OUT.Depths.y;

    if (fragDepth < nearestDepth || fragDepth > farthestDepth)
    {
        // Skip this depth in the peeling algorithm
        OUT.Depths.xy = -MAX_DEPTH_FLOAT;
        return OUT;
    }
    
    if (fragDepth > nearestDepth && fragDepth < farthestDepth)
    {
        // This fragment needs to be peeled again
        OUT.Depths.xy = float2(-fragDepth, fragDepth);
        return OUT;
    }
    
    // If we made it here, this fragment is on the peeled layer from last pass
    // therefore, we need to shade it, and make sure it is not peeled any farther
    OUT.Depths.xy = -MAX_DEPTH_FLOAT;
    
    if (fragDepth == nearestDepth)
    {
        color.rgb *= color.a;
        OUT.FrontColor = color;
    }
    else
    {
        OUT.BackColor = color;
    }
    return OUT;
}

float4 blendingPS(ScreenDupVS_INPUT IN) : SV_TARGET
{
    return tLayerColor.Load(int2(IN.Pos.xy), 0);
}

float4 finalPS(ScreenDupVS_INPUT IN) : SV_TARGET
{
    float4 frontColor = tFrontBlender.Load(int3(IN.Pos.xy, 0));
    float3 backColor = tBackBlender.Load(int3(IN.Pos.xy, 0)).xyz;
    float3 final = frontColor.xyz + backColor * frontColor.w;
    float alpha = frontColor.w;
    return float4(final, alpha);
}

#endif