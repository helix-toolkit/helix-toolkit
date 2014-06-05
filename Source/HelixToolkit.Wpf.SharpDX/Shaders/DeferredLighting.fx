//--------------------------------------------------------------------------------------
// File: Deferred Lighting for HelixToolkitDX
// Author: Przemyslaw Musialski
// Created:  03/03/13
// Modified: 06/18/13
// References & Sources: 
// code based on the shader from: http://hieroglyph3.codeplex.com/
//--------------------------------------------------------------------------------------

//#include "./Shaders/Common.fx"
//#define MSAA
#define EXERCISE3


#ifdef MSAA
	#define NUMSUBSAMPLES 4
#endif

//-------------------------------------------------------------------------------------------------
// STATES
//-------------------------------------------------------------------------------------------------
RasterizerState RSFront
{
	FillMode					= 3;
	CullMode					= FRONT;
	FrontCounterClockwise		= true;
};

RasterizerState RSBack
{
	FillMode					= 3;
	CullMode					= BACK;
	FrontCounterClockwise		= true;
};

BlendState BSAdditive
{
	BlendEnable[0] = true;
    BlendOp = ADD;
    SrcBlend = ONE;
    DestBlend = ONE;
    BlendOpAlpha = ADD;
    SrcBlendAlpha = ONE;
    DestBlendAlpha = ONE;
};

DepthStencilState DSSNoDepth
{
	DepthEnable					= false;	
};


//-------------------------------------------------------------------------------------------------
// Constants
//-------------------------------------------------------------------------------------------------
cbuffer LightParams
{
	// the light direction is here the vector which looks towards the light
	float3 vLightDir;
	float3 vLightPos;
	float3 vLightAtt;		// const, linerar, quadratic, 
	float4 vLightSpot;		//(outer angle , inner angle, falloff, light-range!!!)	
	float4 vLightColor;
	//float4 vLightAmbient	= float4(0.2f, 0.2f, 0.2f, 1.0f);
	matrix mLightModel;
};

float4 vBackgroundColor		= float4(1.0f, 1.0f, 1.0f, 1.0f);

matrix mLightView;
matrix mLightProj;


//-------------------------------------------------------------------------------------------------
// Input/output structs
//-------------------------------------------------------------------------------------------------
struct VSInputDS
{
	float4 p			: POSITION;
	float4 screenPos	: SV_POSITION;
};

struct VSOutputDS
{
	float4 pCS			: SV_Position;
	float2 tc			: TEXCOORD0;	
};

//struct PSOutputDS
//{
//	float4 Color		: SV_Target0;
//};

//-------------------------------------------------------------------------------------------------
// Textures
//-------------------------------------------------------------------------------------------------

/// TODO Bonus 5.1:
/// Implement the G-Buffer as Texture2DArray
#ifdef MSAA
	Texture2DMS<float4,NUMSUBSAMPLES> NormalTexture				: register( t0 );
	Texture2DMS<float4,NUMSUBSAMPLES> DiffuseAlbedoTexture		: register( t1 );
	Texture2DMS<float4,NUMSUBSAMPLES> SpecularAlbedoTexture		: register( t2 );
	Texture2DMS<float4,NUMSUBSAMPLES> PositionTexture			: register( t3 );	
#else
	Texture2D       NormalTexture					: register( t0 );
	Texture2D		DiffuseAlbedoTexture			: register( t1 );
	Texture2D		SpecularAlbedoTexture			: register( t2 );
	Texture2D		PositionTexture					: register( t3 );
	
/// TODO Bonus 4:
/// Optimize G-Buffer and transform depth-buffer to world-space
/// Texture2D		DepthTexture					: register( t3 );
#endif

	Texture2D		RandNormalsTexture				: register( t4 );

/// TODO Bonus 5.2:
/// Implement the ScreenSpaceBufferTexture as Texture2DArray
	Texture2D		ScreenSpaceBufferTexture0		: register( t5 );
	Texture2D		ScreenSpaceBufferTexture1		: register( t6 );	
	

