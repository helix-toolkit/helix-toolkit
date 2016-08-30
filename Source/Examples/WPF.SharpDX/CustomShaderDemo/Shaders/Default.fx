//--------------------------------------------------------------------------------------
// File: Default Effect for HelixToolkitDX
// Author: Przemyslaw Musialski
// Date: 11/11/12
// References & Sources: 
// light equations from OpenGL Spec: http://www.opengl.org/documentation/specs/version1.2/OpenGL_spec_1.2.1.pdf
// spotlight equation from DX9: http://msdn.microsoft.com/en-us/library/windows/desktop/bb174697(v=vs.85).aspx
// parts of the code based on: http://takinginitiative.net/2010/08/30/directx-10-tutorial-8-lighting-theory-and-hlsl/
//--------------------------------------------------------------------------------------

//--------------------------------------------------------------------------------------
// pre-processor includes
//--------------------------------------------------------------------------------------
#include "./Shaders/Common.fx"
#include "./Shaders/Material.fx"
#include "./Shaders/Lines.fx"
#include "./Shaders/Points.fx"
#include "./Shaders/BillboardText.fx"

//--------------------------------------------------------------------------------------
// pre-processor defines
//--------------------------------------------------------------------------------------
#define LIGHTS 16

//--------------------------------------------------------------------------------------
// Constant Buffer Variables
//--------------------------------------------------------------------------------------
cbuffer cbPerFrame
{
	int iLightType[LIGHTS];
	// the light direction is here the vector which looks towards the light
	float4 vLightDir[LIGHTS];
	float4 vLightPos[LIGHTS];
	float4 vLightAtt[LIGHTS];
	float4 vLightSpot[LIGHTS];        //(outer angle , inner angle, falloff, free)
	float4 vLightColor[LIGHTS];	
	matrix mLightView[LIGHTS];
	matrix mLightProj[LIGHTS];
	
};

//--------------------------------------------------------------------------------------
// VERTEX AND PIXEL SHADER INPUTS
//--------------------------------------------------------------------------------------
struct VSInput
{
	float4 p			: POSITION;
	float4 c			: COLOR;
	float2 t			: TEXCOORD; 
	float3 n			: NORMAL;  
	float3 t1			: TANGENT;
    float3 t2			: BINORMAL;

	float4 mr0			: TEXCOORD1;
	float4 mr1			: TEXCOORD2;
	float4 mr2			: TEXCOORD3;
	float4 mr3			: TEXCOORD4;
};

//--------------------------------------------------------------------------------------
struct PSInput
{
	float4 p			: SV_POSITION;  
	float4 wp			: POSITION0;
	float4 sp			: TEXCOORD1;
	float3 n			: NORMAL;	    // normal
	float2 t			: TEXCOORD0;	// tex coord	
	float3 t1			: TANGENT;		// tangent
    float3 t2			: BINORMAL;		// bi-tangent	
	float4 c			: COLOR;		// solid color (for debug)
};



//--------------------------------------------------------------------------------------
// Phong Lighting Reflection Model
//--------------------------------------------------------------------------------------
// Returns the sum of the diffuse and specular terms in the Phong reflection model
// The specular and diffuse reflection constants for the currently loaded material (k_d and k_s) as well
// as other material properties are defined in Material.fx.
float4 calcPhongLighting( float4 LColor, float4 vMaterialTexture, float3 N, float3 L, float3 V, float3 R )
{
	float4 Id = vMaterialTexture * vMaterialDiffuse * saturate( dot(N,L) );
	float4 Is = vMaterialSpecular * pow( saturate(dot(R,V)), sMaterialShininess );	
	return (Id + Is) * LColor;
} 

//--------------------------------------------------------------------------------------
// Blinn-Phong Lighting Reflection Model
//--------------------------------------------------------------------------------------
// Returns the sum of the diffuse and specular terms in the Blinn-Phong reflection model.
float4 calcBlinnPhongLighting( float4 LColor, float4 vMaterialTexture, float3 N, float3 L, float3 H )
{
	float4 Id = vMaterialTexture * vMaterialDiffuse * saturate( dot(N,L) );
	float4 Is = vMaterialSpecular * pow( saturate(dot(N,H)), sMaterialShininess );
	return (Id + Is) * LColor;
}

