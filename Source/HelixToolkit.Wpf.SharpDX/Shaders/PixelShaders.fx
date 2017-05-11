#ifndef PIXELSHADERS_FX
#define PIXELSHADERS_FX

#include "Lighting.fx"
#include "Common.fx"

//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING  - PHONG
//------------------------------------------------------------------------------------
float4 PShaderPhong(PSInput input) : SV_Target
{

	//calculate lighting vectors - renormalize vectors	
	input.n = calcNormal(input);

// get per pixel vector to eye-position
float3 eye = normalize(vEyePos - input.wp.xyz);

// light emissive and ambient intensity
// this variable can be used for light accumulation
float4 I = vMaterialEmissive + vMaterialAmbient * vLightAmbient;

// get shadow color
float s = 1;
if (bHasShadowMap)
{
	s = shadowStrength(input.sp);
}

// add diffuse sampling
float4 vMaterialTexture = 1.0f;
if (bHasDiffuseMap)
{
	// SamplerState is defined in Common.fx.
	vMaterialTexture = texDiffuseMap.Sample(LinearSampler, input.t);
}


float alpha = 1;
if (bHasAlphaMap)
{
	float4 color = texAlphaMap.Sample(LinearSampler, input.t);
	alpha = color[3];
	color[3] = 1;
	vMaterialTexture *= color;
}

// loop over lights
for (int i = 0; i < LIGHTS; i++)
{
	///If light color is black,ignore
	if (vLightColor[i].x == 0 && vLightColor[i].y == 0 && vLightColor[i].z == 0)
	{
		continue;
	}
	// This framework calculates lighting in world space.
	// For every light type, you should calculate the input values to the
	// calcPhongLighting function, namely light direction and the reflection vector.
	// For computuation of attenuation and the spot light factor, use the
	// model from the DirectX documentation:
	// http://msdn.microsoft.com/en-us/library/windows/desktop/bb172279(v=vs.85).aspx

	if (iLightType[i] == 1) // directional
	{
		float3 d = normalize((float3)vLightDir[i]);
		float3 r = reflect(-d, input.n);
		I += s * calcPhongLighting(vLightColor[i], vMaterialTexture, input.n, d, eye, r);
	}
	else if (iLightType[i] == 2)  // point
	{
		float3 d = (float3)(vLightPos[i] - input.wp);	 // light dir	
		float dl = length(d);
		d = normalize(d);
		float3 r = reflect(-d, input.n);
		float att = 1.0f / (vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl);
		I += att * calcPhongLighting(vLightColor[i], vMaterialTexture, input.n, d, eye, r);
	}
	else if (iLightType[i] == 3)  // spot
	{
		float3 d = (float3)(vLightPos[i] - input.wp);	 // light dir
		float dl = length(d);
		d = normalize(d);
		float3 r = reflect(-d, input.n);
		float3 sd = normalize((float3)vLightDir[i]);	// missuse the vLightDir variable for spot-dir

														/* --- this is the OpenGL 1.2 version (not so nice) --- */
														//float spot = (dot(-d, sd));
														//if(spot > cos(vLightSpot[i].x))
														//	spot = pow( spot, vLightSpot[i].y );
														//else
														//	spot = 0.0f;	
														/* --- */

														/* --- this is the  DirectX9 version (better) --- */
		float rho = dot(-d, sd);
		float spot = pow(saturate((rho - vLightSpot[i].x) / (vLightSpot[i].y - vLightSpot[i].x)), vLightSpot[i].z);
		float att = spot / (vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl);
		I += att * calcPhongLighting(vLightColor[i], vMaterialTexture, input.n, d, eye, r);
	}
	else
	{
		//I += 0;
	}
}

/// set diffuse alpha
I.a = vMaterialDiffuse.a;
if (bHasAlphaMap)
{
	I.a *= alpha;
}
// multiply by vertex colors
I = I * input.c;

/// get reflection-color
if (bHasCubeMap)
{
	I = cubeMapReflection(input, I);
}

return I;
}

