#ifndef COMMON_FX
#define COMMON_FX

//--------------------------------------------------------------------------------------
// File: Header for HelixToolkitDX
// Author: Przemyslaw Musialski
// Date: 11/11/12
// References & Sources: 
//--------------------------------------------------------------------------------------


//--------------------------------------------------------------------------------------
//  STATES DEFININITIONS 
//--------------------------------------------------------------------------------------
SamplerState LinearSampler
{
    Filter						= MIN_MAG_MIP_LINEAR;
    AddressU					= Wrap;
    AddressV					= Wrap;
    MaxAnisotropy				= 16;
};
SamplerState NormalSampler
{
    Filter						= MIN_MAG_MIP_LINEAR;
    AddressU					= Wrap;
    AddressV					= Wrap;
};
SamplerState PointSampler
{
    Filter						= MIN_MAG_MIP_POINT;
    AddressU					= Wrap;
    AddressV					= Wrap;
};
SamplerComparisonState CmpSampler
{
   // sampler state
   Filter						= COMPARISON_MIN_MAG_MIP_LINEAR;
   AddressU						= MIRROR;
   AddressV						= MIRROR;
   // sampler comparison state
   ComparisonFunc				= LESS_EQUAL;
};


//--------------------------------------------------------------------------------------
RasterizerState RSWire
{
	FillMode					= 2;
	CullMode					= BACK;
	DepthBias					= false;
	FrontCounterClockwise		= true;
	MultisampleEnable			= true;
	AntialiasedLineEnable		= true;	
};
RasterizerState RSWireNone
{
	FillMode					= 2;
	CullMode					= NONE;
	DepthBias					= false;
	FrontCounterClockwise		= true;
	MultisampleEnable			= true;
	AntialiasedLineEnable		= true;	
};
RasterizerState RSSolid
{
	FillMode					= 3;
	CullMode					= BACK;	
	DepthBias					= -5;
    DepthBiasClamp				= -10;
    SlopeScaledDepthBias		= +0;
	FrontCounterClockwise		= true;
	MultisampleEnable			= true;
	AntialiasedLineEnable		= true;	
};
RasterizerState RSSolidNone
{
	FillMode					= 3;
	CullMode					= NONE;	
	DepthBias					= -5;
    DepthBiasClamp				= -10;
    SlopeScaledDepthBias		= +0;
	FrontCounterClockwise		= true;
	MultisampleEnable			= true;
	AntialiasedLineEnable		= true;	
};
/*RasterizerState RSFill
{
    FillMode					= SOLID;
    CullMode					= None;
    DepthBias					= false;
	AntialiasedLineEnable		= true;
    MultisampleEnable			= true;
	FrontCounterClockwise		= true;
}*/;
RasterizerState RSLines
{
    FillMode					= SOLID;
    CullMode					= None;    
	DepthBias					= -10;
    //DepthBiasClamp				= -10;
    SlopeScaledDepthBias		= -1;

	AntialiasedLineEnable		= true;
    MultisampleEnable			= true;
	FrontCounterClockwise		= true;
};
RasterizerState RSSolidCubeMap
{
	FillMode					= 3;
	CullMode					= BACK;
	DepthBias					= false;	
	FrontCounterClockwise		= false;
};
//--------------------------------------------------------------------------------------
DepthStencilState DSSLessEqual
{
	// Make sure the depth function is LESS_EQUAL and not just LESS.
	// Otherwise, the normalized depth values at z = 1 (NDC) will
	// fail the depth test if the depth buffer was cleared to 1.
	DepthFunc					= LESS_EQUAL;
};
DepthStencilState DSSDepthLessEqual
{
	DepthEnable					= true;	
	DepthWriteMask				= All;
	DepthFunc					= Less_Equal;
};
DepthStencilState DSSDepthLess
{
	DepthEnable					= true;	
	DepthWriteMask				= All;
	DepthFunc					= Less;
};

DepthStencilState DSSDepthParticle
{
	DepthEnable = true;
    DepthWriteMask = Zero;
};

//--------------------------------------------------------------------------------------
BlendState BSNoBlending
{
    BlendEnable[0]				= false;
    //RenderTargetWriteMask[0]	= 0x00;
};

BlendState BSBlending
{
    BlendEnable[0]				= true;
    SrcBlend					= SRC_ALPHA;
    DestBlend					= INV_SRC_ALPHA ;
    BlendOp						= ADD;
    SrcBlendAlpha				= SRC_ALPHA;
    DestBlendAlpha				= DEST_ALPHA;
    BlendOpAlpha				= ADD;
    RenderTargetWriteMask[0]	= 0x0F;
};

BlendState BSParticleBlending
{
    BlendEnable[0] = true;
	SrcBlend = ONE;
	DestBlend = ONE;
    BlendOp = ADD;
    SrcBlendAlpha = ONE;
    DestBlendAlpha = ZERO;
    BlendOpAlpha = ADD;
    RenderTargetWriteMask[0] = 0x0F;
};

//--------------------------------------------------------------------------------------
// GLOBAL VARIABLES
//--------------------------------------------------------------------------------------
//cbuffer cbTransforms
//{
	float4x4 mWorld;
	float4x4 mView;
	float4x4 mProjection;
//}

	// camera frustum: 
	// [fov,asepct-ratio,near,far]
	float4 vFrustum;

	// viewport:
	// [w,h,0,0]
	float4 vViewport;	
		
	// camera position
	float3 vEyePos;

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
#endif