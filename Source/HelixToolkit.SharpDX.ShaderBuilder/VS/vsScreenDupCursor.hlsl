//--------------------------------------------------------------------------------------
// Vertex Shader
//--------------------------------------------------------------------------------------
#define SCREENDUPLICATION
#include"..\Common\CommonBuffers.hlsl"

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

    output.Tex = quadtexcoords[vI];

    if (vI == 0)
    {
        output.Pos.x = ((CursorRect.x + CursorRect.z) - DesktopCenter.x) / DesktopCenter.x;
        output.Pos.y = -1 * (CursorRect.y - DesktopCenter.y) / DesktopCenter.y;
        output.Pos.w = 1;
    }
    else if (vI == 1)
    {
        output.Pos.x = (CursorRect.x - DesktopCenter.x) / DesktopCenter.x;
        output.Pos.y = -1 * (CursorRect.y - DesktopCenter.y) / DesktopCenter.y;
        output.Pos.w = 1;
    }
    else if (vI == 2)
    {
        output.Pos.x = ((CursorRect.x + CursorRect.z) - DesktopCenter.x) / DesktopCenter.x;
        output.Pos.y = -1 * ((CursorRect.y + CursorRect.w) - DesktopCenter.y) / DesktopCenter.y;
        output.Pos.w = 1;
    }
    else if (vI == 3)
    {
        output.Pos.x = (CursorRect.x - DesktopCenter.x) / DesktopCenter.x;
        output.Pos.y = -1 * ((CursorRect.y + CursorRect.w) - DesktopCenter.y) / DesktopCenter.y;
        output.Pos.w = 1;
    }
    return output;
}

