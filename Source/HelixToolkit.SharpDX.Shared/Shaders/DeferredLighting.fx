#ifndef DEFERREDLIGHTING
#define DEFERREDLIGHTING
//--------------------------------------------------------------------------------------
// File: Deferred Lighting for HelixToolkitDX
// Author: Przemyslaw Musialski
// Created:  03/03/13
// Modified: 06/18/13
// References & Sources: 
// code based on the shader from: http://hieroglyph3.codeplex.com/
//--------------------------------------------------------------------------------------

 
//-------------------------------------------------------------------------------------------------
// STATES
//-------------------------------------------------------------------------------------------------
#include"Common.fx"

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
	DepthEnable					= false	;
};

float3 randomVectors[16] = 
{
	float3(-0.59682494,-0.7417948,0.3058437),
	float3(0.22237878,0.10807908,0.9689512),
	float3(-0.69301194,-0.12047215,-0.7107889),
	float3(-0.24678397,-0.9549722,-0.16469908),
	float3(0.77601177,0.44597107,0.44599935),
	float3(0.92894953,-0.1701165,0.3288058),
	float3(-0.78814965,-0.52545476,-0.32049552),
	float3(-0.7113109,-0.34438255,0.6127295),
	float3(0.5816374,-0.5162177,0.6286632),
	float3(-0.97526366,0.17013744,-0.14111745),
	float3(-0.91364634,-0.19148356,0.35858673),
	float3(-0.46875733,-0.37794605,0.798388),
	float3(-0.34760386,0.92936206,-0.124328844),
	float3(0.90002877,-0.06448909,-0.43103287),
	float3(0.21451922,0.9263589,-0.30958164),
	float3(-0.57708883,-0.8097383,-0.10626555)
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
    float4 vLightAmbient;
	// redefinition float4 vLightAmbient	= float4(0.2f, 0.2f, 0.2f, 1.0f);
	matrix mLightModel;
};

float4 vBackgroundColor		= float4(1.0f, 1.0f, 1.0f, 1.0f);
matrix mLightView;
matrix mLightProj;

#ifdef DEFERRED_MSAA
	int		nMsaaSamples	= 1;	
#endif

#ifdef SSAO
	// need for SSAO
	float4 vInvViewportSize;
	float4x4 mInvProjection;
	float4x4 mInvView;
	float4x4 mViewProjection;
#endif


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

struct PSOutputDS
{
	float4 Color		: SV_Target0;
};

//-------------------------------------------------------------------------------------------------
// Textures
//-------------------------------------------------------------------------------------------------



#ifdef DEFERRED_MSAA
	Texture2DMS<float4> NormalTexture				: register( t0 );
	Texture2DMS<float4> DiffuseAlbedoTexture		: register( t1 );
	Texture2DMS<float4> SpecularAlbedoTexture		: register( t2 );
	Texture2DMS<float4> PositionTexture				: register( t3 );
	Texture2DMS<float4> DepthTexture				: register( t7 );	
#else
	Texture2D       NormalTexture					: register( t0 );
	Texture2D		DiffuseAlbedoTexture			: register( t1 ); // .w component contains ambient factor
	Texture2D		SpecularAlbedoTexture			: register( t2 ); // .w component contains shininess

	Texture2D		PositionTexture					: register( t3 );
	Texture2D		DepthTexture					: register( t7 );

#endif


	// Use this buffer as a random normal 4x4 texture
	Texture2D		RandNormalsTexture				: register( t4 );

#ifdef SSAO
	Texture2D		ScreenSpaceBufferTexture0		: register( t5 );
	Texture2D		ScreenSpaceBufferTexture1		: register( t6 );
	Texture2D		ScreenSpaceBufferTexture2		: register( t8 );
#endif
	

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

	// transform cone to view space
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
float3 CalcDirLighting(		in float3 normal, 
							in float3 position, 
							in float3 diffuseAlbedo,
							in float4 specularAlbedo)
{
	// Light direction is explicit for directional lights
	float3 L	= -normalize(vLightDir);		
	float3 V	= normalize( vEyePos - position );	// directrion to viewer
	float3 H	= normalize( V + L );				// halfway vector	
		
	float3 Id	= diffuseAlbedo * saturate( dot( normal, L) );
	float3 Is	= specularAlbedo.rgb * pow( saturate( dot( normal, H ) ), specularAlbedo.w);
	
	return  (Id + Is) * vLightColor.rgb;
}

