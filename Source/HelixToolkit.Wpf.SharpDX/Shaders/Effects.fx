#ifndef EFFECTS_FX
#define EFFECTS_FX
#include "Default.fx"

//--------------------------------------------------------------------------------------
// Techniques
//--------------------------------------------------------------------------------------
technique11 RenderPhong
{
	pass P0
	{
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderPhong()));
	}
	pass P1
	{
		SetRasterizerState(RSWire);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderPhong()));
	}
    pass P2 //XRay
    {
    	SetDepthStencilState(DSSDepthXRay, 0);
        //SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSXRayBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);

		SetVertexShader(CompileShader(vs_4_0, VShaderXRay()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PSShaderXRay()));
    }
}

technique11 RenderBlinn
{
	pass P0
	{
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);

		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PSShaderBlinnPhong()));
	}
	pass P1
	{
		SetRasterizerState(RSWire);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);

		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PSShaderBlinnPhong()));
	}

    pass P2 //XRay
    {
    	//SetDepthStencilState(DSSDepthXRay, 0);
		//SetBlendState(BSXRayBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);

		SetVertexShader(CompileShader(vs_4_0, VShaderXRay()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PSShaderXRay()));
    }
}

technique11 RenderBoneSkinBlinn
{
	pass P0
	{
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);

		SetVertexShader(CompileShader(vs_4_0, VShaderBoneSkin()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PSShaderBlinnPhong()));
	}
	pass P1
	{
		SetRasterizerState(RSWire);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);

		SetVertexShader(CompileShader(vs_4_0, VShaderBoneSkin()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PSShaderBlinnPhong()));
	}
}

technique11 RenderInstancingBlinn
{
	pass P0
	{
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);

		SetVertexShader(CompileShader(vs_4_0, VInstancingShader()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PSInstancingShaderBlinnPhong()));
	}
	pass P1
	{
		SetRasterizerState(RSWire);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);

		SetVertexShader(CompileShader(vs_4_0, VInstancingShader()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PSInstancingShaderBlinnPhong()));
	}
}

technique11 RenderDiffuse
{
	pass P0
	{
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderDiffuseMap()));
	}
	pass P1
	{
		SetRasterizerState(RSWire);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderDiffuseMap()));
	}
}

technique11 RenderColors
{
	pass P0
	{
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderColor()));
	}
	pass P1
	{
		SetRasterizerState(RSWire);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderColor()));
	}
}

technique11 RenderPositions
{
	pass P0
	{
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderPositions()));
	}
	pass P1
	{
		SetRasterizerState(RSWire);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderPositions()));
	}
}

technique11 RenderNormals
{
	pass P0
	{
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderNormals()));
	}
	pass P1
	{
		SetRasterizerState(RSWire);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderNormals()));
	}
}

technique11 RenderPerturbedNormals
{
	pass P0
	{
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderPerturbedNormals()));
	}
	pass P1
	{
		SetRasterizerState(RSWire);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderPerturbedNormals()));
	}
}

technique11 RenderTangents
{
	pass P0
	{
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderTangents()));
	}
	pass P1
	{
		SetRasterizerState(RSWire);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderTangents()));
	}
}

technique11 RenderTexCoords
{
	pass P0
	{
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderTexCoords()));
	}
	pass P1
	{
		SetRasterizerState(RSWire);
		SetDepthStencilState(DSSDepthLess, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderTexCoords()));
	}
}

technique11 RenderWires
{
	pass P0
	{
		SetRasterizerState(RSWire);
		SetDepthStencilState(DSSDepthLess, 0);
		//SetBlendState( BSNoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );

		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderPhong()));
	}
	pass P1
	{
		SetRasterizerState(RSWire);
		SetDepthStencilState(DSSDepthLess, 0);
		//SetBlendState( BSNoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );

		SetVertexShader(CompileShader(vs_4_0, VShaderDefault()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderPhong()));
	}
}

technique11 RenderCubeMap
{
	pass P0
	{
		SetRasterizerState(RSSolidCubeMap);
		SetDepthStencilState(DSSDepthLessEqual, 0);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		//SetBlendState( BSNoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0x00000000 );

		SetVertexShader(CompileShader(vs_4_0, VShaderCubeMap()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderCubeMap()));
	}
}



