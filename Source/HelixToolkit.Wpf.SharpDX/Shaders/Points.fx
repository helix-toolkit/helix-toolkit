//#include "./Shaders/Common.fx"
//#include "./Shaders/Default.fx"

//--------------------------------------------------------------------------------------
// VERTEX AND PIXEL SHADER INPUTS
//--------------------------------------------------------------------------------------
struct VSInputPS
{
	float4 p	: POSITION;
	float4 c	: COLOR;
};

struct GSInputPS
{
	float4 p	: POSITION;
	float4 c	: COLOR;
};

struct PSInputPS
{
	float4 p	: SV_POSITION;
	noperspective 
	float3 t	: TEXCOORD;
	float4 c	: COLOR;
};

void makeQuad(out float4 points[4], in float4 posA)
{
    // Bring A and B in window space
    float2 Aw = projToWindow(posA);

	// Compute the corners of the ribbon in window space
    float2 A1w = float2(Aw.x + 2, Aw.y + 2);
    float2 A2w = float2(Aw.x - 2, Aw.y + 2);
    float2 B1w = float2(Aw.x - 2, Aw.y - 2);
    float2 B2w = float2(Aw.x + 2, Aw.y - 2);

    // bring back corners in projection frame
    points[0] = windowToProj(A1w, posA.z, posA.w);
    points[1] = windowToProj(A2w, posA.z, posA.w);
    points[2] = windowToProj(B2w, posA.z, posA.w);
    points[3] = windowToProj(B1w, posA.z, posA.w);
}

//--------------------------------------------------------------------------------------
// POINTS SHADER
//-------------------------------------------------------------------------------------
GSInputPS VShaderPoints( GSInputPS input )
{
	GSInputPS output = (GSInputPS)0;	
	output.p = input.p;

	//set position into clip space	
	output.p = mul( output.p, mWorld );		
	output.p = mul( output.p, mView );    
	output.p = mul( output.p, mProjection );	
	output.c = input.c;
	return output;
}

[maxvertexcount(4)]
void GShaderPoints(point GSInputPS input[1], inout TriangleStream<PSInputLS> outStream )
{
	PSInputPS output = (PSInputPS)0;
		
    float4 spriteCorners[4];
    makeQuad(spriteCorners, input[0].p);

    output.p = spriteCorners[0];
	output.c = input[0].c;
    output.t[0] = +1;
    output.t[1] = +1;
    output.t[2] = 1;	
    outStream.Append( output );
    
    output.p = spriteCorners[1];
	output.c = input[0].c;
    output.t[0] = +1;
    output.t[1] = -1;
	output.t[2] = 1;	
    outStream.Append( output );
 
    output.p = spriteCorners[2];
	output.c = input[0].c;
    output.t[0] = -1;
    output.t[1] = +1;
    output.t[2] = 1;	
    outStream.Append( output );
    
    output.p = spriteCorners[3];
	output.c = input[0].c;
    output.t[0] = -1;
    output.t[1] = -1;
	output.t[2] = 1;	
    outStream.Append( output );
    
    outStream.RestartStrip();
}

float4 PShaderPoints( PSInputPS input ) : SV_Target
{
	return input.c;
}

//--------------------------------------------------------------------------------------
// Techniques
//-------------------------------------------------------------------------------------
technique11 RenderPoints
{
    pass P0
    {	
        //SetDepthStencilState( DSSDepthLess, 0 );
		//SetDepthStencilState( DSSDepthLessEqual, 0 );
		//SetRasterizerState	( RSLines );
        //SetRasterizerState( RSFillBiasBack );
        //SetBlendState		( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
		SetVertexShader		( CompileShader( vs_4_0, VShaderPoints() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );        
        SetGeometryShader	( CompileShader( gs_4_0, GShaderPoints() ) );
        SetPixelShader		( CompileShader( ps_4_0, PShaderPoints() ) );
    }    
}