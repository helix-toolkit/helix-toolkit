#ifndef VSFULLSCREENQUAD_HLSL
#define VSFULLSCREENQUAD_HLSL

#define CLIPPLANE
#include"..\Common\DataStructs.hlsl"
#include"..\Common\CommonBuffers.hlsl"

#pragma pack_matrix( row_major )

static const float2 quadtexcoords[4] =
{
    float2(1, 0),
    float2(0, 0),
    float2(1, 1),
    float2(0, 1),
};

PSInputScreenQuad main(uint vI : SV_VERTEXID)
{
    float2 texcoord = quadtexcoords[vI];
    PSInputScreenQuad output = (PSInputScreenQuad) 0;
	output.p = float4((texcoord.x - 0.5f) * 2, -(texcoord.y - 0.5f) * 2, 0, 1);
    output.c = CrossSectionColors;
    return output;
}

#endif