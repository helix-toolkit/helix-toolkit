//--------------------------------------------------------------------------------------
// File: Materials header for HelixToolkit.Wpf.SharpDX
// Author: Przemyslaw Musialski
// Date: 11/11/12
// References & Sources: 
//--------------------------------------------------------------------------------------


//--------------------------------------------------------------------------------------
// CONSTANT BUFF VARIABLES
//--------------------------------------------------------------------------------------
float4 vMaterialAmbient = 0.25f;  //Ka := surface material's ambient coefficient
float4 vMaterialDiffuse = 0.5f;   //Kd := surface material's diffuse coefficient
float4 vMaterialEmissive = 0.0f;   //Ke := surface material's emissive coefficient
float4 vMaterialSpecular = 0.0f;   //Ks := surface material's specular coefficient
float4 vMaterialReflect = 0.0f;   //Kr := surface material's reflectivity coefficient
float  sMaterialShininess = 1.0f;	  //Ps := surface material's shininess

bool   bHasDiffuseMap = false;
bool   bHasNormalMap = false;
bool   bHasDisplacementMap = false;
bool   bHasCubeMap = false;
bool   bHasInstances = false;
bool   bHasShadowMap = false;

float2 vShadowMapSize = float2(1024, 1024);
float4 vShadowMapInfo = float4(0.005, 1.0, 0.5, 0.0);

float4 vSelectionColor = float4(0.0, 1.0, 1.0, 1.0);


//--------------------------------------------------------------------------------------
// GLOBAL Variables (Varing)
//--------------------------------------------------------------------------------------
float4 vTessellation = float4(2.0f, 0.0f, 0.0f, 0.0f); // the first value is the TS-factor, the other are free!
float4 vLightAmbient = float4(0.2f, 0.2f, 0.2f, 1.0f);

//--------------------------------------------------------------------------------------
// TEXTURES
//--------------------------------------------------------------------------------------

Texture2D	texDiffuseMap;
Texture2D	texNormalMap;
Texture2D	texDisplacementMap;
Texture2D	texSpecularMap;
Texture2D	texShadowMap;
TextureCube texCubeMap;


