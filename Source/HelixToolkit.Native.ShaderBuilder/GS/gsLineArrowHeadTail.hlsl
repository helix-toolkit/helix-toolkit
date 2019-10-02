#ifndef GSLINEARROWTAIL_HLSL
#define GSLINEARROWTAIL_HLSL
#define POINTLINE
#include"..\GS\gsLineArrowHead.hlsl"

[maxvertexcount(24)]
void mainArrowHeadTail(line GSInputPS input[2], inout TriangleStream<PSInputPS> outStream)
{
    PSInputPS output = (PSInputPS) 0;
    float texX = length(input[0].wp.xyz - input[1].wp.xyz) / max(1e-5, pTextureScale);
    float4 lineCorners[4] = { (float4) 0, (float4) 0, (float4) 0, (float4) 0 };
    if (fixedSize)
        makeLine(lineCorners, input[0].p, input[1].p, pfParams.x);
    else
        makeLineNonFixed(lineCorners, mul(input[0].wp, mView), mul(input[1].wp, mView), pfParams.x);
    output.vEye = input[0].vEye;
    output.c = input[0].c;
    output.p = lineCorners[0];
    output.t = float3(1, 1, 1);
    output.tex = float3(texX, 1, 1);
    outStream.Append(output);
	
    output.p = lineCorners[1];
    output.c = input[0].c;
    output.t = float3(1, -1, 1);
    output.tex = float3(texX, 0, 1);
    outStream.Append(output);
 
    output.vEye = input[1].vEye;
    output.c = input[1].c;
    output.p = lineCorners[2];
    output.t = float3(-1, 1, 1);
    output.tex = float3(0, 1, 1);
    outStream.Append(output);
	
    output.p = lineCorners[3];
    output.t = float3(-1, -1, 1);
    output.tex = float3(0, 0, 1);
    outStream.Append(output);
	
    outStream.RestartStrip();

    output.vEye = input[1].vEye;
    output.t = float3(0, 0, 0);
    output.c = input[1].c;
    output.tex = float3(0, 0, 0);
    float4 cone[10];
    makeCone(cone, input[1].wp, input[0].wp, pfParams.z);
    int i = 0;
    // Make head
    [unroll]
    for (i = 0; i < 10; ++i)
    {
        output.p = mul(cone[i], mViewProjection);
        outStream.Append(output);
    }
    outStream.RestartStrip();
    // Make tail
    output.vEye = input[0].vEye;
    output.t = float3(0, 0, 0);
    output.c = input[0].c;
    makeCone(cone, input[0].wp, input[1].wp, pfParams.z);

    [unroll]
    for (i = 0; i < 10; ++i)
    {
        output.p = mul(cone[i], mViewProjection);
        outStream.Append(output);
    }
    outStream.RestartStrip();
}
#endif