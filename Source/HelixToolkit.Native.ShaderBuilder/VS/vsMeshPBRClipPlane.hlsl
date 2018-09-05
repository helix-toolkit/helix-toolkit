#ifndef VSMESHPBRCLIPPLANE_HLSL
#define VSMESHPBRCLIPPLANE_HLSL
#define CLIPPLANE
#define MESH
#define PBR
#include"vsMeshClipPlane.hlsl"
#pragma pack_matrix( row_major )

PSInputClip mainPBR(VSInput input)
{
    return main(input);

}

#endif