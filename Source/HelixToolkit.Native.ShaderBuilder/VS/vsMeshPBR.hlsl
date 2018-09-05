#ifndef VSMESHPBR_HLSL
#define VSMESHPBR_HLSL

#define MESH
#define PBR
#include"vsMeshDefault.hlsl"

#pragma pack_matrix( row_major )

PSInput mainPBR(VSInput input)
{
    return main(input);
}

#endif