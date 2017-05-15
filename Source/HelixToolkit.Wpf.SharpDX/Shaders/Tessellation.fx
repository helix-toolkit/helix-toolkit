//--------------------------------------------------------------------------------------
// File: Tessellation Functions for HelixToolkitDX
// Author: Przemyslaw Musialski
// Date: 03/21/13
// References & Sources: Based on NVidia SDK 2011 
//--------------------------------------------------------------------------------------

//--------------------------------------------------------------------------------------
// Work-around for an optimization rule problem in the June 2010 HLSL Compiler
// (9.29.952.3111)
// see http://support.microsoft.com/kb/2448404
//--------------------------------------------------------------------------------------
#if D3DX_VERSION == 0xa2b
#pragma ruledisable 0x0802405f
#endif 

//--------------------------------------------------------------------------------------
// pre-processor includes
//--------------------------------------------------------------------------------------
#include "Effects.fx"

//--------------------------------------------------------------------------------------
// SHADER STRUCTURES
//--------------------------------------------------------------------------------------
struct HSConstantDataOutput
{
    float Edges[3]  : SV_TessFactor;
    float Inside    : SV_InsideTessFactor;
	float Sign      : SIGN;
   
    float3 f3B210   : POSITION3;
    float3 f3B120   : POSITION4;
    float3 f3B021   : POSITION5;
    float3 f3B012   : POSITION6;
    float3 f3B102   : POSITION7;
    float3 f3B201   : POSITION8;
    float3 f3B111   : CENTER;   
};

//--------------------------------------------------------------------------------------
struct HSConstantDataOutputQuads
{
    float  Edges[4]			: SV_TessFactor;
    float  Inside[2]		: SV_InsideTessFactor;
	float  Sign				: SIGN;
	float3 vEdgePos[8]		: EDGEPOS;
    float3 vInteriorPos[4]	: INTERIORPOS;
};

//--------------------------------------------------------------------------------------
struct HSInput
{
    float3 p			: POSITION;
    float2 t			: TEXCOORD0;
    float3 n			: TEXCOORD1;
    float3 t1		    : TEXCOORD2;
	float3 t2	        : TEXCOORD3;
	float4 c			: COLOR;
};

//--------------------------------------------------------------------------------------
struct VSIn
{
	float4 p			: POSITION;
	float4 c			: COLOR;
	float2 t			: TEXCOORD; 
	float3 n			: NORMAL;  
	float3 t1			: TANGENT;
    float3 t2			: BINORMAL;
};


//--------------------------------------------------------------------------------------
// Bernstein-Polynomial Evaluation helper function
// Evaluates the Cubic Bernstein-Polynomial at parametric position t
//------------------------------------------------------------------------------------
float4 BernsteinBasis(float u)
{
    float i = 1.0f - u;

    return float4( i * i * i,
                   3 * u * i * i,
                   3 * u * u * i,
                   u * u * u );
}


//------------------------------------------------------------------------------------
// Bezier-Evaluation helper function
//--------------------------------------------------------------------------------------
//  p0 - p4 - p8 - p12
//  |    |    |    |
//  p1 - p5 - p9 - p13
//  |    |    |    |
//  p2 - p6 - p10- p14
//  |    |    |    |
//  p3 - p7 - p11- p15
//------------------------------------------------------------------------------------
float3 EvaluateBezier( float3  p0, float3  p1, float3  p2, float3  p3,
                       float3  p4, float3  p5, float3  p6, float3  p7,
                       float3  p8, float3  p9, float3 p10, float3 p11,
                       float3 p12, float3 p13, float3 p14, float3 p15,
                       float4 basisU,
                       float4 basisV )
{
    float3 value;
    value  = basisV.x * (  p0 * basisU.x +  p1 * basisU.y +  p2 * basisU.z +  p3 * basisU.w );
    value += basisV.y * (  p4 * basisU.x +  p5 * basisU.y +  p6 * basisU.z +  p7 * basisU.w );
    value += basisV.z * (  p8 * basisU.x +  p9 * basisU.y + p10 * basisU.z + p11 * basisU.w );
    value += basisV.w * ( p12 * basisU.x + p13 * basisU.y + p14 * basisU.z + p15 * basisU.w );
	return value;  
}


