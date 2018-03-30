#ifndef VSOUTLINESCREENQUAD_HLSL
#define VSOUTLINESCREENQUAD_HLSL

#include"..\Common\DataStructs.hlsl"

#pragma pack_matrix( row_major )

static const float2 quadtexcoords[4] =
{
    float2(1, 0),
    float2(0, 0),
    float2(1, 1),
    float2(0, 1),
};

MeshOutlinePS_INPUT main(uint vI : SV_VERTEXID)
{
    MeshOutlinePS_INPUT output = (MeshOutlinePS_INPUT) 0;
    float2 texcoord = quadtexcoords[vI];
    output.Tex = texcoord;
    output.Pos = float4((texcoord.x - 0.5f) * 2, -(texcoord.y - 0.5f) * 2, 0, 1);
    return output;
}

#endif