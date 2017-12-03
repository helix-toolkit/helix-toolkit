#ifndef TESSELLATION_EFFECTS
#define TESSELLATION_EFFECTS
#include "Tessellation.fx"
#include "Effects.fx"

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
		SetBlendState		( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );			
		SetVertexShader		( CompileShader( vs_5_0, VShaderTessellated() ) );                		
		SetHullShader		( CompileShader( hs_5_0, HShaderTriMain() ) );
		SetDomainShader		( CompileShader( ds_5_0, DShaderTri() ) );
		SetGeometryShader	( NULL );
        SetPixelShader		(CompileShader(ps_5_0, PSShaderBlinnPhong()));
    } 
    pass Positions
    {
	//	SetRasterizerState	( RSSolid );
        SetDepthStencilState(DSSDepthLess, 0);
        SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
        SetVertexShader(CompileShader(vs_5_0, VShaderTessellated()));
        SetHullShader(CompileShader(hs_5_0, HShaderTriMain()));
        SetDomainShader(CompileShader(ds_5_0, DShaderTri()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, PShaderPositions()));
    }
    pass Normals
    {
	//	SetRasterizerState	( RSSolid );
        SetDepthStencilState(DSSDepthLess, 0);
        SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
        SetVertexShader(CompileShader(vs_5_0, VShaderTessellated()));
        SetHullShader(CompileShader(hs_5_0, HShaderTriMain()));
        SetDomainShader(CompileShader(ds_5_0, DShaderTri()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, PShaderNormals()));
    }
    pass TexCoords
    {
	//	SetRasterizerState	( RSSolid );
        SetDepthStencilState(DSSDepthLess, 0);
        SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
        SetVertexShader(CompileShader(vs_5_0, VShaderTessellated()));
        SetHullShader(CompileShader(hs_5_0, HShaderTriMain()));
        SetDomainShader(CompileShader(ds_5_0, DShaderTri()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, PShaderTexCoords()));
    }
    pass Tangents
    {
	//	SetRasterizerState	( RSSolid );
        SetDepthStencilState(DSSDepthLess, 0);
        SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
        SetVertexShader(CompileShader(vs_5_0, VShaderTessellated()));
        SetHullShader(CompileShader(hs_5_0, HShaderTriMain()));
        SetDomainShader(CompileShader(ds_5_0, DShaderTri()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, PShaderTangents()));
    }
    pass Colors
    {
	//	SetRasterizerState	( RSSolid );
        SetDepthStencilState(DSSDepthLess, 0);
        SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
        SetVertexShader(CompileShader(vs_5_0, VShaderTessellated()));
        SetHullShader(CompileShader(hs_5_0, HShaderTriMain()));
        SetDomainShader(CompileShader(ds_5_0, DShaderTri()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, PShaderColor()));
    }
}
//--------------------------------------------------------------------------------------
technique11 RenderPNQuads
{
    pass Solid
    {
		//SetRasterizerState	( RSSolid );
        SetDepthStencilState(DSSDepthLess, 0);
        SetBlendState(BSNoBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
        SetVertexShader(CompileShader(vs_5_0, VShaderTessellated()));
        SetHullShader(CompileShader(hs_5_0, HShaderQuadMain()));
        SetDomainShader(CompileShader(ds_5_0, DShaderQuad()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, PSShaderBlinnPhong()));
    }
    pass Wires
    {
		//SetRasterizerState	( RSWire );
        SetDepthStencilState(DSSDepthLess, 0);
        SetBlendState(BSNoBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
        SetVertexShader(CompileShader(vs_5_0, VShaderTessellated()));
        SetHullShader(CompileShader(hs_5_0, HShaderQuadMain()));
        SetDomainShader(CompileShader(ds_5_0, DShaderQuad()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, PShaderColor()));
    }
		
    pass Positions
    {
	//	SetRasterizerState	( RSSolid );
        SetDepthStencilState(DSSDepthLess, 0);
        SetBlendState(BSNoBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
        SetVertexShader(CompileShader(vs_5_0, VShaderTessellated()));
        SetHullShader(CompileShader(hs_5_0, HShaderQuadMain()));
        SetDomainShader(CompileShader(ds_5_0, DShaderQuad()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, PShaderPositions()));
    }
    pass Normals
    {
	//	SetRasterizerState	( RSSolid );
        SetDepthStencilState(DSSDepthLess, 0);
        SetBlendState(BSNoBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
        SetVertexShader(CompileShader(vs_5_0, VShaderTessellated()));
        SetHullShader(CompileShader(hs_5_0, HShaderQuadMain()));
        SetDomainShader(CompileShader(ds_5_0, DShaderQuad()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, PShaderNormals()));
    }
    pass TexCoords
    {
	//	SetRasterizerState	( RSSolid );
        SetDepthStencilState(DSSDepthLess, 0);
        SetBlendState(BSNoBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
        SetVertexShader(CompileShader(vs_5_0, VShaderTessellated()));
        SetHullShader(CompileShader(hs_5_0, HShaderQuadMain()));
        SetDomainShader(CompileShader(ds_5_0, DShaderQuad()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, PShaderTexCoords()));
    }
    pass Tangents
    {
	//	SetRasterizerState	( RSSolid );
        SetDepthStencilState(DSSDepthLess, 0);
        SetBlendState(BSNoBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
        SetVertexShader(CompileShader(vs_5_0, VShaderTessellated()));
        SetHullShader(CompileShader(hs_5_0, HShaderQuadMain()));
        SetDomainShader(CompileShader(ds_5_0, DShaderQuad()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, PShaderTangents()));
    }
    pass Colors
    {
	//	SetRasterizerState	( RSSolid );
        SetDepthStencilState(DSSDepthLess, 0);
        SetBlendState(BSNoBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
        SetVertexShader(CompileShader(vs_5_0, VShaderTessellated()));
        SetHullShader(CompileShader(hs_5_0, HShaderQuadMain()));
        SetDomainShader(CompileShader(ds_5_0, DShaderQuad()));
        SetGeometryShader(NULL);
        SetPixelShader(CompileShader(ps_5_0, PShaderColor()));
    }
}


#endif