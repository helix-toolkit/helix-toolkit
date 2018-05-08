//--------------------------------------------------------------------------------------
// Vertex Shader
//--------------------------------------------------------------------------------------
#define SCREENDUPLICATION
#include"..\Common\CommonBuffers.hlsl"

#pragma pack_matrix( row_major )

static const float2 quadtexcoords[4] =
{
    float2(1, 0),
    float2(0, 0),
    float2(1, 1),
    float2(0, 1),
};


ScreenDupVS_INPUT main(uint vI : SV_VERTEXID)
{
    ScreenDupVS_INPUT output = (ScreenDupVS_INPUT) 0;

    output.Tex = quadtexcoords[vI];
    output.Pos = CursorVertCoord[vI];
    return output;
}

