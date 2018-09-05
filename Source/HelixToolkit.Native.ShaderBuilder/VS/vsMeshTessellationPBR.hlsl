#ifndef VSMESHTESSELLATIONPBR_HLSL
#define VSMESHTESSELLATIONPBR_HLSL

#define MESH
#define PBR
#include"vsMeshTessellation.hlsl"
#pragma pack_matrix( row_major )

//--------------------------------------------------------------------------------------
// VERTEX SHADER function
// called per incoming vertex/control point
//--------------------------------------------------------------------------------------
HSInput mainPBR(VSInput input)
{
    return main(input);
}
#endif