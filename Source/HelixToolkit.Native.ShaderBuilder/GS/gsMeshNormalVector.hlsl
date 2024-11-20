#ifndef GSMESHNORMALVECTOR_HLSL
#define GSMESHNORMALVECTOR_HLSL
#define MESH
#include "..\Common\DataStructs.hlsl"
#include "..\Common\Common.hlsl"
void makeLine(out float4 points[4], in float4 posA, in float4 posB, in float width)
{
	if (posA.w * posB.w < 0)
	{
		if (posA.w < 0)
		{
			posA = lerp(posA, posB, -posA.w / (posB.w - posA.w));
		}
		else
		{
			posB = lerp(posB, posA, -posB.w / (posA.w - posB.w));
		}

	}
    // Bring A and B in window space
    float2 Aw = projToWindow(posA);
    float2 Bw = projToWindow(posB);

    // Compute tangent and binormal of line AB in window space
    // Binormal is scaled by line width 
    float2 tangent = normalize(Bw.xy - Aw.xy);
    float2 binormal = width * float2(tangent.y, -tangent.x);
    
    // Compute the corners of the ribbon in window space
	float2 A1w = Aw + binormal;
	float2 A2w = Aw - binormal;
	float2 B1w = Bw + binormal;
	float2 B2w = Bw - binormal;

    // bring back corners in projection frame
    points[0] = windowToProj(A1w, posA.z, posA.w);
    points[1] = windowToProj(A2w, posA.z, posA.w);
    points[2] = windowToProj(B1w, posB.z, posB.w);
    points[3] = windowToProj(B2w, posB.z, posB.w);
}

[maxvertexcount(4)]
void main(point PSInput input[1], inout TriangleStream<PSInputPS> outStream)
{
    float4 normal = float4(input[0].n, 0);
    float4 p1 = input[0].wp + normal;
    p1.w = 1;
    p1 = mul(p1, mViewProjection);

    float4 lineCorners[4];
    makeLine(lineCorners, input[0].p, p1, 0.5);
    PSInputPS output = (PSInputPS) 0;
    output.c = float4(input[0].n * 0.5 + 0.5, 1);

    output.p = lineCorners[0];
    outStream.Append(output);
	
    output.p = lineCorners[1];
    outStream.Append(output);
 
    output.p = lineCorners[2];
    outStream.Append(output);
	
    output.p = lineCorners[3];
    outStream.Append(output);
	
    outStream.RestartStrip();
}

#endif