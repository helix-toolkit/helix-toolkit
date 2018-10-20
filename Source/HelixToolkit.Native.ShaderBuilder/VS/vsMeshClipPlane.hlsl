#ifndef VSMESHCLIPPLANE_HLSL
#define VSMESHCLIPPLANE_HLSL
#define CLIPPLANE
#define MESH
#include"vsMeshDefault.hlsl"
#pragma pack_matrix( row_major )

PSInputClip mainClip(VSInput input)
{
    return main(input);

}
#endif