//-------------------------------------------------------------------------------------------------
// Vertex shader entry point
//-------------------------------------------------------------------------------------------------
VSOutputDS VSSimple( VSInputDS input )
{
	VSOutputDS output = (VSOutputDS)0;
	
	//set position into camera clip space	
	output.pCS = input.p;
	output.tc  = (input.p.xy+1)/2;

	return output;
}

//-------------------------------------------------------------------------------------------------
// Sphere vertex shader entry point
//-------------------------------------------------------------------------------------------------
VSOutputDS VSSphere( VSInputDS input )
{
	VSOutputDS output = (VSOutputDS)0;
	
	float4 pos = input.p;

	// transform sphere to proj-view space
	pos = mul(pos, mLightModel);
	pos = mul(pos, mLightView);
	pos = mul(pos, mLightProj);

	//set position into camera clip space	
	output.pCS = pos;

	return output;
}

//-------------------------------------------------------------------------------------------------
// Cone vertex shader entry point
//-------------------------------------------------------------------------------------------------
VSOutputDS VSCone( VSInputDS input )
{
	VSOutputDS output = (VSOutputDS)0;	
	float4 pos = input.p;

	// transform sphere to proj-view space
	pos = mul(pos, mLightModel);
	pos = mul(pos, mLightView);
	pos = mul(pos, mLightProj);

	//set position into camera clip space	
	output.pCS = pos;

	return output;
}

//-------------------------------------------------------------------------------------------------
// Calculates the directional lighting 
//------------------------------------------------------------------------------------------------
float3 CalcDirLighting( in float3 normal, 
						in float3 position, 
						in float3 diffuseAlbedo,
						in float4 specularAlbedo)
{
	// Light direction is explicit for directional lights
	float3 L = -normalize(vLightDir);		
	float3 V = normalize( vEyePos - position );
	float3 H = normalize( V + L );
	float3 N = normal.xyz;
	
	float3 Id = diffuseAlbedo * saturate( dot(N,L) );
	float3 Is = specularAlbedo.rgb * pow( saturate( dot(N,H) ), specularAlbedo.w);
	
	return  (Id + Is) * vLightColor.rgb;
}

//-------------------------------------------------------------------------------------------------
// Current implementation - point light
//-------------------------------------------------------------------------------------------------
float3 CalcPointLighting(	in float3 normal, 
							in float3 position, 
							in float3 diffuseAlbedo,
							in float4 specularAlbedo)
{
	// Calculate the diffuse term
	float3 L = 0;

	// Base the the light vector on the light position
	L = vLightPos - position;

	// Calculate attenuation based on distance from the light source
	float dist = length( L );
	float attenuation_factor = 1/(vLightAtt.x + dist * vLightAtt.y + dist * dist * vLightAtt.z); //clamp?

	L /= dist; //normalization (?)

	float3 V = normalize( vEyePos - position );
	float3 H = normalize( V + L );
	float3 N = normal.xyz;
		
	float3 Id = diffuseAlbedo * saturate( dot(N,L) );	
	float3 Is = specularAlbedo.rgb * pow( saturate( dot(N,H) ), specularAlbedo.w);
	
	return  (Id + Is) * vLightColor.rgb * attenuation_factor;
}

//-------------------------------------------------------------------------------------------------
// Compute the spotlight
//-------------------------------------------------------------------------------------------------
float3 CalcSpotLighting(	in float3 normal, 
							in float3 position, 
							in float3 diffuseAlbedo,
							in float4 specularAlbedo)
{	
	return float3(1.0, 1.0, 1.0);
}



