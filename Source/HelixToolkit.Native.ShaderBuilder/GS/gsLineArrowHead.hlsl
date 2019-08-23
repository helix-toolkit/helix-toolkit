#ifndef GSLINEARROWHEAD_HLSL
#define GSLINEARROWHEAD_HLSL
#define POINTLINE
#include"..\GS\gsLine.hlsl"

const static float default_cone_w = 0.5;
const static float default_cone_h = 1.5;
const static float4 cone[10] =
{
    float4(-default_cone_w, -default_cone_h, -default_cone_w, default_cone_w),
    float4(default_cone_w, -default_cone_h, -default_cone_w, default_cone_w),
    float4(-default_cone_w, -default_cone_h, default_cone_w, default_cone_w),
    float4(default_cone_w, -default_cone_h, default_cone_w, default_cone_w),
    float4(0, 0, 0, default_cone_w),
    float4(default_cone_w, -default_cone_h, -default_cone_w, default_cone_w),
    float4(-default_cone_w, -default_cone_h, -default_cone_w, default_cone_w),
    float4(-default_cone_w, -default_cone_h, -default_cone_w, default_cone_w),
    float4(-default_cone_w, -default_cone_h, default_cone_w, default_cone_w),
    float4(0, 0, 0, default_cone_w)
};

void makeCone(out float4 points[10], in float4 posA, in float4 posB, in float scale)
{
    float3 dir = normalize(posA.xyz - posB.xyz);
    float3 axis = normalize(cross(float3(0, 1, 0), dir));
    float cosA = dot(float3(0, 1, 0), dir);
    float4x4 transform = float4x4(scale, 0, 0, 0, 0, scale, 0, 0, 0, 0, scale, 0, 0, 0, 0, 1);
    if (1 - abs(cosA) > 1e-3)
    {
        float sinA = sin(acos(cosA));
        float x = axis.x;
        float y = axis.y;
        float z = axis.z;
        float xy = axis.x * axis.y;
        float xz = axis.x * axis.z;
        float yz = axis.y * axis.z;
        float xx = axis.x * axis.x;
        float yy = axis.y * axis.y;
        float zz = axis.z * axis.z;
        transform = mul(transform, float4x4(
            (xx + cosA * (1 - xx)), (xy - (cosA * xy)) + sinA * z, (xz - (cosA * xz)) - y * sinA, 0,
            (xy - (cosA * xy)) - z * sinA, (yy + (cosA * (1 - yy))), (yz - (cosA * yz)) + sinA * x, 0,
            (xz - (cosA * xz)) + sinA * y, (yz - (cosA * yz)) - sinA * x, (zz + (cosA * (1 - zz))), 0,
            0, 0, 0, 1));
    }
    else // Parallel to the arrow direction
    {
        float d = sign(cosA);
        transform._m00_m11_m22 *= d;
    }
    transform._m30_m31_m32 = posA.xyz;
    [unroll]
    for (int i = 0; i < 10; ++i)
    {
        points[i] = mul(cone[i], transform);
    }
}

[maxvertexcount(14)]
void mainArrowHead(line GSInputPS input[2], inout TriangleStream<PSInputPS> outStream)
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

    [unroll]
    for (int i = 0; i < 10; ++i)
    {
        output.p = mul(cone[i], mViewProjection);
        outStream.Append(output);
    }
    outStream.RestartStrip();
}
#endif