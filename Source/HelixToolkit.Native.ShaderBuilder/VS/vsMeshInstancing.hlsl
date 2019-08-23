#ifndef VSMESHINSTANCING_HLSL
#define VSMESHINSTANCING_HLSL

#define MESH
#define INSTANCINGPARAM
#include"vsMeshDefault.hlsl"
#pragma pack_matrix( row_major )

//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING  - Vertex Shader
//--------------------------------------------------------------------------------------
PSInput mainInstancing(VSInstancingInput input)
{
    return main(input);
}

#endif