//-------------------------------------------------------------------------------------------------
// Fragment Shader for ambient light 
//-------------------------------------------------------------------------------------------------
float4 PSAmbient(in VSOutputDS input, in float4 screenPos : SV_Position, in uint coverage : SV_Coverage  ) : SV_Target
{
	int3 samplePos  = int3( screenPos.xy, 0 );
			
#ifdef MSAA	
	
	// variable for color
	float3 lightingColor = 0;

	// Calculate lighting for MSAA samples
	float numSamplesApplied = 0.0f;
	for ( int i = 0; i < NUMSUBSAMPLES; ++i )
	{
		// We only want to calculate lighting for a sample if the light volume is covered.
		// We determine this using the input coverage mask.
		//if ( coverage & ( 1 << i ) )
		{
			float4 normal			= NormalTexture.Load( samplePos.xy, i );	
			float4 diffuse			= DiffuseAlbedoTexture.Load( samplePos.xy, i );
				
			lightingColor		   += ((normal.w == 0) ? diffuse.rgb : (vLightAmbient.rgb * diffuse.w));
			++numSamplesApplied;
		}
	}
	lightingColor /= numSamplesApplied;
	return float4(lightingColor, 1.0f);

#else

	float4 normal	= NormalTexture.Load( samplePos );
	float4 diffuse	= DiffuseAlbedoTexture.Load( samplePos );
		
	if(normal.w==0) 
		return float4(diffuse.rgb, 1.0f);
	
	return vLightAmbient * diffuse.w;

#endif
}

//-------------------------------------------------------------------------------------------------
// Pixel Shader for directional light
//-------------------------------------------------------------------------------------------------
float4 PSDirLight(in VSOutputDS input, in float4 screenPos : SV_Position, in uint coverage : SV_Coverage  ) : SV_Target
{
	// Get values per pixel from the G-Buffer
	// use float4 screenPos : SV_Position to get an integer screen-space position
	int3 samplePos			= int3( screenPos.xy, 0 );

#ifdef MSAA		
		
	// variable for color
	float3 lightingColor = 0;

	// Calculate lighting for MSAA samples
	float numSamplesApplied = 0.0f;
	for ( int i = 0; i < NUMSUBSAMPLES; ++i )
	{
		// We only want to calculate lighting for a sample if the light volume is covered.
		// We determine this using the input coverage mask.
		//if ( coverage & ( 1 << i ) )
		{
			float4 normal			= NormalTexture.Load( samplePos.xy, i );	
			if(normal.w==0) 
				discard;

			float4 position			= PositionTexture.Load( samplePos.xy, i );
			float4 diffuseAlbedo	= DiffuseAlbedoTexture.Load( samplePos.xy, i );
			float4 specularAlbedo	= SpecularAlbedoTexture.Load( samplePos.xy, i );

			lightingColor		   += CalcDirLighting( normal.xyz, position.xyz, diffuseAlbedo.xyz, specularAlbedo );
			++numSamplesApplied;
		}
	}
	lightingColor /= numSamplesApplied;	
	return float4( lightingColor, 1.0f );

#else

	float4 normal			= NormalTexture.Load( samplePos );	
	// if the w-component of normal is 0, there is no geometry, 
	// so return just diffuse albedo, which is the clear-color
	if(normal.w==0) 
		discard;
	float4 position			= PositionTexture.Load( samplePos );
	float4 diffuseAlbedo	= DiffuseAlbedoTexture.Load( samplePos );
	float4 specularAlbedo	= SpecularAlbedoTexture.Load( samplePos );
		
	// Calculate lighting for a single G-Buffer sample	
	return	float4( CalcDirLighting( normal.xyz, position.xyz, diffuseAlbedo.xyz, specularAlbedo ), 1.0f);

#endif
}


//-------------------------------------------------------------------------------------------------
// Fragment Shader for point light
//-------------------------------------------------------------------------------------------------
float4 PSPointLight(in VSOutputDS input, in float4 screenPos : SV_Position,  in uint coverage : SV_Coverage  ) : SV_Target
{
	int3 samplePos			= int3( screenPos.xy, 0 );

#ifdef MSAA		
		
	// variable for color
	float3 lightingColor = 0;

	// Calculate lighting for MSAA samples
	float numSamplesApplied = 0.0f;
	for ( int i = 0; i < NUMSUBSAMPLES; ++i )
	{
		// We only want to calculate lighting for a sample if the light volume is covered.
		// We determine this using the input coverage mask.
		//if ( coverage & ( 1 << i ) )
		{
			float4 normal			= NormalTexture.Load( samplePos.xy, i );	
			if(normal.w==0) 
				discard;

			float4 position			= PositionTexture.Load( samplePos.xy, i );
			float4 diffuseAlbedo	= DiffuseAlbedoTexture.Load( samplePos.xy, i );
			float4 specularAlbedo	= SpecularAlbedoTexture.Load( samplePos.xy, i );

			lightingColor		   += CalcPointLighting( normal.xyz, position.xyz, diffuseAlbedo.xyz, specularAlbedo );
			++numSamplesApplied;
		}
	}
	lightingColor /= numSamplesApplied;	
	return float4( lightingColor, 1.0f );

#else

	float4 normal			= NormalTexture.Load( samplePos );	
	// if the w-component of normal is 0, there is no geometry, 
	// so return just diffuse albedo, which is the clear-color
	if(normal.w==0) 
		discard;

	float4 position			= PositionTexture.Load( samplePos );
	float4 diffuseAlbedo	= DiffuseAlbedoTexture.Load( samplePos );
	float4 specularAlbedo	= SpecularAlbedoTexture.Load( samplePos );			

	// Calculate lighting for a single G-Buffer sample	
	return float4( CalcPointLighting( normal.xyz, position.xyz, diffuseAlbedo.xyz, specularAlbedo ), 1.0f );

#endif
}


