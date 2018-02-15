#define MESH
#include"..\Common\CommonBuffers.hlsl"

float4 main(PSInput input) : SV_Target
{
    return wireframeColor;
}