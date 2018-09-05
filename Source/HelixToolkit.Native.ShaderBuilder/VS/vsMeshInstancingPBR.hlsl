#ifndef VSMESHINSTANCINGPBR_HLSL
#define VSMESHINSTANCINGPBR_HLSL

#define MESH
#define PBR
#include"vsMeshInstancing.hlsl"
#pragma pack_matrix( row_major )

PSInput mainPBR(VSInstancingInput input)
{
    return main(input);

}

#endif