//-------------------------------------------------------------------------------------------------
// Fragment Shader for spot light
//-------------------------------------------------------------------------------------------------
float4 PSSpotLight(in VSOutputDS input, in float4 screenPos : SV_Position,  in uint coverage : SV_Coverage ) : SV_Target
{	
	int3 samplePos			= int3( screenPos.xy, 0 );

#ifdef MSAA		
		
	// variable for color
	float3 lightingColor = 0;

	// Calculate lighting for MSAA samples
	float numSamplesApplied = 0.0f;
	for ( int i = 0; i < NUMSUBSAMPLES; ++i )
	{
		// We only want to calculate lighting for a sample if the light volume is covered.
		// We determine this using the input coverage mask.
		//if ( coverage & ( 1 << i ) )
		{
			float4 normal			= NormalTexture.Load( samplePos.xy, i );	
			if(normal.w==0) 
				discard;

			float4 position			= PositionTexture.Load( samplePos.xy, i );
			float4 diffuseAlbedo	= DiffuseAlbedoTexture.Load( samplePos.xy, i );
			float4 specularAlbedo	= SpecularAlbedoTexture.Load( samplePos.xy, i );

			lightingColor		   += CalcSpotLighting( normal.xyz, position.xyz, diffuseAlbedo.xyz, specularAlbedo );
			++numSamplesApplied;
		}
	}
	lightingColor /= numSamplesApplied;	
	return float4( lightingColor, 1.0f );

#else

	float4 normal			= NormalTexture.Load( samplePos );
	
	// if the w-component of normal is 0, there is no geometry, 
	// so return just diffuse albedo, which is the clear-color
	if(normal.w==0) 
		discard;

	float4 position			= PositionTexture.Load( samplePos );
	float4 diffuseAlbedo	= DiffuseAlbedoTexture.Load( samplePos );
	float4 specularAlbedo	= SpecularAlbedoTexture.Load( samplePos );		
	return float4( CalcSpotLighting( normal.xyz, position.xyz, diffuseAlbedo.xyz, specularAlbedo ), 1.0f );

#endif
}


#ifdef EXERCISE3

//--------------------------------------------------------------------------------------
// Screen-Space Ambient Occlusion global variables
//--------------------------------------------------------------------------------------

float g_scale		= 100.00f;
float g_intensity	= 15.00f;
float g_bias		= 0.10f;

static const float2 sampleMask[8] = 
{
	float2(+1, 0),
	float2(-1, 0),						   
	float2(0, +1),
	float2(0, -1),

	float2(+1,+1),
	float2(-1,+1),						   
	float2(+1,-1),
	float2(-1,-1),
};

//--------------------------------------------------------------------------------------
// Screen-Space Ambient Occlusion function
//--------------------------------------------------------------------------------------
float AmbientOcclusion(in float2 uv, in float3 pos, in float3 normal)
{	
	//float3 diff			= PositionTexture.Sample( PointSampler, tc+uv ).xyz - pos;	
	float3 diff			= PositionTexture.Load( int3(uv,0) ).xyz - pos;
	float3 v			= normalize(diff);
	float d				= length(diff) * g_scale;
	
	return max( 0.0f, dot(normal,v) - g_bias) * ( 1.0f / (1.0f+d) ) * g_intensity;
}
#endif

