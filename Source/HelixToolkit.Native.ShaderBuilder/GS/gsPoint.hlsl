#ifndef GSPOINT_HLSL
#define GSPOINT_HLSL
#define POINTLINE
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

void makeQuad(out float4 points[4], in float4 posA, in float w, in float h)
{
    // Bring A and B in window space
    float2 Aw = projToWindow(posA);
    float w2 = w * 0.5;
    float h2 = h * 0.5;

    // Compute the corners of the ribbon in window space
    float2 A1w = float2(Aw.x + w2, Aw.y + h2);
    float2 A2w = float2(Aw.x - w2, Aw.y + h2);
    float2 B1w = float2(Aw.x - w2, Aw.y - h2);
    float2 B2w = float2(Aw.x + w2, Aw.y - h2);

    // bring back corners in projection frame
    points[0] = windowToProj(A1w, posA.z, posA.w);
    points[1] = windowToProj(A2w, posA.z, posA.w);
    points[2] = windowToProj(B2w, posA.z, posA.w);
    points[3] = windowToProj(B1w, posA.z, posA.w);
}

[maxvertexcount(4)]
void main(point GSInputPS input[1], inout TriangleStream<PSInputPS> outStream)
{
    PSInputPS output = (PSInputPS) 0;
    output.vEye = input[0].vEye;    
    float4 spriteCorners[4];
    makeQuad(spriteCorners, input[0].p, pfParams.x, pfParams.y);

    output.p = spriteCorners[0];
    output.c = input[0].c;
    output.t[0] = +1;
    output.t[1] = +1;
    output.t[2] = 1;   
    outStream.Append(output);
    
    output.p = spriteCorners[1];
    output.c = input[0].c;
    output.t[0] = +1;
    output.t[1] = -1;
    output.t[2] = 1;
    outStream.Append(output);
 
    output.p = spriteCorners[2];
    output.c = input[0].c;
    output.t[0] = -1;
    output.t[1] = +1;
    output.t[2] = 1;
    outStream.Append(output);
    
    output.p = spriteCorners[3];
    output.c = input[0].c;
    output.t[0] = -1;
    output.t[1] = -1;
    output.t[2] = 1;
    outStream.Append(output);
    
    outStream.RestartStrip();
}
#endif