//--------------------------------------------------------------------------------------
// PER PIXEL LIGHTING - BLINN-PHONG
//--------------------------------------------------------------------------------------
float4 PSShaderBlinnPhong(PSInput input) : SV_Target
{
	// renormalize interpolated vectors
	input.n = calcNormal(input);

// get per pixel vector to eye-position
float3 eye = normalize(vEyePos - input.wp.xyz);

// light emissive intensity and add ambient light
float4 I = vMaterialEmissive + vMaterialAmbient * vLightAmbient;

// get shadow color
float s = 1;
if (bHasShadowMap)
{
	s = shadowStrength(input.sp);
}

// add diffuse sampling
float4 vMaterialTexture = 1.0f;
if (bHasDiffuseMap)
{
	// SamplerState is defined in Common.fx.
	vMaterialTexture *= texDiffuseMap.Sample(LinearSampler, input.t);
}

float alpha = 1;
if (bHasAlphaMap)
{
	float4 color = texAlphaMap.Sample(LinearSampler, input.t);
	alpha = color[3];
	color[3] = 1;
	vMaterialTexture *= color;
}

// compute lighting
for (int i = 0; i < LIGHTS; i++)
{
	///If light color is black,ignore
	if (vLightColor[i].x == 0 && vLightColor[i].y == 0 && vLightColor[i].z == 0)
	{
		continue;
	}
	// Same as for the Phong PixelShader, but use
	// calcBlinnPhongLighting instead.
	if (iLightType[i] == 1) // directional
	{
		float3 d = normalize((float3)vLightDir[i]);  // light dir	
		float3 h = normalize(eye + d);
		I += s * calcBlinnPhongLighting(vLightColor[i], vMaterialTexture, input.n, vMaterialDiffuse, d, h);
	}
	else if (iLightType[i] == 2)  // point
	{
		float3 d = (float3)(vLightPos[i] - input.wp);	// light dir
		float dl = length(d);							// light distance
		d = d / dl;										// normalized light dir						
		float3 h = normalize(eye + d);				// half direction for specular
		float att = 1.0f / (vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl);
		I += att * calcBlinnPhongLighting(vLightColor[i], vMaterialTexture, input.n, vMaterialDiffuse, d, h);
	}
	else if (iLightType[i] == 3)  // spot
	{
		float3 d = (float3)(vLightPos[i] - input.wp);	// light dir
		float  dl = length(d);							// light distance
		d = d / dl;										// normalized light dir					
		float3 h = normalize(eye + d);				// half direction for specular
		float3 sd = normalize((float3)vLightDir[i]);	// missuse the vLightDir variable for spot-dir

														/* --- this is the OpenGL 1.2 version (not so nice) --- */
														//float spot = (dot(-d, sd));
														//if(spot > cos(vLightSpot[i].x))
														//	spot = pow( spot, vLightSpot[i].y );
														//else
														//	spot = 0.0f;	
														/* --- */

														/* --- this is the  DirectX9 version (better) --- */
		float rho = dot(-d, sd);
		float spot = pow(saturate((rho - vLightSpot[i].x) / (vLightSpot[i].y - vLightSpot[i].x)), vLightSpot[i].z);
		float att = spot / (vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl);
		I += att*calcBlinnPhongLighting(vLightColor[i], vMaterialTexture, input.n, vMaterialDiffuse, d, h);
	}
	else
	{
		//I += 0;
	}
}
I.a = vMaterialDiffuse.a;
if (bHasAlphaMap)
{
	I.a *= alpha;
}

// get reflection-color
if (bHasCubeMap)
{
	I = cubeMapReflection(input, I);
}

return I;
}


