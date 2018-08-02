#ifndef GSMESHSKINNED_HLSL
#define GSMESHSKINNED_HLSL
#define MESH
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

[maxvertexcount(1)]
void main(point VSSkinnedOutput input[1], inout PointStream<VSSkinnedOutput> output)
{
    input[0].p /= input[0].p.w;
    output.Append(input[0]);
    output.RestartStrip();
}

#endif