#ifndef VSPLANEGRID_HLSL
#define VSPLANEGRID_HLSL
#define PLANEGRID
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
    float2 vt = (float2) 0;
    float4 v = (float4) 0;
    PSPlaneGridInput output = (PSPlaneGridInput) 0;
    if (axis == 0)
    {
        vt = vertCoord[vI] * vFrustum.w * gridSpacing + vEyePos.yz;
        v = float4(planeD, vt.x, vt.y, 1);
        output.uv = v.yz;
    }
    else if (axis == 1)
    {
        vt = vertCoord[vI] * vFrustum.w * gridSpacing + vEyePos.xz;
        v = float4(vt.x, planeD, vt.y, 1);
        output.uv = v.xz;
    }
    else if (axis == 2)
    {
        vt = vertCoord[vI] * vFrustum.w * gridSpacing + vEyePos.xy;
        v = float4(vt.x, vt.y, planeD, 1);
        output.uv = v.xy;
    }
    output.wp = v.xyz;
    output.p = mul(v, mViewProjection);    
    if (bHasShadowMap)
    {
        output.sp = mul(v, vLightViewProjection);
    }
    return output;
}
#endif