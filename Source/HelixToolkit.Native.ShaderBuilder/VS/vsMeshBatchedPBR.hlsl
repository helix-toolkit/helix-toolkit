#ifndef VSMESHBATCHEDPBR_HLSL
#define VSMESHBATCHEDPBR_HLSL

#define MESH
#define PBR
#include"vsMeshBatched.hlsl"
#pragma pack_matrix( row_major )

PSInput mainPBR(VSInputBatched input)
{
    return main(input);
}

#endif