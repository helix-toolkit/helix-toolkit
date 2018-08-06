#ifndef VSPLANEGRID_HLSL
#define VSPLANEGRID_HLSL
#define POINTLINE
#include"..\Common\DataStructs.hlsl"
#include"..\Common\CommonBuffers.hlsl"
#pragma pack_matrix( row_major )

static const float2 vertCoord[4] =
{
    float2(-1, 1),
    float2(-1, -1),
    float2(1, 1),
    float2(1, -1),
};


PSPlaneGridInput main(uint vI : SV_VERTEXID)
{
    float2 vt = vertCoord[vI] * 10000 + vEyePos.xz;
    float4 v = mul(float4(vt.x, 0, vt.y, 1), pWorld);
    PSPlaneGridInput output = (PSPlaneGridInput) 0;
    output.uv = v.xz / pfParams.x;
    output.p = mul(v, mViewProjection);
    float3 vEye = vEyePos - v.xyz;
    output.vEye = float4(normalize(vEye), length(vEye)); //Use wp for camera->vertex direction
    return output;
}
#endif