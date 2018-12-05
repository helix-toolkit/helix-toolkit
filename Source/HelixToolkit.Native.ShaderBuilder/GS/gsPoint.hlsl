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
    points[1] = windowToProj(A1w, posA.z, posA.w);
    points[0] = windowToProj(A2w, posA.z, posA.w);
    points[3] = windowToProj(B2w, posA.z, posA.w);
    points[2] = windowToProj(B1w, posA.z, posA.w);
}

void makeNonFixedQuad(out float4 points[4], in float4 posA, in float w, in float h)
{
    // Bring A and B in window space
    float2 Aw = posA.xy;
    float w2 = w * 0.5;
    float h2 = h * 0.5;

    // Compute the corners of the ribbon in window space
    float4 A1w = float4(Aw.x + w2, Aw.y + h2, posA.z, posA.w);
    float4 A2w = float4(Aw.x - w2, Aw.y + h2, posA.z, posA.w);
    float4 B1w = float4(Aw.x - w2, Aw.y - h2, posA.z, posA.w);
    float4 B2w = float4(Aw.x + w2, Aw.y - h2, posA.z, posA.w);

    // bring back corners in projection frame
    points[1] = mul(A1w, mProjection);
    points[0] = mul(A2w, mProjection);
    points[3] = mul(B2w, mProjection);
    points[2] = mul(B1w, mProjection);
}

[maxvertexcount(4)]
void main(point GSInputPS input[1], inout TriangleStream<PSInputPS> outStream)
{
    PSInputPS output = (PSInputPS) 0;
    output.vEye = input[0].vEye;   
    output.c = input[0].c;
    float4 spriteCorners[4] = { (float4) 0, (float4) 0, (float4) 0, (float4) 0 };
    if(fixedSize)
        makeQuad(spriteCorners, input[0].p, pfParams.x, pfParams.y);
    else
        makeNonFixedQuad(spriteCorners, mul(input[0].wp, mView), pfParams.x, pfParams.y);
    output.p = spriteCorners[0];    
    output.t = float3(1, 1, 1);
    outStream.Append(output);
    
    output.p = spriteCorners[1];
    output.t = float3(1, -1, 1);
    outStream.Append(output);
 
    output.p = spriteCorners[2];
    output.t = float3(-1, 1, 1);
    outStream.Append(output);
    
    output.p = spriteCorners[3];
    output.t = float3(-1, -1, 1);
    outStream.Append(output);
    
    outStream.RestartStrip();
}
#endif