//--------------------------------------------------------------------------------------
// normal mapping
//--------------------------------------------------------------------------------------
// This function returns the normal in world coordinates.
// The input struct contains tangent (t1), bitangent (t2) and normal (n) of the
// unperturbed surface in world coordinates. The perturbed normal in tangent space
// can be read from texNormalMap.
// The RGB values in this texture need to be normalized from (0, +1) to (-1, +1).
float3 calcNormal(PSInput input)
{	
	if(bHasNormalMap)
	{
		// Normalize the per-pixel interpolated tangent-space
		input.n  = normalize( input.n );
		input.t1 = normalize( input.t1 );
		input.t2 = normalize( input.t2 );		

		// Sample the texel in the bump map.
		float4 bumpMap = texNormalMap.Sample(NormalSampler, input.t);
		// Expand the range of the normal value from (0, +1) to (-1, +1).
		bumpMap = (bumpMap * 2.0f) - 1.0f;
		// Calculate the normal from the data in the bump map.
		input.n = input.n + bumpMap.x * input.t1 + bumpMap.y * input.t2;
	}
	return normalize( input.n );
}

//--------------------------------------------------------------------------------------
// reflectance mapping
//--------------------------------------------------------------------------------------
float4 cubeMapReflection( PSInput input, float4 I )
{
	float3 v = normalize( (float3)input.wp - vEyePos );
	float3 r = reflect( v, input.n );
	return (1.0f-vMaterialReflect)*I + vMaterialReflect*texCubeMap.Sample(LinearSampler, r);	
}

//--------------------------------------------------------------------------------------
// get shadow color
//--------------------------------------------------------------------------------------
float2 texOffset( int u, int v )
{
	return float2( u * 1.0f / vShadowMapSize.x, v * 1.0f / vShadowMapSize.y );
}

//--------------------------------------------------------------------------------------
// get shadow color
//--------------------------------------------------------------------------------------
float shadowStrength(float4 sp)
{				
	sp = sp / sp.w;
	if( sp.x < -1.0f || sp.x > 1.0f || sp.y < -1.0f || sp.y > 1.0f || sp.z < 0.0f  || sp.z > 1.0f ) 
	{
		return 1;
	}
	sp.x = sp.x/+2.0 + 0.5;
	sp.y = sp.y/-2.0 + 0.5;
		
	//apply shadow map bias
	sp.z -= vShadowMapInfo.z;
		
	//// --- not in shadow, hard cut
	//float shadowMapDepth = texShadowMap.Sample(PointSampler, sp.xy).r;
	//if ( shadowMapDepth < sp.z) 
	//{
	//	return 0;
	//}
				
	//// --- basic hardware PCF - single texel
	//float shadowFactor = texShadowMap.SampleCmpLevelZero( CmpSampler, sp.xy, sp.z ).r;

	//// --- PCF sampling for shadow map
	float sum = 0;
	float x = 0, y = 0;
	float range = vShadowMapInfo.y;
	float div = 0.0000001;
		
	// ---perform PCF filtering on a 4 x 4 texel neighborhood
	for (y = -range; y <= range; y += 1.0)
	{
		for (x = -range; x <= range; x += 1.0)
		{
			sum += texShadowMap.SampleCmpLevelZero( CmpSampler, sp.xy + texOffset(x,y), sp.z );
			div++;
		}
	}
		
	float shadowFactor = sum / (float)div;
	float fixTeil = vShadowMapInfo.x;
	float nonTeil = 1-vShadowMapInfo.x;
	// now, put the shadow-strengh into the 0-nonTeil range
	nonTeil = shadowFactor*nonTeil;
	return (fixTeil + nonTeil);		
}



