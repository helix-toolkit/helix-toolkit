//--------------------------------------------------------------------------------------
// File: Line Effects for HelixToolkitDX
// Author: Przemyslaw Musialski
// Date: 11/11/12
// References & Sources: 
// http://cgg-journal.com/2008-2/06/index.html
// http://developer.download.nvidia.com/SDK/10/direct3d/Source/SolidWireframe/Doc/SolidWireframe.pdf
//-------------------------------------------------------------------------------------

//#include "./Shaders/Common.fx"
//#include "./Shaders/Default.fx"


//--------------------------------------------------------------------------------------
// Constant Buffer Variables
//--------------------------------------------------------------------------------------
//cbuffer cbPerObject
//{
float4 vLineParams = float4(0, 0, 0, 0);
//bool   bHasInstances	 = false;
//}

//--------------------------------------------------------------------------------------
// VERTEX AND PIXEL SHADER INPUTS
//--------------------------------------------------------------------------------------
struct VSInputLS
{
	float4 p	: POSITION;
	float4 c	: COLOR;
	float4 parameters  : COLOR1;

	float4 mr0	: TEXCOORD1;
	float4 mr1	: TEXCOORD2;
	float4 mr2	: TEXCOORD3;
	float4 mr3	: TEXCOORD4;
};

struct GSInputLS
{
	float4 p	: POSITION;
	float4 c	: COLOR;
};

struct PSInputLS
{
	float4 p	: SV_POSITION;
	noperspective
		float3 t	: TEXCOORD;
	float4 c	: COLOR;
};


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
// Make a a ribbon line of the specified pixel width from 2 points in the projection frame.
//--------------------------------------------------------------------------------------
void makeLine(out float4 points[4], in float4 posA, in float4 posB, in float width)
{
	// Bring A and B in window space
	float2 Aw = projToWindow(posA);
	float2 Bw = projToWindow(posB);

	// Compute tangent and binormal of line AB in window space
	// Binormal is scaled by line width 
	float2 tangent = normalize(Bw.xy - Aw.xy);
	float2 binormal = width * float2(tangent.y, -tangent.x);

	// Compute the corners of the ribbon in window space
	float2 A1w = (Aw - binormal);
	float2 A2w = (Aw + binormal);
	float2 B1w = (Bw - binormal);
	float2 B2w = (Bw + binormal);

	// bring back corners in projection frame
	points[0] = windowToProj(A1w, posA.z, posA.w);
	points[1] = windowToProj(A2w, posA.z, posA.w);
	points[2] = windowToProj(B1w, posB.z, posB.w);
	points[3] = windowToProj(B2w, posB.z, posB.w);
}


//--------------------------------------------------------------------------------------
// LINES SHADER
//-------------------------------------------------------------------------------------
GSInputLS VShaderLines(VSInputLS input)
{
	GSInputLS output = (GSInputLS)0;
	output.p = input.p;

	// compose instance matrix
	if (bHasInstances)
	{
		matrix mInstance =
		{
			input.mr0.x, input.mr1.x, input.mr2.x, input.mr3.x, // row 1
			input.mr0.y, input.mr1.y, input.mr2.y, input.mr3.y, // row 2
			input.mr0.z, input.mr1.z, input.mr2.z, input.mr3.z, // row 3
			input.mr0.w, input.mr1.w, input.mr2.w, input.mr3.w, // row 4
		};
		output.p = mul(mInstance, output.p);
	}

	bool isSelected = input.parameters.x;
	float4 finalColor;

	if (isSelected) {
		finalColor = lerp(vSelectionColor, input.c, 0.3);
	}
	else {
		finalColor = input.c;
	}

	//set position into clip space	
	output.p = mul(output.p, mWorld);
	output.p = mul(output.p, mView);
	output.p = mul(output.p, mProjection);
	output.c = finalColor;

	return output;
}

// do nothing G-Shader
[maxvertexcount(6)]
void GSPassThru(line GSInputLS input[2], inout LineStream<PSInputLS> outStream)
{
	PSInputLS output = (PSInputLS)0;

	for (uint i = 0; i<2; i++)
	{
		output.p = input[i].p;
		output.c = input[i].c;

		outStream.Append(output);
	}
	outStream.RestartStrip();
}

