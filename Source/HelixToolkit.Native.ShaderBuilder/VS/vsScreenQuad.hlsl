//--------------------------------------------------------------------------------------
// Vertex Shader
//--------------------------------------------------------------------------------------
#define SCREENQUAD
#include"..\Common\CommonBuffers.hlsl"

#pragma pack_matrix( row_major )

ScreenDupVS_INPUT main(uint vI : SV_VERTEXID)
{
    ScreenDupVS_INPUT output = (ScreenDupVS_INPUT) 0;
    float3x3 transform = float3x3(mWorld._11_12_13, mWorld._21_22_23, mWorld._31_32_33);
    output.Tex = TextureCoord[vI].xy;
    float2 zw = VertCoord[vI].zw;
    output.Pos = float4(mul(VertCoord[vI].xyz, transform).xy + mWorld._41_42, zw);
    return output;
}

