//--------------------------------------------------------------------------------------
// Vertex Shader
//--------------------------------------------------------------------------------------
#include"..\Common\DataStructs.hlsl"

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
    float2 vertCoord = quadtexcoords[vI];
    output.Pos = float4((vertCoord.x - 0.5f) * 2, -(vertCoord.y - 0.5f) * 2, 0, 1);
    output.Tex = quadtexcoords[vI];
    return output;
}