//--------------------------------------------------------------------------------------
// Pixel Shader for Screen-Space Ambient Occlusion
//--------------------------------------------------------------------------------------
float4 PSSSAO(in VSOutputDS input, in float4 screenPos : SV_Position ) : SV_Target
{
	int3 samplePos			= int3( screenPos.xy, 0 );

#ifdef EXERCISE3
	
	//float g_sample_rad		= 0.9f;	
	//float rad				= g_sample_rad / position.z;	
	
	//float2 tc				= input.tc;
	float2 uv				= float2(input.tc.x, 1.0 - input.tc.y); // align properly
	
	float ao				= 0.0f;
	float3 position			= PositionTexture.Load( samplePos ).xyz;
	float3 normal			= NormalTexture.Load( samplePos ).xyz;
	float2 rand				= normalize( RandNormalsTexture.Sample( PointSampler,  uv ) * 2.0f - 1.0f).xy;		
	
	float3 viewVec =	vEyePos-position;
	float rad = 1.0;//length(viewVec);
	//return float4(rad, rad, rad, 1.0f);

	// SSAO Calculation
#define ITERATIONS 8

	[unroll]
	for (int j = 0; j < ITERATIONS; j++)
	{		
		float2 coord1 = sampleMask[j];// reflect( sampleMask[j] , rand );
		float2 coord2 = sampleMask[j];// float2( coord1.x*0.707 - coord1.y*0.707,  coord1.x*0.707 + coord1.y*0.707 );
		//float2 coord1 = reflect( sampleMask[j] , rand );
		//float2 coord2 = float2( coord1.x*0.707 - coord1.y*0.707,  coord1.x*0.707 + coord1.y*0.707 );

		ao += AmbientOcclusion(samplePos.xy+coord1 * 1 * rad, position, normal);
		ao += AmbientOcclusion(samplePos.xy+coord2 * 4 * rad, position, normal);
		ao += AmbientOcclusion(samplePos.xy+coord1 * 8 * rad, position, normal);
		ao += AmbientOcclusion(samplePos.xy+coord2 * 16 * rad, position, normal);
	}
	 
	ao /= ((float)ITERATIONS * 4.0f);

	ao = 1.0f - ao;
	return float4(ao ,ao, ao, 1.0f);	
	
#else

	/// TODO 1:
	/// Implement basic screen space ambient occlusion, 
	/// if necessary create additional helper functions in the fx-file
	return float4(1.0, 1.0, 1.0, 1.0);
#endif
}


//--------------------------------------------------------------------------------------
// Pixel Shader Merge SSAO with lighting
//--------------------------------------------------------------------------------------
float4 PSMerge(in VSOutputDS input, in float4 screenPos : SV_Position ) : SV_Target
{
	int3 samplePos			= int3( screenPos.xy, 0 );

	float3 color1 = ScreenSpaceBufferTexture0.Load( samplePos ).xyz;
	float3 color2 = ScreenSpaceBufferTexture1.Load( samplePos ).xyz;
	return float4( color1 * color2, 1.0f);
}


//--------------------------------------------------------------------------------------
// 4x4 Blur Pass
//--------------------------------------------------------------------------------------
float4 PSBlur4x4(in VSOutputDS input ) : SV_Target
{
	float4 color = float4(0.0, 0.0, 0.0, 0.0);
	float2 uv = float2(input.tc.x, 1.0 - input.tc.y);
#ifdef EXERCISE3
	
	/// TODO 3:
	/// Implement 4x4 blur
	color += ScreenSpaceBufferTexture0.Sample( PointSampler, float2(uv.x, uv.y));
#endif
 
	return color;	
}


#ifdef EXERCISE3
//--------------------------------------------------------------------------------------
// Blur Size
//--------------------------------------------------------------------------------------
static const float blurSize = 1.0f / 512.0f;
#endif