//-------------------------------------------------------------------------------------------------
// Compute point lighting
//-------------------------------------------------------------------------------------------------
float3 CalcPointLighting(	in float3 normal, 
							in float3 position, 
							in float3 diffuseAlbedo,
							in float4 specularAlbedo)
{
	float3 L	= vLightPos - position;				// light dir
	float  dl	= length(L);						// distance to light source	
	L			= L / dl;

	float3 V	= normalize( vEyePos - position );	// directrion to viewer
	float3 H	= normalize( L + V );				// halfway vector	
	
		
	float att	= 1.0 / ( vLightAtt.x + vLightAtt.y * dl + vLightAtt.z * dl * dl );
		
	float3 Id	= diffuseAlbedo.rgb * saturate( dot( normal, L) );		
	float3 Is	= specularAlbedo.rgb * pow( saturate( dot(normal, H) ), specularAlbedo.w);
	
	return  (Id + Is) * vLightColor.rgb * att;
}

//-------------------------------------------------------------------------------------------------
// Compute spot lighting
//-------------------------------------------------------------------------------------------------
float3 CalcSpotLighting(	in float3 normal, 
							in float3 position, 
							in float3 diffuseAlbedo,
							in float4 specularAlbedo)
{	
	float3 L	= vLightPos - position;				// light dir
	float  dl	= length(L);						// distance to light source	
	L			= L / dl;
	float3 V	= normalize( vEyePos - position );	// directrion to viewer
	float3 H	= normalize( L + V );				// Halfway vector	
	
	float3 sd	= normalize( vLightDir );			// direction to light source
	float rho	= dot(-L, sd);
	float spot	= pow( saturate( (rho - vLightSpot.x) / (vLightSpot.y - vLightSpot.x) ), vLightSpot.z );			
	float att	= spot / ( vLightAtt.x + vLightAtt.y * dl + vLightAtt.z * dl * dl );

	float3 Id	= diffuseAlbedo.rgb * saturate( dot( normal, L) );		
	float3 Is	= specularAlbedo.rgb * pow( saturate( dot(normal, H) ), specularAlbedo.w);
	
	return  (Id + Is) * vLightColor.rgb * att;
}