[maxvertexcount(4)]
void GShaderLines(line GSInputLS input[2], inout TriangleStream<PSInputLS> outStream)
{
	PSInputLS output = (PSInputLS)0;

	float4 lineCorners[4];
	makeLine(lineCorners, input[0].p, input[1].p, vLineParams.x);

	output.p = lineCorners[0];
	output.c = input[0].c;
	output.t[0] = +1;
	output.t[1] = +1;
	output.t[2] = 1;
	outStream.Append(output);

	output.p = lineCorners[1];
	output.c = input[0].c;
	output.t[0] = +1;
	output.t[1] = -1;
	output.t[2] = 1;
	outStream.Append(output);

	output.p = lineCorners[2];
	output.c = input[0].c;
	output.t[0] = -1;
	output.t[1] = +1;
	output.t[2] = 1;
	outStream.Append(output);

	output.p = lineCorners[3];
	output.c = input[0].c;
	output.t[0] = -1;
	output.t[1] = -1;
	output.t[2] = 1;
	outStream.Append(output);

	outStream.RestartStrip();
}

float4 PShaderLines(PSInputLS input) : SV_Target
{
	//float arrowScale = 1 / (0.25 + (input.t[2]>0.9)*(-input.t[2]+1)*10);

	//   // Compute distance of the fragment to the edges    
	//   float alpha = min(abs(input.t[0]), abs(input.t[1]));
	//   float dist = arrowScale * alpha;
	//   alpha = exp2(-4*dist*dist*dist*dist);
	//   float4 color = input.c;
	//   color.a *= alpha;
	//   return color;	
	return input.c;
}

float4 PShaderLinesFade(PSInputLS input) : SV_Target
{

	// Compute distance of the fragment to the edges    
	//float dist = min(abs(input.t[0]), abs(input.t[1]));	
	float dist = abs(input.t.y);

// Cull fragments too far from the edge.
//if (dist > 0.5*vLineParams.x+1) discard;

// Map the computed distance to the [0,2] range on the border of the line.
//dist = clamp((dist - (0.5*vLineParams.x - 1)), 0, 2);

// Alpha is computed from the function exp2(-2(x)^2).
float sigma = 2.0f / (vLineParams.y + 1e-6);
dist *= dist;
float alpha = exp2(-2 * dist / sigma);

//if(alpha<0.1) discard;

// Standard wire color
float4 color = input.c;

//color = texDiffuseMap.Sample(SSLinearSamplerWrap, input.t.xy);	
color.a = alpha;

return color;
}

float4 PSDrawTool(PSInputLS input) : SV_Target
{
	float arrowScale = 1 / (0.25 + (input.t[2]>0.9)*(-input.t[2] + 1) * 10);

// Compute distance of the fragment to the edges    
float alpha = min(abs(input.t[0]), abs(input.t[1]));
float dist = arrowScale * alpha;
alpha = exp2(-4 * dist*dist*dist*dist);

float4 color = input.c;
color.a *= alpha;
return color;
}



//--------------------------------------------------------------------------------------
// Techniques
//-------------------------------------------------------------------------------------
technique11 RenderLines
{
	pass P0
	{
		//SetDepthStencilState( DSSDepthLess, 0 );
		//SetDepthStencilState( DSSDepthLessEqual, 0 );
		//SetRasterizerState	( RSLines );
		//SetRasterizerState( RSFillBiasBack );
		//SetBlendState		( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
		SetVertexShader(CompileShader(vs_4_0, VShaderLines()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(CompileShader(gs_4_0, GShaderLines()));
		SetPixelShader(CompileShader(ps_4_0, PShaderLinesFade()));
	}
}

technique11 RenderLinesHard
{
	pass P0
	{
		//SetDepthStencilState( DSSDepthLess, 0 );
		SetDepthStencilState(DSSDepthLessEqual, 0);
		SetRasterizerState(RSSolid);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderLines()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(CompileShader(gs_4_0, GShaderLines()));
		SetPixelShader(CompileShader(ps_4_0, PShaderLines()));
	}
}