//--------------------------------------------------------------------------------------
// Horizontal Blur Pass
// http://www.gamerendering.com/2008/10/11/gaussian-blur-filter-shader/
//--------------------------------------------------------------------------------------
float4 PSHBlur(in VSOutputDS input ) : SV_Target
{
	float4 color = float4(0.0, 0.0, 0.0, 0.0);
	float2 uv = float2(input.tc.x, 1.0 - input.tc.y);

#ifdef EXERCISE3
	

	// blur in y (vertical)
	// take nine samples, with the distance blurSize between them
	color += ScreenSpaceBufferTexture0.Sample( PointSampler, float2(uv.x - 4.0*blurSize, uv.y)) * 0.05;
	color += ScreenSpaceBufferTexture0.Sample( PointSampler, float2(uv.x - 3.0*blurSize, uv.y)) * 0.09;
	color += ScreenSpaceBufferTexture0.Sample( PointSampler, float2(uv.x - 2.0*blurSize, uv.y)) * 0.12;
	color += ScreenSpaceBufferTexture0.Sample( PointSampler, float2(uv.x - blurSize, uv.y))		* 0.15;
	color += ScreenSpaceBufferTexture0.Sample( PointSampler, float2(uv.x, uv.y))				* 0.18;
	color += ScreenSpaceBufferTexture0.Sample( PointSampler, float2(uv.x + blurSize, uv.y))		* 0.15;
	color += ScreenSpaceBufferTexture0.Sample( PointSampler, float2(uv.x + 2.0*blurSize, uv.y)) * 0.12;
	color += ScreenSpaceBufferTexture0.Sample( PointSampler, float2(uv.x + 3.0*blurSize, uv.y)) * 0.09;
	color += ScreenSpaceBufferTexture0.Sample( PointSampler, float2(uv.x + 4.0*blurSize, uv.y)) * 0.05;
#else

	/// TODO 2.1:
	/// Implement separable Gaussian blur 	
	color += ScreenSpaceBufferTexture0.Sample( PointSampler, float2(uv.x, uv.y));

#endif
   
	return color;	
}

//--------------------------------------------------------------------------------------
// Vertical Blur Pass
//--------------------------------------------------------------------------------------
float4 PSVBlur(in VSOutputDS input ) : SV_Target
{
	float4 color = float4(0.0, 0.0, 0.0, 0.0);
	float2 uv = float2(input.tc.x, 1.0 - input.tc.y);

#ifdef EXERCISE3
	
	// blur in y (vertical)
	// take nine samples, with the distance blurSize between them
	color += ScreenSpaceBufferTexture1.Sample( PointSampler, float2(uv.x, uv.y - 4.0*blurSize )) * 0.05;
	color += ScreenSpaceBufferTexture1.Sample( PointSampler, float2(uv.x, uv.y - 3.0*blurSize )) * 0.09;
	color += ScreenSpaceBufferTexture1.Sample( PointSampler, float2(uv.x, uv.y - 2.0*blurSize )) * 0.12;
	color += ScreenSpaceBufferTexture1.Sample( PointSampler, float2(uv.x, uv.y - blurSize ))	 * 0.15;
	color += ScreenSpaceBufferTexture1.Sample( PointSampler, float2(uv.x, uv.y))				 * 0.18;
	color += ScreenSpaceBufferTexture1.Sample( PointSampler, float2(uv.x, uv.y + blurSize ))	 * 0.15;
	color += ScreenSpaceBufferTexture1.Sample( PointSampler, float2(uv.x, uv.y + 2.0*blurSize )) * 0.12;
	color += ScreenSpaceBufferTexture1.Sample( PointSampler, float2(uv.x, uv.y + 3.0*blurSize))  * 0.09;
	color += ScreenSpaceBufferTexture1.Sample( PointSampler, float2(uv.x, uv.y + 4.0*blurSize))  * 0.05;
#else	

	/// TODO 2.2:
	/// Implement separable Gaussian blur
	color += ScreenSpaceBufferTexture1.Sample( PointSampler, float2(uv.x, uv.y));
#endif
 
	return color;	
}


