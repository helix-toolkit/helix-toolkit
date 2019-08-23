#ifndef VSMESHINSTANCINGTESSELLATION_HLSL
#define VSMESHINSTANCINGTESSELLATION_HLSL

#define MESH
#define INSTANCINGPARAM
#include"vsMeshTessellation.hlsl"
#pragma pack_matrix( row_major )

//--------------------------------------------------------------------------------------
// VERTEX SHADER function
// called per incoming vertex/control point
//--------------------------------------------------------------------------------------
HSInput mainInstancing(VSInstancingInput input)
{
    return main(input);

}
#endif