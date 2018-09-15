#ifndef GSLINE_HLSL
#define GSLINE_HLSL
#define POINTLINE
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"
//--------------------------------------------------------------------------------------
// Make a a ribbon line of the specified pixel width from 2 points in the projection frame.
//--------------------------------------------------------------------------------------
void makeLine(out float4 points[4], in float4 posA, in float4 posB, in float width)
{
    // Bring A and B in window space
    float2 Aw = projToWindow(posA);
    float2 Bw = projToWindow(posB);

    // Compute tangent and binormal of line AB in window space
    // Binormal is scaled by line width 
    float2 tangent = normalize(Bw.xy - Aw.xy);
    float2 binormal = width * float2(tangent.y, -tangent.x);
    
    // Compute the corners of the ribbon in window space
    float2 A1w = (Aw - binormal);
    float2 A2w = (Aw + binormal);
    float2 B1w = (Bw - binormal);
    float2 B2w = (Bw + binormal);

    // bring back corners in projection frame
    points[0] = windowToProj(A1w, posA.z, posA.w);
    points[1] = windowToProj(A2w, posA.z, posA.w);
    points[2] = windowToProj(B1w, posB.z, posB.w);
    points[3] = windowToProj(B2w, posB.z, posB.w);
}

[maxvertexcount(4)]
void main(line GSInputPS input[2], inout TriangleStream<PSInputPS> outStream)
{
    PSInputPS output = (PSInputPS) 0;
		
	float4 lineCorners[4];
    makeLine(lineCorners, input[0].p, input[1].p, pfParams.x);

	output.p = lineCorners[0];
	output.c = input[0].c;
	output.t[0] = +1;
	output.t[1] = +1;
	output.t[2] = 1;
    output.vEye = input[0].vEye;
	outStream.Append(output);
	
	output.p = lineCorners[1];
	output.c = input[0].c;
	output.t[0] = +1;
	output.t[1] = -1;
	output.t[2] = 1;
	outStream.Append(output);
 
	output.p = lineCorners[2];
	output.c = input[1].c;
	output.t[0] = -1;
	output.t[1] = +1;
	output.t[2] = 1;
    output.vEye = input[1].vEye;
	outStream.Append(output);
	
	output.p = lineCorners[3];
	output.c = input[1].c;
	output.t[0] = -1;
	output.t[1] = -1;
	output.t[2] = 1;
    output.vEye = input[1].vEye;
	outStream.Append(output);
	
	outStream.RestartStrip();
}
#endif