//--------------------------------------------------------------------------------------
// VERTEX SHADER function
// called per incoming vertex/control point
//--------------------------------------------------------------------------------------
HSInput VShaderTessellated( VSIn input )
{
    HSInput output;        
    output.p		= input.p.xyz;
    output.t		= input.t;       
    output.n		= input.n; 
    output.t1		= input.t1;
	output.t2		= input.t2;
	output.c		= input.c;
    return output;
}

//--------------------------------------------------------------------------------------
// HULL SHADER main function
// called per output-control point
//--------------------------------------------------------------------------------------
[domain("tri")]
[partitioning("integer")]
[outputtopology("triangle_cw")]
[outputcontrolpoints(3)]
[patchconstantfunc("HShaderTriConstant")]
[maxtessfactor(64.0)]
HSInput HShaderTriMain( InputPatch<HSInput, 3> inputPatch, uint cpID : SV_OutputControlPointID, uint patchID : SV_PrimitiveID)
{
    return inputPatch[cpID];
}

[domain("quad")]
[partitioning("integer")]
[outputtopology("triangle_cw")]
[outputcontrolpoints(4)]
[patchconstantfunc("HShaderQuadConstant")]
[maxtessfactor(64.0)]
HSInput HShaderQuadMain( InputPatch<HSInput, 4> inputPatch, uint cpID : SV_OutputControlPointID, uint patchID : SV_PrimitiveID)
{
    return inputPatch[cpID];
}

//--------------------------------------------------------------------------------------
// HULL SHADER constant function for triangular patches
// called per patch
//--------------------------------------------------------------------------------------
HSConstantDataOutput HShaderTriConstant( InputPatch<HSInput, 3> inputPatch)
{    
	HSConstantDataOutput output;

	// edge control points
    output.f3B210 = 1;
    output.f3B120 = 1;
    output.f3B021 = 1;
    output.f3B012 = 1;
    output.f3B102 = 1;
    output.f3B201 = 1;

    // center control point
    output.f3B111 = 1;

	// tessellation factors
	[unroll]
	for (uint ie = 0; ie < 3; ++ie)
	{
		output.Edges[ie] = vTessellation.x;
	}
	output.Inside = (output.Edges[0] + output.Edges[1] + output.Edges[2]) / 3;

 
	// edge control points
    output.f3B210 = ( ( 2.0f * inputPatch[0].p ) + inputPatch[1].p - ( dot( ( inputPatch[1].p - inputPatch[0].p ), inputPatch[0].n ) * inputPatch[0].n ) ) / 3.0f;
    output.f3B120 = ( ( 2.0f * inputPatch[1].p ) + inputPatch[0].p - ( dot( ( inputPatch[0].p - inputPatch[1].p ), inputPatch[1].n ) * inputPatch[1].n ) ) / 3.0f;
    output.f3B021 = ( ( 2.0f * inputPatch[1].p ) + inputPatch[2].p - ( dot( ( inputPatch[2].p - inputPatch[1].p ), inputPatch[1].n ) * inputPatch[1].n ) ) / 3.0f;
    output.f3B012 = ( ( 2.0f * inputPatch[2].p ) + inputPatch[1].p - ( dot( ( inputPatch[1].p - inputPatch[2].p ), inputPatch[2].n ) * inputPatch[2].n ) ) / 3.0f;
    output.f3B102 = ( ( 2.0f * inputPatch[2].p ) + inputPatch[0].p - ( dot( ( inputPatch[0].p - inputPatch[2].p ), inputPatch[2].n ) * inputPatch[2].n ) ) / 3.0f;
    output.f3B201 = ( ( 2.0f * inputPatch[0].p ) + inputPatch[2].p - ( dot( ( inputPatch[2].p - inputPatch[0].p ), inputPatch[0].n ) * inputPatch[0].n ) ) / 3.0f;
    // center control point
    float3 f3E = ( output.f3B210 + output.f3B120 + output.f3B021 + output.f3B012 + output.f3B102 + output.f3B201 ) / 6.0f;
    float3 f3V = ( inputPatch[0].p + inputPatch[1].p + inputPatch[2].p ) / 3.0f;
    output.f3B111 = f3E + ( ( f3E - f3V ) / 2.0f );

	// -- culling in HS
    float2 t01 = inputPatch[1].t - inputPatch[0].t;
    float2 t02 = inputPatch[2].t - inputPatch[0].t;
    output.Sign = t01.x * t02.y - t01.y * t02.x > 0.0f ? 1 : -1;

    return output;
}