//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING  - Vertex Shader
//--------------------------------------------------------------------------------------
PSInput VShaderDefault( VSInput input )
{
	PSInput output = (PSInput)0;
	float4 inputp  = input.p;	

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

	//set position into light-clip space
	if(bHasShadowMap)
	{
		//for (int i = 0; i < 1; i++)
		{
			output.sp = mul( inputp, mWorld );			
			output.sp = mul( output.sp, mLightView[0] );    
			output.sp = mul( output.sp, mLightProj[0] );
		}
	}
	
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

//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING  - PHONG
//------------------------------------------------------------------------------------
float4 PShaderPhong( PSInput input ) : SV_Target
{   

	//calculate lighting vectors - renormalize vectors	
	input.n = calcNormal( input );

	// get per pixel vector to eye-position
	float3 eye = normalize( vEyePos - input.wp.xyz );

	// light emissive and ambient intensity
	// this variable can be used for light accumulation
	float4 I = vMaterialEmissive + vMaterialAmbient * vLightAmbient;
	
	// get shadow color
	float s = 1;
	if(bHasShadowMap)
	{
		s = shadowStrength( input.sp );
	}

	// add diffuse sampling
	float4 vMaterialTexture = 1.0f;
	if(bHasDiffuseMap)
	{	
		// SamplerState is defined in Common.fx.
		vMaterialTexture = texDiffuseMap.Sample(LinearSampler, input.t);
	}

	// loop over lights
	for (int i = 0; i < LIGHTS; i++)
	{		
		// This framework calculates lighting in world space.
		// For every light type, you should calculate the input values to the
		// calcPhongLighting function, namely light direction and the reflection vector.
		// For computuation of attenuation and the spot light factor, use the
		// model from the DirectX documentation:
		// http://msdn.microsoft.com/en-us/library/windows/desktop/bb172279(v=vs.85).aspx

		if(iLightType[i] == 1) // directional
		{
			float3 d = normalize( (float3)vLightDir[i] );
			float3 r = reflect( -d, input.n );
			I += s * calcPhongLighting( vLightColor[i], vMaterialTexture, input.n, d, eye, r );
		}
		else if(iLightType[i] == 2)  // point
		{
			float3 d = (float3)( vLightPos[i] - input.wp );	 // light dir	
			float dl = length(d);
			d = normalize(d);	
			float3 r = reflect( -d, input.n );
			float att = 1.0f / ( vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl );
			I += att * calcPhongLighting( vLightColor[i], vMaterialTexture, input.n, d, eye, r );
		}
		else if(iLightType[i] == 3)  // spot
		{
			float3 d = (float3)( vLightPos[i] - input.wp );	 // light dir
			float dl = length(d);
			d = normalize(d);	
			float3 r = reflect( -d, input.n );
			float3 sd = normalize( (float3)vLightDir[i] );	// missuse the vLightDir variable for spot-dir

			/* --- this is the OpenGL 1.2 version (not so nice) --- */
			//float spot = (dot(-d, sd));
			//if(spot > cos(vLightSpot[i].x))
			//	spot = pow( spot, vLightSpot[i].y );
			//else
			//	spot = 0.0f;	
			/* --- */

			/* --- this is the  DirectX9 version (better) --- */			
			float rho = dot(-d, sd);
			float spot = pow( saturate( (rho - vLightSpot[i].x) / (vLightSpot[i].y - vLightSpot[i].x) ), vLightSpot[i].z );			
			float att = spot / ( vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl );
			I += att * calcPhongLighting( vLightColor[i], vMaterialTexture, input.n, d, eye, r );
		}
		else
		{		
			//I += 0;
		}		
	}

	/// set diffuse alpha
	I.a = vMaterialDiffuse.a;

	// multiply by vertex colors
	I = I * input.c;

	/// get reflection-color
	if(bHasCubeMap)
	{
		I = cubeMapReflection( input, I );
	}
	
	return I;	
}

//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING - BLINN-PHONG
//--------------------------------------------------------------------------------------
float4 PSShaderBlinnPhong( PSInput input ) : SV_Target
{     	
	// renormalize interpolated vectors
	input.n = calcNormal( input );

	// get per pixel vector to eye-position
	float3 eye = normalize( vEyePos - input.wp.xyz );
				
	// light emissive intensity and add ambient light
	float4 I = vMaterialEmissive + vMaterialAmbient * vLightAmbient;

		// get shadow color
	float s = 1;
	if(bHasShadowMap)
	{
		s = shadowStrength( input.sp );
	}
	
	// add diffuse sampling
	float4 vMaterialTexture = 1.0f;
	if(bHasDiffuseMap)
	{			
		// SamplerState is defined in Common.fx.
		vMaterialTexture *= texDiffuseMap.Sample(LinearSampler, input.t);
	}

	// compute lighting
	for (int i = 0; i < LIGHTS; i++)
	{		
		// Same as for the Phong PixelShader, but use
		// calcBlinnPhongLighting instead.
		if(iLightType[i] == 1) // directional
		{
			float3 d = normalize( (float3)vLightDir[i] );  // light dir	
			float3 h = normalize( eye + d );
			I += s * calcBlinnPhongLighting( vLightColor[i], vMaterialTexture, input.n, d, h );
		}
		else if(iLightType[i] == 2)  // point
		{
			float3 d = (float3)( vLightPos[i] - input.wp );	// light dir
			float dl = length(d);							// light distance
			d = d/dl;										// normalized light dir						
			float3 h = normalize( eye + d );				// half direction for specular
			float att = 1.0f / ( vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl );
			I += att * calcBlinnPhongLighting( vLightColor[i], vMaterialTexture, input.n, d, h );
		}
		else if(iLightType[i] == 3)  // spot
		{			
			float3 d = (float3)( vLightPos[i] - input.wp );	// light dir
			float  dl = length(d);							// light distance
			d = d/dl;										// normalized light dir					
			float3 h = normalize( eye + d );				// half direction for specular
			float3 sd = normalize( (float3)vLightDir[i] );	// missuse the vLightDir variable for spot-dir

			/* --- this is the OpenGL 1.2 version (not so nice) --- */
			//float spot = (dot(-d, sd));
			//if(spot > cos(vLightSpot[i].x))
			//	spot = pow( spot, vLightSpot[i].y );
			//else
			//	spot = 0.0f;	
			/* --- */

			/* --- this is the  DirectX9 version (better) --- */			
			float rho = dot(-d, sd);
			float spot = pow( saturate( (rho - vLightSpot[i].x) / (vLightSpot[i].y - vLightSpot[i].x) ), vLightSpot[i].z );			
			float att = spot / ( vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl );
			I += att*calcBlinnPhongLighting( vLightColor[i], vMaterialTexture, input.n, d, h );
		}
		else
		{
			//I += 0;
		}
	}		
	I.a = vMaterialDiffuse.a;

	// get reflection-color
	if(bHasCubeMap)
	{	
		I = cubeMapReflection( input, I );
	}
		
	return I;   
}

//--------------------------------------------------------------------------------------
// Given Per-Vertex Color
//--------------------------------------------------------------------------------------
float4 PShaderColor( PSInput input ) : SV_Target
{
	return input.c;
}

//--------------------------------------------------------------------------------------
//  Render Positions as Color
//--------------------------------------------------------------------------------------
float4 PShaderPositions( PSInput input ) : SV_Target
{
	return float4( input.wp.xyz, 1);
}

//--------------------------------------------------------------------------------------
//  Render Normals as Color
//--------------------------------------------------------------------------------------
float4 PShaderNormals( PSInput input ) : SV_Target
{
	return float4(input.n*0.5+0.5, 1);
}

//--------------------------------------------------------------------------------------
//  Render Perturbed normals as Color
//--------------------------------------------------------------------------------------
float4 PShaderPerturbedNormals( PSInput input ) : SV_Target
{
	return float4(calcNormal(input)*0.5+0.5, 1.0f);
}

//--------------------------------------------------------------------------------------
//  Render Tangents as Color
//--------------------------------------------------------------------------------------
float4 PShaderTangents( PSInput input ) : SV_Target
{
	return float4(input.t1*0.5+0.5, 1);
}

//--------------------------------------------------------------------------------------
//  Render TexCoords as Color
//--------------------------------------------------------------------------------------
float4 PShaderTexCoords( PSInput input ) : SV_Target
{
	return float4(input.t, 1, 1);
}

//--------------------------------------------------------------------------------------
// diffuse map pixel shader
//--------------------------------------------------------------------------------------
float4 PShaderDiffuseMap ( PSInput input )  : SV_Target
{	
	// SamplerState is defined in Common.fx.
	return texDiffuseMap.Sample(LinearSampler, input.t);
}

//--------------------------------------------------------------------------------------
// empty pixel shader
//--------------------------------------------------------------------------------------
void PShaderEmpty ( PSInput input ) 
{
}


//--------------------------------------------------------------------------------------
// CUBE-MAP funcs
//--------------------------------------------------------------------------------------
struct PSInputCube
{
	float4 p  : SV_POSITION;	
	float3 t  : TEXCOORD;
	float4 c  : COLOR;
};

PSInputCube VShaderCubeMap(float4 p : POSITION)
{
	PSInputCube output = (PSInputCube)0;

	//set position into clip space		
	output.p = mul( p, mWorld );		
	output.p = mul( output.p, mView );    
	output.p = mul( output.p, mProjection ).xyww;	
	
	//set texture coords and color
	//output.t = input.t;	
	//output.c = p;
	
	//Set Pos to xyww instead of xyzw, so that z will always be 1 (furthest from camera)	
	output.t = p.xyz;	

	return output;
}

float4 PShaderCubeMap(PSInputCube input) : SV_Target
{
	return texCubeMap.Sample(LinearSampler, input.t);
	//return float4(input.t,1);
	return float4(1,0,0,1);
}


//--------------------------------------------------------------------------------------
// Techniques
//--------------------------------------------------------------------------------------
technique11 RenderPhong
{
    pass P0
    {
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
        SetVertexShader		( CompileShader( vs_4_0, VShaderDefault() ) );        
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_4_0, PShaderPhong() ) );        
    }
	pass P1
    {
		SetRasterizerState	( RSWire );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
        SetVertexShader		( CompileShader( vs_4_0, VShaderDefault() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_4_0, PShaderPhong() ) );        
    }
}