//--------------------------------------------------------------------------------------
// Techniques
//--------------------------------------------------------------------------------------
technique11 RenderDeferredLighting
{
	pass AmbientPass
    {
		SetRasterizerState	( RSBack );
		SetDepthStencilState( DSSNoDepth, 0);
	    SetBlendState		( BSNoBlending, float4( 1.0f, 1.0f, 1.0f, 1.0f ), 0xffffffff );
        SetVertexShader		( CompileShader( vs_5_0, VSSimple() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );        
		SetPixelShader		( CompileShader( ps_5_0, PSAmbient() ) );        
    }

	pass DirectionalLightPass
    {
		SetRasterizerState	( RSBack );
		SetDepthStencilState( DSSNoDepth, 0);
	    SetBlendState		( BSAdditive, float4( 1.0f, 1.0f, 1.0f, 1.0f ), 0xffffffff );	
        SetVertexShader		( CompileShader( vs_5_0, VSSimple() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );      
		SetPixelShader		( CompileShader( ps_5_0, PSDirLight() ) );        
    }

	pass PointLightPass
    {
		SetRasterizerState	( RSFront );
		SetDepthStencilState( DSSNoDepth, 0);
	    SetBlendState		( BSAdditive, float4( 1.0f, 1.0f, 1.0f, 1.0f ), 0xffffffff ); //float4: blendFactor
        SetVertexShader		( CompileShader( vs_5_0, VSSphere() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );      
		SetPixelShader		( CompileShader( ps_5_0, PSPointLight() ) );        
    }  
		
	pass SpotLightPass
    {
		SetRasterizerState	( RSFront );
		SetDepthStencilState( DSSNoDepth, 0);
	    SetBlendState		( BSAdditive, float4( 1.0f, 1.0f, 1.0f, 1.0f ), 0xffffffff ); //float4: blendFactor
        SetVertexShader		( CompileShader( vs_5_0, VSCone() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );      
		SetPixelShader		( CompileShader( ps_5_0, PSSpotLight() ) );        
    } 
}

technique11 RenderScreenSpace
{
	pass SSAOPass
    {
		SetRasterizerState	( RSBack );
		SetDepthStencilState( DSSNoDepth, 0);
	    SetBlendState		( BSNoBlending, float4( 1.0f, 1.0f, 1.0f, 1.0f ), 0xffffffff );
        SetVertexShader		( CompileShader( vs_4_0, VSSimple() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );        
		SetPixelShader		( CompileShader( ps_4_0, PSSSAO() ) );        
    }

	pass MergePass
    {
		SetRasterizerState	( RSBack );
		SetDepthStencilState( DSSNoDepth, 0);
	    SetBlendState		( BSNoBlending, float4( 1.0f, 1.0f, 1.0f, 1.0f ), 0xffffffff );
        SetVertexShader		( CompileShader( vs_4_0, VSSimple() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );        
		SetPixelShader		( CompileShader( ps_4_0, PSMerge() ) );        
    }

	pass Blur4x4Pass
    {
		SetRasterizerState	( RSBack );
		SetDepthStencilState( DSSNoDepth, 0);
	    SetBlendState		( BSNoBlending, float4( 1.0f, 1.0f, 1.0f, 1.0f ), 0xffffffff );
        SetVertexShader		( CompileShader( vs_4_0, VSSimple() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );        
		SetPixelShader		( CompileShader( ps_4_0, PSBlur4x4() ) );        
    }

	pass BlurHPass
    {
		SetRasterizerState	( RSBack );
		SetDepthStencilState( DSSNoDepth, 0);
	    SetBlendState		( BSNoBlending, float4( 1.0f, 1.0f, 1.0f, 1.0f ), 0xffffffff );
        SetVertexShader		( CompileShader( vs_4_0, VSSimple() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );        
		SetPixelShader		( CompileShader( ps_4_0, PSHBlur() ) );        
    }

	pass BlurVPass
    {
		SetRasterizerState	( RSBack );
		SetDepthStencilState( DSSNoDepth, 0);
	    SetBlendState		( BSNoBlending, float4( 1.0f, 1.0f, 1.0f, 1.0f ), 0xffffffff );
        SetVertexShader		( CompileShader( vs_4_0, VSSimple() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );        
		SetPixelShader		( CompileShader( ps_4_0, PSVBlur() ) );        
    }
}