//--------------------------------------------------------------------------------------
// HULL SHADER constant function for quad patches
// called per patch
//--------------------------------------------------------------------------------------
HSConstantDataOutputQuads HShaderQuadConstant( InputPatch<HSInput, 4> inputPatch)
{    
    HSConstantDataOutputQuads output = (HSConstantDataOutputQuads)0;

	// tessellation factors are proportional to model space edge length	
	[unroll]
	for (int ie = 0; ie < 4; ++ie)
	{
		output.Edges[ie] = vTessellation.x;
	}
	output.Inside[1] = (output.Edges[0] + output.Edges[2]) / 2;
	output.Inside[0] = (output.Edges[1] + output.Edges[3]) / 2;

	// edge-points
    [unroll]
    for (int iedge = 0; iedge < 4; ++iedge)
    {
        int i = iedge;
        int j = (iedge + 1) & 3;
        float3 vPosTmp = 0;

        vPosTmp = inputPatch[j].p - inputPatch[i].p;
        output.vEdgePos[iedge * 2] = (2 * inputPatch[i].p + inputPatch[j].p - dot(vPosTmp, inputPatch[i].n) * inputPatch[i].n) / 3;

        i = j;
        j = iedge;
        vPosTmp = inputPatch[j].p - inputPatch[i].p;
        output.vEdgePos[iedge * 2 + 1] = (2 * inputPatch[i].p + inputPatch[j].p - dot(vPosTmp, inputPatch[i].n) * inputPatch[i].n) / 3;
    }

    // interior-points
    float3 q = output.vEdgePos[0];
	
	[unroll]
    for (int i = 1; i < 8; ++i)
    {
        q += output.vEdgePos[i];
    }
    float3 center = inputPatch[0].p + inputPatch[1].p + inputPatch[2].p + inputPatch[3].p;

	[unroll]
    for (i = 0; i < 4; ++i)
    {
        float3 Ei = (2 * (output.vEdgePos[i * 2] + output.vEdgePos[((i + 3) & 3) * 2 + 1] + q) - (output.vEdgePos[((i + 1) & 3) * 2 + 1] + output.vEdgePos[((i + 2) & 3) * 2])) / 18;
        float3 Vi = (center + 2 * (inputPatch[(i + 3) & 3].p + inputPatch[(i + 1) & 3].p) + inputPatch[(i + 2) & 3].p) / 9;
        output.vInteriorPos[i] = 3./2 * Ei - 1./2 * Vi;
    }

    float2 t01 = inputPatch[1].t - inputPatch[0].t;
    float2 t02 = inputPatch[2].t - inputPatch[0].t;
    output.Sign = t01.x * t02.y - t01.y * t02.x > 0.0f ? 1 : -1;

    return output;
}