//--------------------------------------------------------------------------------------
// Line Techniques
//-------------------------------------------------------------------------------------
technique11 RenderLines
{
	pass P0
	{
		//SetDepthStencilState( DSSDepthLess, 0 );
		//SetDepthStencilState( DSSDepthLessEqual, 0 );
		//SetRasterizerState	( RSLines );
		//SetRasterizerState( RSFillBiasBack );
		SetBlendState		( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
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


//--------------------------------------------------------------------------------------
// Billboard Techniques
//-------------------------------------------------------------------------------------

technique11 RenderBillboard
{
	pass P0
	{
		//SetDepthStencilState( DSSDepthLess, 0 );
		SetDepthStencilState(DSSDepthLessEqual, 0);
		SetRasterizerState(RSSolid);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderBillboardText()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderBillboardText()));
	}
	pass P1
	{
		//SetDepthStencilState( DSSDepthLess, 0 );
		SetDepthStencilState(DSSDepthLessEqual, 0);
		SetRasterizerState(RSSolid);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderBillboardText()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderBillboardBackground()));
	}
	pass P2
	{
		//SetDepthStencilState( DSSDepthLess, 0 );
		SetDepthStencilState(DSSDepthLessEqual, 0);
		SetRasterizerState(RSSolid);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderBillboardText()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderBillboardImage()));
	}
}

technique11 RenderBillboardInstancing
{
	pass P0
	{
		//SetDepthStencilState( DSSDepthLess, 0 );
		SetDepthStencilState(DSSDepthLessEqual, 0);
		SetRasterizerState(RSSolid);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderBillboardInstancing()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderBillboardText()));
	}
	pass P1
	{
		//SetDepthStencilState( DSSDepthLess, 0 );
		SetDepthStencilState(DSSDepthLessEqual, 0);
		SetRasterizerState(RSSolid);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderBillboardInstancing()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderBillboardBackground()));
	}
	pass P2
	{
		//SetDepthStencilState( DSSDepthLess, 0 );
		SetDepthStencilState(DSSDepthLessEqual, 0);
		SetRasterizerState(RSSolid);
		SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		SetVertexShader(CompileShader(vs_4_0, VShaderBillboardInstancing()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(NULL);
		SetPixelShader(CompileShader(ps_4_0, PShaderBillboardImage()));
	}
}
//--------------------------------------------------------------------------------------
// Point Techniques
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
		SetVertexShader(CompileShader(vs_4_0, VShaderPoints()));
		SetHullShader(NULL);
		SetDomainShader(NULL);
		SetGeometryShader(CompileShader(gs_4_0, GShaderPoints()));
		SetPixelShader(CompileShader(ps_4_0, PShaderPoints()));
	}
}

technique11 ParticleStorm
{
    pass P0
    {
        SetVertexShader(NULL);
        SetHullShader(NULL);
        SetDomainShader(NULL);
        SetPixelShader(NULL);
        SetComputeShader(CompileShader(cs_5_0, ParticleInsertCSMAIN()));
    }
    pass P1
    {
        SetVertexShader(NULL);
        SetHullShader(NULL);
        SetDomainShader(NULL);
        SetPixelShader(NULL);
        SetComputeShader(CompileShader(cs_5_0, ParticleUpdateCSMAIN()));
    }
    pass P2
    {
		SetDepthStencilState(DSSDepthParticle, 0);
        SetBlendState(BSParticleBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
		//SetDepthStencilState(DSSDepthLessEqual, 0);
		SetRasterizerState(RSSolid);
		//SetBlendState(BSBlending, float4(0.0f, 0.0f, 0.0f, 0.0f), 0xFFFFFFFF);
        SetVertexShader(CompileShader(vs_4_0, ParticleVSMAIN()));
        SetHullShader(NULL);
        SetDomainShader(NULL);       
		//SetGeometryShader(NULL);
        SetGeometryShader(CompileShader(gs_4_0, ParticleGSMAIN()));
        SetPixelShader(CompileShader(ps_4_0, ParticlePSMAIN()));
    }
}
#endif