technique11 RenderBlinn
{
    pass P0
    {
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );

        SetVertexShader		( CompileShader( vs_4_0, VShaderDefault() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_4_0, PSShaderBlinnPhong() ) );        
    }
	pass P1
    {
		SetRasterizerState	( RSWire );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );

        SetVertexShader		( CompileShader( vs_4_0, VShaderDefault() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_4_0, PSShaderBlinnPhong() ) );        
    } 
}

technique11 RenderDiffuse
{
    pass P0
    {
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
        SetVertexShader		( CompileShader( vs_4_0, VShaderDefault() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_4_0, PShaderDiffuseMap() ) );        
    } 
	pass P1
    {
		SetRasterizerState	( RSWire );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
        SetVertexShader		( CompileShader( vs_4_0, VShaderDefault() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_4_0, PShaderDiffuseMap() ) );         
    }
}

technique11 RenderColors
{
    pass P0
    {
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
        SetVertexShader		( CompileShader( vs_4_0, VShaderDefault() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_4_0, PShaderColor() ) );        
    } 
	pass P1
    {
		SetRasterizerState	( RSWire );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
        SetVertexShader		( CompileShader( vs_4_0, VShaderDefault() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_4_0, PShaderColor() ) );         
    }
}

technique11 RenderPositions
{
    pass P0
    {
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
        SetVertexShader		( CompileShader( vs_4_0, VShaderDefault() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_4_0, PShaderPositions() ) );        
    } 
	pass P1
    {
		SetRasterizerState	( RSWire );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
        SetVertexShader		( CompileShader( vs_4_0, VShaderDefault() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_4_0, PShaderPositions() ) );         
    }
}

