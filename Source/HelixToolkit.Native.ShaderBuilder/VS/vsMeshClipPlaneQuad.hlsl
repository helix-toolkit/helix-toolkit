#ifndef VSFULLSCREENQUAD_HLSL
#define VSFULLSCREENQUAD_HLSL

#include"..\Common\DataStructs.hlsl"

#pragma pack_matrix( row_major )

static const float2 quadtexcoords[4] =
{
    float2(1, 0),
    float2(0, 0),
    float2(1, 1),
    float2(0, 1),
};

float4 main(uint vI : SV_VERTEXID) : SV_Position
{
    float2 texcoord = quadtexcoords[vI];
    return float4((texcoord.x - 0.5f) * 2, -(texcoord.y - 0.5f) * 2, 0, 1);
}

#endif