//-------------------------------------------------------------------------------------------------
// Pixel Shader for ambient light 
//-------------------------------------------------------------------------------------------------
float4 PSAmbient(in VSOutputDS input, in float4 screenPos : SV_Position, in uint coverage : SV_Coverage  ) : SV_Target
{
	int3 samplePos  = int3( screenPos.xy, 0 );

#ifdef DEFERRED_MSAA	
	// variable for color
	float3 lightingColor = 0;

	// Calculate lighting for DEFERRED_MSAA samples
	float numSamplesApplied = 0.0f;
	for ( int i = 0; i < nMsaaSamples; ++i )
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
	
#ifdef DEFERRED_MSAA	
	// variable for color
	float3 lightingColor = 0;

	// Calculate lighting for DEFERRED_MSAA samples
	float numSamplesApplied = 0.0f;
	for ( int i = 0; i < nMsaaSamples; ++i )
	{
		// We only want to calculate lighting for a sample if the light volume is covered.
		// We determine this using the input coverage mask.
		//if ( coverage & ( 1 << i ) )
		{
			float4 normal			= NormalTexture.Load( samplePos.xy, i );	
				
			//if(normal.w==0) 
			//	continue;

			float4 position			= PositionTexture.Load( samplePos.xy, i );
			float4 diffuseAlbedo	= DiffuseAlbedoTexture.Load( samplePos.xy, i );
			float4 specularAlbedo	= SpecularAlbedoTexture.Load( samplePos.xy, i );

				
			lightingColor		   += (normal.w == 0) ? float3(0.0f,0.0f,0.0f) : CalcDirLighting( normal.xyz, position.xyz, diffuseAlbedo.xyz, specularAlbedo );
				
			++numSamplesApplied;
				
		}
	}
	lightingColor /= numSamplesApplied;	
	return float4( lightingColor, 1.0f );
	//return float4(0,0,0,0);
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
// Pixel Shader for point light
//-------------------------------------------------------------------------------------------------
float4 PSPointLight(in VSOutputDS input, in float4 screenPos : SV_Position,  in uint coverage : SV_Coverage  ) : SV_Target
{
	int3 samplePos			= int3( screenPos.xy, 0 );

#ifdef DEFERRED_MSAA				
	// variable for color
	float3 lightingColor = 0;

	// Calculate lighting for DEFERRED_MSAA samples
	float numSamplesApplied = 0.0f;
	for ( int i = 0; i < nMsaaSamples; ++i )
	{
		// We only want to calculate lighting for a sample if the light volume is covered.
		// We determine this using the input coverage mask.
		//if ( coverage & ( 1 << i ) )
		{
			float4 normal			= NormalTexture.Load( samplePos.xy, i );	

			float4 position			= PositionTexture.Load( samplePos.xy, i );
			float4 diffuseAlbedo	= DiffuseAlbedoTexture.Load( samplePos.xy, i );
			float4 specularAlbedo	= SpecularAlbedoTexture.Load( samplePos.xy, i );

			lightingColor		   += (normal.w==0) ? float3(0.0f,0.0f,0.0f) : CalcPointLighting( normal.xyz, position.xyz, diffuseAlbedo.xyz, specularAlbedo );
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
	//return float4( CalcPointLighting( normal.xyz, position.xyz, diffuseAlbedo.xyz, specularAlbedo ), 1.0f );
	return float4( 0.1, 0.1, 0.1, 1 );
#endif	
}


//-------------------------------------------------------------------------------------------------
// Pixel Shader for spot light
//-------------------------------------------------------------------------------------------------
float4 PSSpotLight(in VSOutputDS input, in float4 screenPos : SV_Position,  in uint coverage : SV_Coverage ) : SV_Target
{	
	int3 samplePos			= int3( screenPos.xy, 0 );
		
#ifdef DEFERRED_MSAA		
	// variable for color
	float3 lightingColor = 0;

	// Calculate lighting for DEFERRED_MSAA samples
	float numSamplesApplied = 0.0f;
	for ( int i = 0; i < nMsaaSamples; ++i )
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

			lightingColor		   += (normal.w==0) ? float3(0.0f,0.0f,0.0f) : CalcSpotLighting( normal.xyz, position.xyz, diffuseAlbedo.xyz, specularAlbedo );
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




//--------------------------------------------------------------------------------------
// Screen-Space Ambient Occlusion global variables
//--------------------------------------------------------------------------------------

#ifdef SSAO

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

	// Returns the z-coordinate of the view position, calculated from the depth value stored
	// in the DepthTexture
	float GetZView(in float zNDC) {
		return -mLightProj._m32 / (zNDC + mLightProj._m22);
	}

	// Returns the normalized device coordinates in the x- and y-coordinates and the non-linear
	// depth value stored in the DepthTexture in the z-coordinate.
	float3 GetNDC(in float3 samplePos)
	{
		float3 ndc = float3(
			(2.0f * samplePos.x) * vInvViewportSize.x - 1.0f,
			1.0f - (2.0f * samplePos.y) * vInvViewportSize.y,
#ifdef DEFERRED_MSAA
			DepthTexture.Load(samplePos,0).x
#else
			DepthTexture.Load(samplePos).x
#endif
		);
		return ndc;
	}

	//
	// Returns the view position of the surface. Use the return value of GetNDC as input
	float3 CalcViewPosition(in float3 ndcPosition)
	{
		float zView = -mLightProj._m32 / (ndcPosition.z + mLightProj._m22);
		float4 projPosition = float4(-zView*ndcPosition, -zView);
		return mul(projPosition, mInvProjection).xyz;
	}
	

	//--------------------------------------------------------------------------------------
	// Pixel Shader for Screen-Space Ambient Occlusion
	//--------------------------------------------------------------------------------------
	float4 PSSSAO(in VSOutputDS input, in float4 screenPos : SV_Position ) : SV_Target
	{
		int3 samplePos			= int3( screenPos.xy, 0 );

		// get pixel normal
#ifdef DEFERRED_MSAA
		float3 normal = NormalTexture.Load( samplePos,0 ).xyz;
#else
		float3 normal = NormalTexture.Load( samplePos ).xyz;
#endif

		// calc rotation matrix
		int repeatedRandom = (samplePos.x & 3) + (samplePos.y & 3) * 4; // repeats every 4x4 pixels
		float3 vRotation = randomVectors[repeatedRandom]; 

		float3x3 rotMat;
		float h = 1.f / (1.f + vRotation.z);
		rotMat._m00 = h*vRotation.y*vRotation.y + vRotation.z;
		rotMat._m01 = -h*vRotation.y*vRotation.x;
		rotMat._m02 = -vRotation.x;
		rotMat._m10 = -h*vRotation.y*vRotation.x;
		rotMat._m11 = h*vRotation.x*vRotation.x + vRotation.z;
		rotMat._m12 = -vRotation.y;
		rotMat._m20 = vRotation.x;
		rotMat._m21 = vRotation.y;
		rotMat._m22 = vRotation.z;

		// move sample position to center of pixel
		float3 samplePosCenter = samplePos + float3(0.5f,0.5f,0.0f);
		float2 screenTC = (samplePosCenter.xy) * vInvViewportSize;
		float3 currentNDC = GetNDC(samplePosCenter);
		float3 currentViewPosition = CalcViewPosition(currentNDC); // view position of current fragment
		float fSceneDepthP = -currentViewPosition.z; // linear depth (0 = at camera, positive = distance to camera)

		// apply offsets in world space for temporal consistency. if this is done in screen space with
		// a perspective projection, it creates banding. If this is done in view space,
		// the brightness of a surface flickers a little bit when the camera angle changes.
		float3 currentWorldPosition = mul(float4(currentViewPosition, 1.0), mInvView).xyz;
		//float3 normal = normalize(cross(ddy_fine(currentWorldPosition), ddx_fine(currentWorldPosition)));
	
		const int nSamplesNum = 16;
		float offsetScale = 0.01;
		const float offsetScaleStep = 1 + 2.4/nSamplesNum;

		float Accessibility = 0.0;

		for (int i=0; i<(nSamplesNum/8); i++)
			for (int x=-1; x<=1; x+=2)
				for (int y=-1; y<=1; y+=2)
					for (int z=-1; z<=1; z+=2) {
						// the offset should be the same magnitude in screen space, thus multiply with fSceneDepthP
						float3 vOffset = normalize(float3(x,y,z)) * (offsetScale *= offsetScaleStep) * fSceneDepthP;

						float3 vRotatedOffset = mul(vOffset, rotMat);

						// put in the correct hemisphere
						int direction = -sign(dot(vRotatedOffset, normal)); // will be 1 if the offset is in the wrong hemisphere, <= 0 otherwise
						vRotatedOffset = lerp(vRotatedOffset, reflect(vRotatedOffset, normal), direction); // flip if necessary

						// calculate sample position
						float3 sampleWorldPosition = currentWorldPosition + vRotatedOffset;
						float4 sampleTC = mul(float4(sampleWorldPosition, 1.f), mViewProjection);
						sampleTC /= sampleTC.w;
						float sampleDepth = -GetZView(sampleTC.z); // linear depth value at sample position (reference value)

						
#ifdef DEFERRED_MSAA
						// sample depth texture
						sampleTC.xy =  float2(sampleTC.x*0.5f + 0.5f, 0.5f - sampleTC.y*0.5f) * vViewport.xy;
						float depthSample = DepthTexture.Load(sampleTC.xy, 0).r;
						float fSceneDepthS = -GetZView(depthSample)+0.005; // heightfield linear depth value at sample position (value in current scene)
						// bias depth, because we cannot sample depth texture with interpolation
#else
						// sample depth texture
						sampleTC.xy = float2(sampleTC.x*0.5f + 0.5f, 0.5f - sampleTC.y*0.5f);
						float depthSample = DepthTexture.Sample(NormalSampler, sampleTC.xy).r;
						float fSceneDepthS = -GetZView(depthSample); // heightfield linear depth value at sample position (value in current scene)
#endif
						
						// will be 1 if sample point is a lot closer to camera than our current pixel
						// between 0 and 1 if we have a potential occluder that is close enough to out current pixel to be valid
						float fRangeIsInvalid = saturate((fSceneDepthP - fSceneDepthS) / fSceneDepthS); 

						// if range is valid -> check whether reference sample is above height field and increment if this is the case.
						Accessibility += lerp(fSceneDepthS > sampleDepth, 1.0, fRangeIsInvalid);
					}

		Accessibility = Accessibility / nSamplesNum;

		// this is just for background with undefined world position in the test scenes, otherwise it flickers like crazy;
		// should be removed if there is a sky box or no empty space
		Accessibility = lerp(Accessibility, 1.0, fSceneDepthP > 500.0);

		return float4(Accessibility,Accessibility,Accessibility,1.0);
		//return float4(currentWorldPosition,1);
	
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
		
		/// Implement 4x4 average blur
		/// You should sample the ScreenSpaceBufferTexture0 for this pass.
		color += float4(ScreenSpaceBufferTexture0.Sample( PointSampler, float2(uv.x, uv.y)).rrr, 1.0);
 
		return color;	
	}

	float blurFalloff = 0.08f;
	float4 fBlurSharpness = float4(60,0,0,1);
	
	///
	///
	float BlurFunction0(float2 uv, float r, float center_c, float center_d, inout float w_total, int index)
	{


		float c = ScreenSpaceBufferTexture0.Sample( PointSampler, uv).r;
#ifdef DEFERRED_MSAA
		float d = -GetZView(DepthTexture.Load(uv*vViewport.xy,0).r);
#else
		float d = -GetZView(DepthTexture.Sample(PointSampler, uv).r);
#endif

		float ddiff = d - center_d;
		float w = exp(-r*r*blurFalloff - ddiff*ddiff*fBlurSharpness.x);
		w_total += w;

		return w*c;
	}

	///
	///
	float BlurFunction1(float2 uv, float r, float center_c, float center_d, inout float w_total, int index)
	{
		float c = ScreenSpaceBufferTexture1.Sample( PointSampler, uv).r;
#ifdef DEFERRED_MSAA
		float d = -GetZView(DepthTexture.Load(uv*vViewport.xy,0).r);
#else
		float d = -GetZView(DepthTexture.Sample(PointSampler, uv).r);
#endif

		float ddiff = d - center_d;
		float w = exp(-r*r*blurFalloff - ddiff*ddiff*fBlurSharpness.x);
		w_total += w;

		return w*c;
	}


	//--------------------------------------------------------------------------------------
	// Horizontal Blur Pass
	// http://www.gamerendering.com/2008/10/11/gaussian-blur-filter-shader/
	//--------------------------------------------------------------------------------------
	float4 PSHBlur(in VSOutputDS input, in float4 screenPos : SV_Position ) : SV_Target
	{
		float2 uv = (screenPos.xy) * vInvViewportSize; 
		float4 color = float4(0.0, 0.0, 0.0, 0.0);

		float b = 0.0f;
		float w_total = 0.0f;
		float center_c = ScreenSpaceBufferTexture0.Sample( PointSampler, uv).r;
#ifdef DEFERRED_MSAA
		float center_d = -GetZView(DepthTexture.Load(screenPos.xy,0).r);
#else
		float center_d = -GetZView(DepthTexture.Sample(PointSampler, uv).r);
#endif
	
		for(int x=-1; x<=2; x++) {
			float2 sampleUV = uv + float2(vInvViewportSize.x * x, 0.0f);
			b += BlurFunction0(sampleUV, x-0.5f, center_c, center_d, w_total, 0);
		}

		float result = b/w_total;
		//result = center_c;
		color = float4(result,result,result,1.0f);

		return color;
	}

	//--------------------------------------------------------------------------------------
	// Vertical Blur Pass
	//--------------------------------------------------------------------------------------
	float4 PSVBlur(in VSOutputDS input, in float4 screenPos : SV_Position ) : SV_Target
	{
		float2 uv = (screenPos.xy) * vInvViewportSize;

		float4 color = float4(0.0, 0.0, 0.0, 0.0);
		//float2 uv = float2(input.tc.x, 1.0 - input.tc.y);

		
		float b = 0.0f;
		float w_total = 0.0f;
		float center_c = ScreenSpaceBufferTexture1.Sample( PointSampler, uv).r;
#ifdef DEFERRED_MSAA
		float center_d = -GetZView(DepthTexture.Load(screenPos.xy,0).r);
#else
		float center_d = -GetZView(DepthTexture.Sample(PointSampler, uv).r);
#endif
	
		for(int y=-1; y<=2; y++) {
			float2 sampleUV = uv + float2(0.0f,vInvViewportSize.y * y);
			b += BlurFunction1(sampleUV, y-0.5f, center_c, center_d, w_total, 0);
		}

		float result = b/w_total;
		//result = center_c;
		color = float4(result,result,result,1.0f);

		return color;
	}

	//--------------------------------------------------------------------------------------
	// FXAA Pass
	//--------------------------------------------------------------------------------------
	float4 PSFXAA(in VSOutputDS input, in float4 screenPos : SV_Position ) : SV_Target
	{
		// TODO:
		// Implement FXAA
		// Use ScreenSpaceBufferTexture2 to sample color information
		int3 samplePos = int3( screenPos.xy, 0 );
		float3 color = ScreenSpaceBufferTexture2.Load( samplePos ).rgb + float3(0.5,0.0,0.0);
		return float4(color,1.0f);
	}
	

#endif // SSAO

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

#ifdef SSAO
technique11 RenderScreenSpace
{
	pass SSAOPass
	{
		SetRasterizerState	( RSBack );
		SetDepthStencilState( DSSNoDepth, 0);
		SetBlendState		( BSNoBlending, float4( 1.0f, 1.0f, 1.0f, 1.0f ), 0xffffffff );
		SetVertexShader		( CompileShader( vs_5_0, VSSimple() ) );
		SetHullShader		( NULL );
		SetDomainShader		( NULL );
		SetGeometryShader	( NULL );        
		SetPixelShader		( CompileShader( ps_5_0, PSSSAO() ) );        
	}

	pass MergePass
	{
		SetRasterizerState	( RSBack );
		SetDepthStencilState( DSSNoDepth, 0);
		SetBlendState		( BSNoBlending, float4( 1.0f, 1.0f, 1.0f, 1.0f ), 0xffffffff );
		SetVertexShader		( CompileShader( vs_5_0, VSSimple() ) );
		SetHullShader		( NULL );
		SetDomainShader		( NULL );
		SetGeometryShader	( NULL );        
		SetPixelShader		( CompileShader( ps_5_0, PSMerge() ) );        
	}

	pass Blur4x4Pass
	{
		SetRasterizerState	( RSBack );
		SetDepthStencilState( DSSNoDepth, 0);
		SetBlendState		( BSNoBlending, float4( 1.0f, 1.0f, 1.0f, 1.0f ), 0xffffffff );
		SetVertexShader		( CompileShader( vs_5_0, VSSimple() ) );
		SetHullShader		( NULL );
		SetDomainShader		( NULL );
		SetGeometryShader	( NULL );        
		SetPixelShader		( CompileShader( ps_5_0, PSBlur4x4() ) );        
	}

	pass BlurHPass
	{
		SetRasterizerState	( RSBack );
		SetDepthStencilState( DSSNoDepth, 0);
		SetBlendState		( BSNoBlending, float4( 1.0f, 1.0f, 1.0f, 1.0f ), 0xffffffff );
		SetVertexShader		( CompileShader( vs_5_0, VSSimple() ) );
		SetHullShader		( NULL );
		SetDomainShader		( NULL );
		SetGeometryShader	( NULL );        
		SetPixelShader		( CompileShader( ps_5_0, PSHBlur() ) );        
	}

	pass BlurVPass
	{
		SetRasterizerState	( RSBack );
		SetDepthStencilState( DSSNoDepth, 0);
		SetBlendState		( BSNoBlending, float4( 1.0f, 1.0f, 1.0f, 1.0f ), 0xffffffff );
		SetVertexShader		( CompileShader( vs_5_0, VSSimple() ) );
		SetHullShader		( NULL );
		SetDomainShader		( NULL );
		SetGeometryShader	( NULL );        
		SetPixelShader		( CompileShader( ps_5_0, PSVBlur() ) );        
	}

	pass FXAAPass
	{
		SetRasterizerState	( RSBack );
		SetDepthStencilState( DSSNoDepth, 0);
		SetBlendState		( BSNoBlending, float4( 1.0f, 1.0f, 1.0f, 1.0f ), 0xffffffff );
		SetVertexShader		( CompileShader( vs_5_0, VSSimple() ) );
		SetHullShader		( NULL );
		SetDomainShader		( NULL );
		SetGeometryShader	( NULL );        
		SetPixelShader		( CompileShader( ps_5_0, PSFXAA() ) );  
	}
}  
#endif // SSAO

#endif