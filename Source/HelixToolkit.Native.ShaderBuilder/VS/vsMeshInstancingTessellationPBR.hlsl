#ifndef VSMESHINSTANCINGTESSELLATIONPBR_HLSL
#define VSMESHINSTANCINGTESSELLATIONPBR_HLSL

#define MESH
#define PBR
#include"vsMeshInstancingTessellation.hlsl"

#pragma pack_matrix( row_major )

//--------------------------------------------------------------------------------------
// VERTEX SHADER function
// called per incoming vertex/control point
//--------------------------------------------------------------------------------------
HSInput mainPBR(VSInstancingInput input)
{
    return main(input);

}
#endif