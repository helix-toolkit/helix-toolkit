//--------------------------------------------------------------------------------------
// File: Deferred G-Buffer Rendering for HelixToolkitDX
// Author: Przemyslaw Musialski
// Date: 03/03/13
// References & Sources: 
// code based on: http://hieroglyph3.codeplex.com/
//--------------------------------------------------------------------------------------

#include "Common.fx"
#include "Material.fx"
#include "DeferredLighting.fx"


//--------------------------------------------------------------------------------------
//VERTEX AND PIXEL SHADER INPUTS
//--------------------------------------------------------------------------------------
struct VSInput
{
	float4 p					: POSITION;
	float4 c					: COLOR;
	float2 t					: TEXCOORD; 
	float3 n					: NORMAL;  
	float3 t1					: TANGENT;
    float3 t2					: BINORMAL;

	float4 mr0					: TEXCOORD1;
	float4 mr1					: TEXCOORD2;
	float4 mr2					: TEXCOORD3;
	float4 mr3					: TEXCOORD4;
};

struct PSInput
{
	float4 p					: SV_POSITION;  
	float4 wp					: POSITION0;
	float4 sp					: TEXCOORD1;
	float3 n					: NORMAL;	    // normal
	float2 t					: TEXCOORD0;	// tex coord	
	float3 t1					: TANGENT;		// tangent
    float3 t2					: BINORMAL;		// bi-tangent	
	float4 c					: COLOR;		// solid color (for debug)
};

struct PSOutput
{
	float4 Normal				: SV_Target0;
	float4 DiffuseAlbedo 		: SV_Target1;
	float4 SpecularAlbedo 		: SV_Target2;
	float4 Position				: SV_Target3;
};

//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING  - Vertex Shader
//--------------------------------------------------------------------------------------
PSInput VShaderDeferred( VSInput input )
{
	PSInput output = (PSInput)0;
	float4 inputp  = input.p;	
    if (bInvertNormal)
    {
        input.n = -input.n;
    }
	// compose instance matrix
	if(bHasInstances)
	{
		matrix mInstance =  
		{	
			input.mr0.x, input.mr1.x, input.mr2.x, input.mr3.x, // row 1
			input.mr0.y, input.mr1.y, input.mr2.y, input.mr3.y, // row 2
			input.mr0.z, input.mr1.z, input.mr2.z, input.mr3.z, // row 3
			input.mr0.w, input.mr1.w, input.mr2.w, input.mr3.w, // row 4
		};	
		inputp = mul( mInstance, input.p );
	}
	
	//set position into camera clip space	
	output.p = mul( inputp, mWorld );	
	output.wp = output.p;
	output.p = mul( output.p, mView );    
	output.p = mul( output.p, mProjection );	

	//set texture coords and color
	output.t = input.t;	
	output.c = input.c;
	
	//set normal for interpolation	
	output.n = normalize( mul(input.n, (float3x3)mWorld) );

	if(bHasNormalMap)
	{
		// transform the tangents by the world matrix and normalize
		output.t1 = normalize( mul(input.t1, (float3x3)mWorld) );				
		output.t2 = normalize( mul(input.t2, (float3x3)mWorld) );		
	}
	else
	{
		output.t1 = 0.0f;
		output.t2 = 0.0f;
	}
	    
	return output;  
}


//-------------------------------------------------------------------------------------------------
// Basic pixel shader, no optimizations
//-------------------------------------------------------------------------------------------------
PSOutput PShaderDeferred( in PSInput input )
{
	PSOutput output = (PSOutput)0;

	float4 diffuse = vMaterialDiffuse; 
	if(bHasDiffuseMap)
	{	
		diffuse *=  texDiffuseMap.Sample( NormalSampler, input.t);	
	}

	// Sample the tangent-space normal map and decompress
	float3 normalWS;
	if(bHasNormalMap)
	{
		// Normalize the per-pixel interpolated tangent-space
		input.n  = normalize( input.n );
		input.t1 = normalize( input.t1 );
		input.t2 = normalize( input.t2 );		

		// Sample the texel in the bump map and expand the range of the normal value from (0, +1) to (-1, +1).
		float3 bumpMap = texNormalMap.Sample(NormalSampler, input.t).xyz * 2.0f - 1.0f;		
		// Calculate the normal from the data in the bump map.
		normalWS = normalize( input.n + bumpMap.x * input.t1 + bumpMap.y * input.t2 );
	}
	else
	{
		normalWS = normalize( input.n );
	}

	// Output our G-Buffer values
	output.Normal			= float4( normalWS, 1.0f );	
	output.DiffuseAlbedo	= float4( diffuse.xyz, vMaterialAmbient.r );
	output.SpecularAlbedo	= float4( vMaterialSpecular.xyz, sMaterialShininess );	
	output.Position			= float4( input.wp );	

	return output;
}


//--------------------------------------------------------------------------------------
// Techniques
//--------------------------------------------------------------------------------------
technique11 RenderDeferred
{
    pass P0
    {
		SetRasterizerState	( RSSolid );
		SetDepthStencilState( DSSDepthLess, 0);
		//SetBlendState( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );

        SetVertexShader		( CompileShader( vs_5_0, VShaderDeferred() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_5_0, PShaderDeferred() ) );        
    }    
}

technique11 RenderGBuffer
{
    pass P0
    {
		SetRasterizerState	( RSSolid );
		SetDepthStencilState( DSSDepthLess, 0);
		//SetBlendState( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );

        SetVertexShader		( CompileShader( vs_5_0, VShaderDeferred() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_5_0, PShaderDeferred() ) );        
    }    
}

