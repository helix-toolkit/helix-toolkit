#ifndef COMMON_HLSL
#define COMMON_HLSL
#include"CommonBuffers.hlsl"

#pragma pack_matrix( row_major )


//--------------------------------------------------------------------------------------
// GLOBAL FUNCTIONS
//--------------------------------------------------------------------------------------

//--------------------------------------------------------------------------------------
// From projection frame to window pixel pos.
//--------------------------------------------------------------------------------------
float2 projToWindow(in float4 pos)
{
    return float2(vViewport.x * 0.5 * (1.0 + (pos.x / pos.w)),
                  vViewport.y * 0.5 * (1.0 - (pos.y / pos.w)));
}

//--------------------------------------------------------------------------------------
// From window pixel pos to projection frame at the specified z (view frame). 
//--------------------------------------------------------------------------------------
float4 windowToProj(in float2 pos, in float z, in float w)
{
    return float4(((pos.x * 2.0 / vViewport.x) - 1.0) * w,
                  ((pos.y * 2.0 / vViewport.y) - 1.0) * -w,
                  z, w);
}

//--------------------------------------------------------------------------------------
// From window pixel pos to normalized device coordinates.
//--------------------------------------------------------------------------------------
float2 windowToNdc(in float2 pos)
{
    return float2((pos.x / vViewport.x) * 2.0, (pos.y / vViewport.y) * 2.0);
}

int whengt(float l, float cmp)
{
    return max(sign(l - cmp), 0);
}

int whenlt(float l, float cmp)
{
    return max(sign(cmp - l), 0);
}

int whenge(float l, float cmp)
{
    return 1 - whenlt(l, cmp);
}

int whenle(float l, float cmp)
{
    return 1 - whengt(l, cmp);
}


#endif