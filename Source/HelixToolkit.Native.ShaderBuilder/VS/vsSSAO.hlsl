#ifndef VSSSAOQUAD_HLSL
#define VSSSAOQUAD_HLSL
#define SSAO
#include"..\Common\CommonBuffers.hlsl"

#pragma pack_matrix( row_major )

static const float2 quadtexcoords[4] =
{
    float2(1, 0),
    float2(0, 0),
    float2(1, 1),
    float2(0, 1),
};

struct SSAOPS_INPUT
{
    float4 Pos : SV_POSITION;
    noperspective
    float2 Tex : TEXCOORD0;
};

SSAOPS_INPUT main(uint vI : SV_VERTEXID)
{
    SSAOPS_INPUT output = (SSAOPS_INPUT) 0;
    float2 texcoord = quadtexcoords[vI];
    output.Tex = texcoord;
    output.Pos = float4((texcoord.x - 0.5f) * 2, -(texcoord.y - 0.5f) * 2, 0, 1);
    return output;
}

#endif