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
    return float4(((pos.x * 2.0 * vViewport.z) - 1.0) * w,
                  ((pos.y * 2.0 * vViewport.w) - 1.0) * -w,
                  z, w);
}

//--------------------------------------------------------------------------------------
// From window pixel pos to normalized device coordinates.
//--------------------------------------------------------------------------------------
float2 windowToNdc(in float2 pos)
{
    return float2((pos.x * vViewport.z) * 2.0, (pos.y * vViewport.w) * 2.0);
}

float whengt(float l, float cmp)
{
    return max(sign(l - cmp), 0.0);
}

float whenlt(float l, float cmp)
{
    return max(sign(cmp - l), 0.0);
}

float whenge(float l, float cmp)
{
    return 1.0 - whenlt(l, cmp);
}

float whenle(float l, float cmp)
{
    return 1.0 - whengt(l, cmp);
}

float when_eq(float x, float y)
{
    return 1.0 - abs(sign(x - y));
}

float when_neq(float x, float y)
{
    return abs(sign(x - y));
}

float3 whengt(float3 l, float3 cmp)
{
    return max(sign(l - cmp), 0.0);
}

float3 whenlt(float3 l, float3 cmp)
{
    return max(sign(cmp - l), 0.0);
}

float3 whenge(float3 l, float3 cmp)
{
    return 1.0 - whenlt(l, cmp);
}

float3 when_neq(float3 x, float3 y)
{
    return abs(sign(x - y));
}

float3 whenle(float3 l, float3 cmp)
{
    return 1.0 - whengt(l, cmp);
}

float3 when_eq(float3 x, float3 y)
{
    return 1.0 - abs(sign(x - y));
}

float3 and(float3 a, float3 b)
{
    return a * b;
}

float3 or(float3 a, float3 b)
{
    return min(a + b, 1.0);
}

float3 xor(float3 a, float3 b)
{
    return (a + b) % 2.0;
}

float3 not(float3 a)
{
    return 1.0 - a;
}

float4 whengt(float4 l, float4 cmp)
{
    return max(sign(l - cmp), 0.0);
}

float4 whenlt(float4 l, float4 cmp)
{
    return max(sign(cmp - l), 0.0);
}

float4 whenge(float4 l, float4 cmp)
{
    return 1.0 - whenlt(l, cmp);
}

float4 when_neq(float4 x, float4 y)
{
    return abs(sign(x - y));
}

float4 whenle(float4 l, float4 cmp)
{
    return 1.0 - whengt(l, cmp);
}

float4 when_eq(float4 x, float4 y)
{
    return 1.0 - abs(sign(x - y));
}

float4 and(float4 a, float4 b)
{
    return a * b;
}

float4 or(float4 a, float4 b)
{
    return min(a + b, 1.0);
}

float4 xor(float4 a, float4 b)
{
    return (a + b) % 2.0;
}

float4 not(float4 a)
{
    return 1.0 - a;
}


static const float4 encode = float4(16777216, 1.0 / 65536, 1.0 / 256, 1.0 / 255.0); 
float4 FloatToRGB(float v)
{
    uint vi = (uint) v;
    float4 c = (float4)0;
    c.r = vi >> 16;
    c.g = vi >> 8 & 0xFF;
    c.b = vi & 0xFF;
    c.rgb *= encode.w;
    c.a = 1;
    return c;
}

float RGBToFloat(float4 c)
{
    c *= 255;
    uint v = (uint) c.r << 16 + (uint) c.g << 8 + (uint) c.b;
    return v;
}

float3 getLookDir(float4x4 viewMatrix)
{
    return viewMatrix._m02_m12_m22;
}
#ifdef VOLUME
float4 getBoxEntryPoint(float3 localCamPos, float3 localCamVec, VolumePS_INPUT input)
{
    const float3 off = float3(0.5, 0.5, 0.5);
    float steps = floor(1 / stepSize * actualSampleDist);
    localCamPos += off;
    float3 invRayDir = 1 / localCamVec;
    float3 firstIntersections = (0 - localCamPos) * invRayDir;
    float3 secondIntersections = (1 - localCamPos) * invRayDir;
    float3 closest = min(firstIntersections, secondIntersections);
    float3 furthest = max(firstIntersections, secondIntersections);
    
    float t0 = max(closest.x, max(closest.y, closest.z));
    float t1 = min(furthest.x, min(furthest.y, furthest.z));
   
    float planeOffset = 1 - frac((t0 - length(localCamPos) - 0.5) * steps);
    t0 += (planeOffset / steps) * enablePlaneAlignment;
    t0 = max(0, t0);
    float thickness = max(0, t1 - t0);
    float3 entry = localCamPos + max(0, t0) * localCamVec;

    // Determine actual depth. If opaque objects is inside volume, ray should end after reaching the opaque object surface.
    float2 texB = input.pos.xy / vViewport.xy;
    float3 front = mul(float4(texVolumeBack.Sample(samplerVolume, texB).xyz, 1), mWorldInv).xyz + off;
    float3 v = entry - front;
    float d = max(0, dot(normalize(v), -localCamVec));
    thickness = d * min(length(v), thickness); 
    return float4(entry, thickness);
}
#endif
#endif