float4 PSInstancingShaderBlinnPhong(PSInput input) : SV_Target
{
	// renormalize interpolated vectors
	input.n = calcNormal(input);

// get per pixel vector to eye-position
float3 eye = normalize(vEyePos - input.wp.xyz);

// light emissive intensity and add ambient light
float4 I = input.c2;

// get shadow color
float s = 1;
if (bHasShadowMap)
{
	s = shadowStrength(input.sp);
}

// add diffuse sampling
float4 vMaterialTexture = 1.0f;
if (bHasDiffuseMap)
{
	// SamplerState is defined in Common.fx.
	vMaterialTexture *= texDiffuseMap.Sample(LinearSampler, input.t);
}

float alpha = 1;
if (bHasAlphaMap)
{
	float4 color = texAlphaMap.Sample(LinearSampler, input.t);
	alpha = color[3];
	color[3] = 1;
	vMaterialTexture *= color;
}

// compute lighting
for (int i = 0; i < LIGHTS; i++)
{
	///If light color is black,ignore
	if (vLightColor[i].x == 0 && vLightColor[i].y == 0 && vLightColor[i].z == 0)
	{
		continue;
	}
	// Same as for the Phong PixelShader, but use
	// calcBlinnPhongLighting instead.
	if (iLightType[i] == 1) // directional
	{
		float3 d = normalize((float3)vLightDir[i]);  // light dir	
		float3 h = normalize(eye + d);
		I += s * calcBlinnPhongLighting(vLightColor[i], vMaterialTexture, input.n, input.c, d, h);
	}
	else if (iLightType[i] == 2)  // point
	{
		float3 d = (float3)(vLightPos[i] - input.wp);	// light dir
		float dl = length(d);							// light distance
		d = d / dl;										// normalized light dir						
		float3 h = normalize(eye + d);				// half direction for specular
		float att = 1.0f / (vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl);
		I += att * calcBlinnPhongLighting(vLightColor[i], vMaterialTexture, input.n, input.c, d, h);
	}
	else if (iLightType[i] == 3)  // spot
	{
		float3 d = (float3)(vLightPos[i] - input.wp);	// light dir
		float  dl = length(d);							// light distance
		d = d / dl;										// normalized light dir					
		float3 h = normalize(eye + d);				// half direction for specular
		float3 sd = normalize((float3)vLightDir[i]);	// missuse the vLightDir variable for spot-dir

														/* --- this is the OpenGL 1.2 version (not so nice) --- */
														//float spot = (dot(-d, sd));
														//if(spot > cos(vLightSpot[i].x))
														//	spot = pow( spot, vLightSpot[i].y );
														//else
														//	spot = 0.0f;	
														/* --- */

														/* --- this is the  DirectX9 version (better) --- */
		float rho = dot(-d, sd);
		float spot = pow(saturate((rho - vLightSpot[i].x) / (vLightSpot[i].y - vLightSpot[i].x)), vLightSpot[i].z);
		float att = spot / (vLightAtt[i].x + vLightAtt[i].y * dl + vLightAtt[i].z * dl * dl);
		I += att*calcBlinnPhongLighting(vLightColor[i], vMaterialTexture, input.n, input.c, d, h);
	}
	else
	{
		//I += 0;
	}
}
I.a = input.c.a;
if (bHasAlphaMap)
{
	I.a *= alpha;
}

// get reflection-color
if (bHasCubeMap)
{
	I = cubeMapReflection(input, I);
}

return I;
}


//--------------------------------------------------------------------------------------
// Given Per-Vertex Color
//--------------------------------------------------------------------------------------
float4 PShaderColor(PSInput input) : SV_Target
{
	return input.c;
}

//--------------------------------------------------------------------------------------
//  Render Positions as Color
//--------------------------------------------------------------------------------------
float4 PShaderPositions(PSInput input) : SV_Target
{
	return float4(input.wp.xyz, 1);
}

//--------------------------------------------------------------------------------------
//  Render Normals as Color
//--------------------------------------------------------------------------------------
float4 PShaderNormals(PSInput input) : SV_Target
{
	return float4(input.n*0.5 + 0.5, 1);
}

//--------------------------------------------------------------------------------------
//  Render Perturbed normals as Color
//--------------------------------------------------------------------------------------
float4 PShaderPerturbedNormals(PSInput input) : SV_Target
{
	return float4(calcNormal(input)*0.5 + 0.5, 1.0f);
}

//--------------------------------------------------------------------------------------
//  Render Tangents as Color
//--------------------------------------------------------------------------------------
float4 PShaderTangents(PSInput input) : SV_Target
{
	return float4(input.t1*0.5 + 0.5, 1);
}

//--------------------------------------------------------------------------------------
//  Render TexCoords as Color
//--------------------------------------------------------------------------------------
float4 PShaderTexCoords(PSInput input) : SV_Target
{
	return float4(input.t, 1, 1);
}

//--------------------------------------------------------------------------------------
// diffuse map pixel shader
//--------------------------------------------------------------------------------------
float4 PShaderDiffuseMap(PSInput input) : SV_Target
{
	// SamplerState is defined in Common.fx.
	return texDiffuseMap.Sample(LinearSampler, input.t);
}


//--------------------------------------------------------------------------------------
// empty pixel shader
//--------------------------------------------------------------------------------------
void PShaderEmpty(PSInput input)
{
}

float4 PShaderCubeMap(PSInputCube input) : SV_Target
{
	return texCubeMap.Sample(LinearSampler, input.t);
//return float4(input.t,1);
return float4(1,0,0,1);
}

#endif