technique11 RenderNormals
{
    pass P0
    {
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
        SetVertexShader		( CompileShader( vs_4_0, VShaderDefault() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_4_0, PShaderNormals() ) );        
    } 
	pass P1
    {
		SetRasterizerState	( RSWire );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
        SetVertexShader		( CompileShader( vs_4_0, VShaderDefault() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_4_0, PShaderNormals() ) );         
    }
}

technique11 RenderPerturbedNormals
{
    pass P0
    {
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
        SetVertexShader		( CompileShader( vs_4_0, VShaderDefault() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_4_0, PShaderPerturbedNormals() ) );        
    } 
	pass P1
    {
		SetRasterizerState	( RSWire );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
        SetVertexShader		( CompileShader( vs_4_0, VShaderDefault() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_4_0, PShaderPerturbedNormals() ) );         
    }
}

technique11 RenderTangents
{
    pass P0
    {
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
        SetVertexShader		( CompileShader( vs_4_0, VShaderDefault() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_4_0, PShaderTangents() ) );        
    } 
	pass P1
    {
		SetRasterizerState	( RSWire );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
        SetVertexShader		( CompileShader( vs_4_0, VShaderDefault() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_4_0, PShaderTangents() ) );         
    }
}

technique11 RenderTexCoords
{
    pass P0
    {
		//SetRasterizerState	( RSSolid );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
        SetVertexShader		( CompileShader( vs_4_0, VShaderDefault() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_4_0, PShaderTexCoords() ) );        
    } 
	pass P1
    {
		SetRasterizerState	( RSWire );
		SetDepthStencilState( DSSDepthLess, 0);
		SetBlendState		( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
        SetVertexShader		( CompileShader( vs_4_0, VShaderDefault() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_4_0, PShaderTexCoords() ) );         
    }
}

technique11 RenderWires
{
	pass P0
    {
		SetRasterizerState	( RSWire );
		SetDepthStencilState( DSSDepthLess, 0);
		//SetBlendState( BSNoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );

        SetVertexShader		( CompileShader( vs_4_0, VShaderDefault() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_4_0, PShaderPhong() ) );        
    }
    pass P1
    {
		SetRasterizerState	( RSWire );
		SetDepthStencilState( DSSDepthLess, 0);
		//SetBlendState( BSNoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );

        SetVertexShader		( CompileShader( vs_4_0, VShaderDefault() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_4_0, PShaderPhong() ) );           
    }    
}

technique11 RenderCubeMap
{
    pass P0
    {		
		SetRasterizerState	( RSSolidCubeMap );
		SetDepthStencilState( DSSDepthLessEqual, 0);
		SetBlendState		( BSBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );
		//SetBlendState( BSNoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0x00000000 );

        SetVertexShader		( CompileShader( vs_4_0, VShaderCubeMap() ) );
        SetHullShader		( NULL );
        SetDomainShader		( NULL );
        SetGeometryShader	( NULL );
        SetPixelShader		( CompileShader( ps_4_0, PShaderCubeMap() ) );
    }    
}

#include "./Shaders/Custom.fx"