//--------------------------------------------------------------------------------------
// DOMAIN SHADER function for triangle-patches
// called per uv-coordinate/ouput vertex
//--------------------------------------------------------------------------------------
[domain("tri")]
PSInput DShaderTri(	HSConstantDataOutput input, float3 barycentricCoords : SV_DomainLocation, OutputPatch<HSInput, 3> inputPatch )
{
    PSInput output = (PSInput)0;

    // --- The barycentric coordinates
    float fU = barycentricCoords.x;
    float fV = barycentricCoords.y;
    float fW = barycentricCoords.z;

    // --- Precompute squares and squares * 3 
    float fUU = fU * fU;
    float fVV = fV * fV;
    float fWW = fW * fW;
    float fUU3 = fUU * 3.0f;
    float fVV3 = fVV * 3.0f;
    float fWW3 = fWW * 3.0f;

	// --- Compute position from cubic control points and barycentric coords
    float3 position =	inputPatch[0].p * fWW * fW + 
						inputPatch[1].p * fUU * fU +
						inputPatch[2].p * fVV * fV +
						input.f3B210 * fWW3 * fU + 
						input.f3B120 * fW * fUU3 + 
						input.f3B201 * fWW3 * fV + 
						input.f3B021 * fUU3 * fV +
						input.f3B102 * fW * fVV3 + 
						input.f3B012 * fU * fVV3 + 
						input.f3B111 * 6.0f * fW * fU * fV;

	// Compute normal from barycentric coords
    output.n		= normalize( inputPatch[0].n * barycentricCoords.z + inputPatch[1].n * barycentricCoords.x + inputPatch[2].n * barycentricCoords.y );    
	
	// Compute tangent-space
    output.t1		= normalize( inputPatch[0].t1 * barycentricCoords.z + inputPatch[1].t1 * barycentricCoords.x + inputPatch[2].t1 * barycentricCoords.y );
	output.t2		= normalize( inputPatch[0].t2 * barycentricCoords.z + inputPatch[1].t2 * barycentricCoords.x + inputPatch[2].t2 * barycentricCoords.y );
		
	// --- interpolate texture coordinates
    output.t = inputPatch[0].t * barycentricCoords.z + inputPatch[1].t * barycentricCoords.x + inputPatch[2].t * barycentricCoords.y;
	
	// ---  interpolated per-vertex colors
	output.c		= inputPatch[0].c * barycentricCoords.z + inputPatch[1].c * barycentricCoords.x + inputPatch[2].c * barycentricCoords.y;
    
	// --- Classical vertex-shader transforms: 
	// --- output position in the clip-space	
	output.p		= mul( float4(position, 1.0f),	mWorld );		
	output.wp		= output.p;
	output.p		= mul( output.p, mView );    
	output.p		= mul( output.p, mProjection );	

	// --- interpolated normals    
    output.n		= normalize( mul(output.n,  (float3x3)mWorld) );
    output.t1		= normalize( mul(output.t1, (float3x3)mWorld) );
	output.t2		= normalize( mul(output.t2, (float3x3)mWorld) );
    	
    return output;
}

//--------------------------------------------------------------------------------------
// DOMAIN SHADER function for quad-patches
// called per uv-coordinate/ouput vertex
//--------------------------------------------------------------------------------------
[domain("quad")]
PSInput DShaderQuad(HSConstantDataOutputQuads input, float2 uv : SV_DomainLocation, OutputPatch<HSInput, 4> inputPatch )
{
    PSInput output = (PSInput)0;

	// --- get Bernstein basis
	float4 basisU = BernsteinBasis( uv.x );
    float4 basisV = BernsteinBasis( uv.y );

	// hint: the order of patch points provided to the evaluation func. EvaluateBezier(...);
	// p0 - e7 - e6 - p3           p0 - p4 - p8 - p12
	// |    |    |    |		       |    |    |    |
	// e0 - i0 - i3 - e5	       p1 - p5 - p9 - p13
	// |    |    |    |		--->   |    |    |    |
	// e1 - i1 - i2 - e4	       p2 - p6 - p10- p14
	// |    |    |    |		       |    |    |    |
	// p1 - e2 - e3 - p2	       p3 - p7 - p11- p15
    float3 position = EvaluateBezier(
		inputPatch[0].p,		//p0
		input.vEdgePos[0],		//e0
		input.vEdgePos[1],		//e1
		inputPatch[1].p,		//p1
                                       
		input.vEdgePos[7],      //e7
		input.vInteriorPos[0],  //i0
		input.vInteriorPos[1],  //i1
		input.vEdgePos[2],	    //e2
                                       
		input.vEdgePos[6],      //e6
		input.vInteriorPos[3], 	//i3
		input.vInteriorPos[2], 	//i2
		input.vEdgePos[3],		//e3
                                       
		inputPatch[3].p,		//p3
		input.vEdgePos[5], 		//e5
		input.vEdgePos[4], 		//e4
		inputPatch[2].p,		//p2
		basisU, 
		basisV);

	// TODO: implement cuadratic normals!!
	
	// bi-linear interpolate normals
    output.n  = (1 - uv.y) * (inputPatch[0].n * (1 - uv.x) + inputPatch[1].n * uv.x) +
                     uv.y  * (inputPatch[3].n * (1 - uv.x) + inputPatch[2].n * uv.x);
	output.n  = normalize( output.n );
    
	// bi-linear interpolate tangents
    output.t1 = (1 - uv.y) * (inputPatch[0].t1 * (1 - uv.x) + inputPatch[1].t1 * uv.x) +
                      uv.y * (inputPatch[3].t1 * (1 - uv.x) + inputPatch[2].t1 * uv.x);
	output.t1 = normalize( output.t1 );

	// bi-linear interpolate tangents
	output.t2 = (1 - uv.y) * (inputPatch[0].t2 * (1 - uv.x) + inputPatch[1].t2 * uv.x) +
                      uv.y * (inputPatch[3].t2 * (1 - uv.x) + inputPatch[2].t2 * uv.x);
	output.t2 = normalize( output.t2 );


	
	// bi-linear interpolate tex-coords
    output.t  = (1 - uv.y) * (inputPatch[0].t * (1 - uv.x) + inputPatch[1].t * uv.x) +
                      uv.y * (inputPatch[3].t * (1 - uv.x) + inputPatch[2].t * uv.x);
   
	// --- bi-linear interpolation of per-vertex colors	
	output.c = (1 - uv.y) * (inputPatch[0].c * (1 - uv.x) + inputPatch[1].c * uv.x) + 
                     uv.y * (inputPatch[3].c * (1 - uv.x) + inputPatch[2].c * uv.x);

	// --- Classical vertex-shader transforms: 
	// --- output postion in the clip-space	
	output.p		= mul( float4(position, 1.0f),	mWorld );		
	output.wp		= output.p;
	output.p		= mul( output.p, mView );    
	output.p		= mul( output.p, mProjection );	

	// --- interpolated normals     
    output.n		= normalize( mul(output.n,  (float3x3)mWorld) );
    output.t1		= normalize( mul(output.t1, (float3x3)mWorld) );
	output.t2		= normalize( mul(output.t2, (float3x3)mWorld) );

    return output;
}


