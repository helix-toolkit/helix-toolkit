#ifndef GSMESHTRIANGLESEPARATE_HLSL
#define GSMESHTRIANGLESEPARATE_HLSL
#define MESH
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"
// Used to do face normal for each triangles.
[maxvertexcount(3)]
void main(triangle PSInput input[3], inout TriangleStream<PSInput> output)
{
    PSInput p0 = input[0];
    PSInput p1 = input[1];
    PSInput p2 = input[2];
    float3 v0 = (p1.wp - p0.wp).xyz;
    float3 v1 = (p2.wp - p0.wp).xyz;
    float3 n = normalize(cross(v0, v1));
    p0.n = n;
    p1.n = n;
    p2.n = n;
    output.Append(p0);
    output.Append(p1);
    output.Append(p2);
    output.RestartStrip();
}

#endif