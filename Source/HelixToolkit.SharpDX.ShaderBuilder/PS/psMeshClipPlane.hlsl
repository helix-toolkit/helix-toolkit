#ifndef PSMESHCLIPPLANE_HLSL
#define PSMESHCLIPPLANE_HLSL
#define MATERIAL
#define CLIPPLANE
#define MESH
#include"psMeshBlinnPhong.hlsl"
#pragma pack_matrix( row_major )

void DetermineCutPlane(float3 pixelPos)
{
    if (EnableCrossPlane.x)
    {
        float3 p = pixelPos - CrossPlaneParams._m00_m01_m02 * CrossPlaneParams._m03;
        if (dot(CrossPlaneParams._m00_m01_m02, p) < 0)
        {
            discard;
        }
    }
    if (EnableCrossPlane.y)
    {
        float3 p = pixelPos - CrossPlaneParams._m10_m11_m12 * CrossPlaneParams._m13;
        if (dot(CrossPlaneParams._m10_m11_m12, p) < 0)
        {
            discard;
        }
    }
    if (EnableCrossPlane.z)
    {
        float3 p = pixelPos - CrossPlaneParams._m20_m21_m22 * CrossPlaneParams._m23;
        if (dot(CrossPlaneParams._m20_m21_m22, p) < 0)
        {
            discard;
        }
    }
    if (EnableCrossPlane.w)
    {
        float3 p = pixelPos - CrossPlaneParams._m30_m31_m32 * CrossPlaneParams._m33;
        if (dot(CrossPlaneParams._m20_m21_m22, p) < 0)
        {
            discard;
        }
    }
}

float4 psClipPlaneMesh(PSInput input) : SV_Target
{
    DetermineCutPlane(input.wp.xyz);
    return main(input);
}

#endif