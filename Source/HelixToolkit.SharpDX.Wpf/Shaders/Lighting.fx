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
struct VS_INPUT
{
    float4 Pos : POSITION;
	float4 Col : COLOR;
    float3 Norm : NORMAL;	
};

struct PS_INPUT
{
    float4 Pos : SV_POSITION;
	float4 Col : COLOR;
    float3 Norm : TEXCOORD0;	
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
    output.Norm = mul( input.Norm, World );
    output.Col = input.Col * Overlay;

    return output;
}


//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------
float4 PS( PS_INPUT input) : SV_Target
{
    float4 finalColor = input.Col;
    
    /// --- do NdotL lighting for 2 lights
    for(int i=0; i<2; i++)
    {
        finalColor += saturate( dot( (float3)vLightDir[i], input.Norm) * vLightColor[i] );
    }
    finalColor.a = 1;
    return finalColor;
	//return input.Col;
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