//--------------------------------------------------------------------------------------
// Simple Color
//--------------------------------------------------------------------------------------
float4 PSColor( PSInput input ) : SV_Target
{
	return float4(1, 0, 0, 1);
}


//--------------------------------------------------------------------------------------
// Techniques:
//  "Solid",
//  "Wires",
//  "Positions",
//  "Normals",
//  "TexCoords",
//  "Tangents",
//  "Colors",
//--------------------------------------------------------------------------------------
technique11 RenderPNTriangs
{
    pass Solid
    {
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSNoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );			
		SetVertexShader		( CompileShader( vs_5_0, VShaderTessellated() ) );                		
		SetHullShader		( CompileShader( hs_5_0, HShaderTriMain() ) );
		SetDomainShader		( CompileShader( ds_5_0, DShaderTri() ) );
		SetGeometryShader	( NULL );
		SetPixelShader		( CompileShader( ps_5_0, PShaderPhong() ) ); 
    } 
	pass Wires
    {
	//	SetRasterizerState	( RSWire );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSNoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );		
        SetVertexShader		( CompileShader( vs_5_0, VShaderTessellated() ) );                
		SetHullShader		( CompileShader( hs_5_0, HShaderTriMain() ) );
        SetDomainShader		( CompileShader( ds_5_0, DShaderTri() ) );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_5_0, PShaderColor() ) ); 
    }
	pass Positions
    {
	//	SetRasterizerState	( RSSolid );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSNoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );		
        SetVertexShader		( CompileShader( vs_5_0, VShaderTessellated() ) );                
		SetHullShader		( CompileShader( hs_5_0, HShaderTriMain() ) );
        SetDomainShader		( CompileShader( ds_5_0, DShaderTri() ) );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_5_0, PShaderPositions() ) ); 
    }
	pass Normals
    {
	//	SetRasterizerState	( RSSolid );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSNoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );		
        SetVertexShader		( CompileShader( vs_5_0, VShaderTessellated() ) );                
		SetHullShader		( CompileShader( hs_5_0, HShaderTriMain() ) );
        SetDomainShader		( CompileShader( ds_5_0, DShaderTri() ) );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_5_0, PShaderNormals() ) ); 
    }		
	pass TexCoords
    {
	//	SetRasterizerState	( RSSolid );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSNoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );		
        SetVertexShader		( CompileShader( vs_5_0, VShaderTessellated() ) );                
		SetHullShader		( CompileShader( hs_5_0, HShaderTriMain() ) );
        SetDomainShader		( CompileShader( ds_5_0, DShaderTri() ) );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_5_0, PShaderTexCoords() ) ); 
    }
	pass Tangents
    {
	//	SetRasterizerState	( RSSolid );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSNoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );		
        SetVertexShader		( CompileShader( vs_5_0, VShaderTessellated() ) );                
		SetHullShader		( CompileShader( hs_5_0, HShaderTriMain() ) );
        SetDomainShader		( CompileShader( ds_5_0, DShaderTri() ) );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_5_0, PShaderTangents() ) ); 
    }		
	pass Colors
    {
	//	SetRasterizerState	( RSSolid );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSNoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );		
        SetVertexShader		( CompileShader( vs_5_0, VShaderTessellated() ) );                
		SetHullShader		( CompileShader( hs_5_0, HShaderTriMain() ) );
        SetDomainShader		( CompileShader( ds_5_0, DShaderTri() ) );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_5_0, PShaderColor() ) ); 
    }
}
//--------------------------------------------------------------------------------------
technique11 RenderPNQuads
{
    pass Solid
    {
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSNoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );		
        SetVertexShader		( CompileShader( vs_5_0, VShaderTessellated() ) );                
		SetHullShader		( CompileShader( hs_5_0, HShaderQuadMain() ) );
        SetDomainShader		( CompileShader( ds_5_0, DShaderQuad() ) );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_5_0, PShaderPhong() ) ); 
    } 
	pass Wires
    {
		//SetRasterizerState	( RSWire );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSNoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );		
        SetVertexShader		( CompileShader( vs_5_0, VShaderTessellated() ) );                
		SetHullShader		( CompileShader( hs_5_0, HShaderQuadMain() ) );
        SetDomainShader		( CompileShader( ds_5_0, DShaderQuad() ) );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_5_0, PShaderColor() ) ); 
    }
		
	pass Positions
    {
	//	SetRasterizerState	( RSSolid );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSNoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );		
        SetVertexShader		( CompileShader( vs_5_0, VShaderTessellated() ) );                
		SetHullShader		( CompileShader( hs_5_0, HShaderQuadMain() ) );
        SetDomainShader		( CompileShader( ds_5_0, DShaderQuad() ) );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_5_0, PShaderPositions() ) ); 
    }
	pass Normals
    {
	//	SetRasterizerState	( RSSolid );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSNoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );		
        SetVertexShader		( CompileShader( vs_5_0, VShaderTessellated() ) );                
		SetHullShader		( CompileShader( hs_5_0, HShaderQuadMain() ) );
        SetDomainShader		( CompileShader( ds_5_0, DShaderQuad() ) );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_5_0, PShaderNormals() ) ); 
    }		
	pass TexCoords
    {
	//	SetRasterizerState	( RSSolid );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSNoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );		
        SetVertexShader		( CompileShader( vs_5_0, VShaderTessellated() ) );                
		SetHullShader		( CompileShader( hs_5_0, HShaderQuadMain() ) );
        SetDomainShader		( CompileShader( ds_5_0, DShaderQuad() ) );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_5_0, PShaderTexCoords() ) ); 
    }
	pass Tangents
    {
	//	SetRasterizerState	( RSSolid );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSNoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );		
        SetVertexShader		( CompileShader( vs_5_0, VShaderTessellated() ) );                
		SetHullShader		( CompileShader( hs_5_0, HShaderQuadMain() ) );
        SetDomainShader		( CompileShader( ds_5_0, DShaderQuad() ) );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_5_0, PShaderTangents() ) ); 
    }		
	pass Colors
    {
	//	SetRasterizerState	( RSSolid );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSNoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );		
        SetVertexShader		( CompileShader( vs_5_0, VShaderTessellated() ) );                
		SetHullShader		( CompileShader( hs_5_0, HShaderQuadMain() ) );
        SetDomainShader		( CompileShader( ds_5_0, DShaderQuad() ) );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_5_0, PShaderColor() ) ); 
    }
}
