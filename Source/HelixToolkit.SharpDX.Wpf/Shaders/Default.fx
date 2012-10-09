//--------------------------------------------------------------------------------------
// File: Tutorial06.fx
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------


//--------------------------------------------------------------------------------------
// Constant Buffer Variables
//--------------------------------------------------------------------------------------
matrix World;
matrix View;
matrix Projection;
float4 vLightDir[2];
float4 vLightColor[2];
float4 vOutputColor;
float4 Overlay : OverlayColor;

//--------------------------------------------------------------------------------------
Texture2D texDiffuse;
SamplerState sampleLinear
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};


//--------------------------------------------------------------------------------------
struct VS_INPUT
{
    float4 Pos : POSITION;
	float4 Col : COLOR;
	float2 Tex : TEXCOORD;
    float3 Nor : NORMAL;	
};

struct PS_INPUT
{
    float4 Pos : SV_POSITION;
	float4 Col : COLOR;
	float2 Tex : TEXCOORD0;
    float3 Nor : TEXCOORD1;	
};


//--------------------------------------------------------------------------------------
// Vertex Shader
//--------------------------------------------------------------------------------------
PS_INPUT VS( VS_INPUT input )
{
    PS_INPUT output = (PS_INPUT)0;
    output.Pos = mul( input.Pos, World );
    output.Pos = mul( output.Pos, View );
    output.Pos = mul( output.Pos, Projection );
    output.Nor = mul( input.Nor, World );
    output.Col = input.Col * Overlay;
	output.Tex = input.Tex;

    return output;
}


//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------
float4 PS( PS_INPUT input) : SV_Target
{
	float4 texColor = texDiffuse.Sample( sampleLinear, input.Tex );	
    float4 finalColor = input.Col*texColor;
    
    /// --- do NdotL lighting for 2 lights
    for(int i=0; i<2; i++)
    {
        finalColor += saturate( dot( (float3)vLightDir[i], input.Nor) * vLightColor[i] );
    }
    finalColor.a = 1;
    return finalColor;	
}


//--------------------------------------------------------------------------------------
// PSSolid - render a solid color
//--------------------------------------------------------------------------------------
float4 PSSolid( PS_INPUT input) : SV_Target
{
    return vOutputColor;
}


//--------------------------------------------------------------------------------------
technique10 Render
{
    pass P0
    {
        SetVertexShader( CompileShader( vs_4_0, VS() ) );
        SetGeometryShader( NULL );
        SetPixelShader( CompileShader( ps_4_0, PS() ) );
    }
}


//--------------------------------------------------------------------------------------
technique10 RenderLight
{
    pass P0
    {
        SetVertexShader( CompileShader( vs_4_0, VS() ) );
        SetGeometryShader( NULL );
        SetPixelShader( CompileShader( ps_4_0, PSSolid() ) );
    }
}

