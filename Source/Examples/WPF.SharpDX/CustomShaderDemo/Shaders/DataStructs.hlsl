#ifndef DATASTRUCTS_FX
#define DATASTRUCTS_FX
#pragma pack_matrix( row_major )
//--------------------------------------------------------------------------------------
// VERTEX AND PIXEL SHADER INPUTS
//--------------------------------------------------------------------------------------
#define LIGHTS 8
//--------------------------------------------------------------------------------------
// Light Buffer
//--------------------------------------------------------------------------------------
struct LightStruct
{
    int iLightType; //4
    float3 paddingL;
	// the light direction is here the vector which looks towards the light
    float4 vLightDir; //8
    float4 vLightPos; //12
    float4 vLightAtt; //16
    float4 vLightSpot; //(outer angle , inner angle, falloff, free), 20
    float4 vLightColor; //24
    matrix mLightView; //40
    matrix mLightProj; //56
};

struct VSSkinnedInput
{
    float4 p : POSITION;
    float3 n : NORMAL;
    float3 t1 : TANGENT;
    float3 t2 : BINORMAL;
    int4 bones : BONEIDS;
    float4 boneWeights : BONEWEIGHTS;
};

struct VSSkinnedOutput
{
    float4 p : POSITION;
    float3 n : NORMAL;
    float3 t1 : TANGENT;
    float3 t2 : BINORMAL;
};

struct VSInput
{
    float4 p : POSITION;
    float3 n : NORMAL;
    float3 t1 : TANGENT;
    float3 t2 : BINORMAL;
    float2 t : TEXCOORD;
    float4 c : COLOR;
    float4 mr0 : TEXCOORD1;
    float4 mr1 : TEXCOORD2;
    float4 mr2 : TEXCOORD3;
    float4 mr3 : TEXCOORD4;
};

struct VSInputBatched
{
    float4 p : POSITION;
    float3 n : NORMAL;
    float3 t1 : TANGENT;
    float3 t2 : BINORMAL;
    float2 t : TEXCOORD;
    float4 c : COLOR; // if batched x: diffuse, y: emissive, z: specular, w: reflect; PBR : albedo
    float4 c1 : COLOR1; // if batched x: ambient, y: specular shininess, z: diffuse alpha; PBR : AO, Roughtness, Metallic
};

struct VSBoneSkinInput
{
    float4 p : POSITION;
    float3 n : NORMAL;
    float3 t1 : TANGENT;
    float3 t2 : BINORMAL;
    float2 t : TEXCOORD;
    float4 c : COLOR;

    float4 mr0 : TEXCOORD1;
    float4 mr1 : TEXCOORD2;
    float4 mr2 : TEXCOORD3;
    float4 mr3 : TEXCOORD4;

    int4 bones : BONEIDS;
    float4 boneWeights : BONEWEIGHTS;

};

//--------------------------------------------------------------------------------------
// VERTEX AND PIXEL SHADER INPUTS
//--------------------------------------------------------------------------------------
struct VSInstancingInput
{
    float4 p : POSITION;
    float3 n : NORMAL;
    float3 t1 : TANGENT;
    float3 t2 : BINORMAL;
    float2 t : TEXCOORD;
    float4 c : COLOR;

    float4 mr0 : TEXCOORD1;
    float4 mr1 : TEXCOORD2;
    float4 mr2 : TEXCOORD3;
    float4 mr3 : TEXCOORD4;

    float4 diffuseC : COLOR1;
    float4 emissiveC : COLOR2;
    float2 tOffset : TEXCOORD5;
};

//--------------------------------------------------------------------------------------
struct PSInput
{
    float4 p : SV_POSITION;
    float4 vEye : POSITION0;
    float3 n : NORMAL; // normal
    float4 wp : POSITION1;
    float4 sp : TEXCOORD1;
    float2 t : TEXCOORD0; // tex coord	
    float3 t1 : TANGENT; // tangent
    float3 t2 : BINORMAL; // bi-tangent	
    float4 c : COLOR; // solid color (for debug)
    float4 c2 : COLOR1; //vMaterialEmissive
    float4 cDiffuse : COLOR2; //vMaterialDiffuse
};

struct PSPlaneGridInput
{
    float4 p : SV_POSITION;
    float2 uv : POSITION0;
    float3 wp : POSITION1;
    float4 sp : POSITION2;
};

struct PSWireframeInput
{
    float4 p : SV_POSITION;
    float z : POSITION0;
};

struct PSInputClip
{
    float4 p : SV_POSITION;
    float4 vEye : POSITION0;
    float3 n : NORMAL; // normal
    float4 wp : POSITION1;
    float4 sp : TEXCOORD1;
    float2 t : TEXCOORD0; // tex coord	
    float3 t1 : TANGENT; // tangent
    float3 t2 : BINORMAL; // bi-tangent	
    float4 c : COLOR; // solid color (for debug)
    float4 c2 : COLOR1; //vMaterialEmissive + vMaterialAmbient * vLightAmbient
    float4 cDiffuse : COLOR2; //vMaterialDiffuse
    float4 clipPlane : SV_ClipDistance0;
};

struct PSInputXRay
{
    float4 p : SV_POSITION;
    float4 vEye : POSITION0;
    float3 n : NORMAL; // normal
};

struct PSShadow
{
    float4 p : SV_POSITION;
};


struct PSOITOutput
{
    float4 color : SV_Target0;
    float4 alpha : SV_Target1;
};
//--------------------------------------------------------------------------------------
// CUBE-MAP funcs
//--------------------------------------------------------------------------------------
struct PSInputCube
{
    float4 p : SV_POSITION;
    float3 t : TEXCOORD;
    float4 c : COLOR;
};

//--------------------------------------------------------------------------------------
// Billboard VERTEX AND PIXEL SHADER INPUTS
//--------------------------------------------------------------------------------------
//struct VSInputBT
//{
//	float4 p : POSITION;
//    float4 foreground : COLOR;
//    float4 background : COLOR1;
//	float4 t : TEXCOORD0; // t.xy = texture coords, t.zw = offset in pixels.
//	float4 mr0 : TEXCOORD1;
//	float4 mr1 : TEXCOORD2;
//	float4 mr2 : TEXCOORD3;
//	float4 mr3 : TEXCOORD4;
//};

struct VSInputBT
{
    float4 p : POSITION;
    float4 foreground : COLOR;
    float4 background : COLOR1;
    float2 t0 : TEXCOORD0;
    float2 t3 : TEXCOORD1;
    float2 offTL : TEXCOORD2;
    float2 offTR : TEXCOORD3;
    float2 offBL : TEXCOORD4;
    float2 offBR : TEXCOORD5;
    float4 mr0 : TEXCOORD6;
    float4 mr1 : TEXCOORD7;
    float4 mr2 : TEXCOORD8;
    float4 mr3 : TEXCOORD9;
};

struct GSInputBT
{
    float4 p : POSITION;
    float4 foreground : COLOR;
    float4 background : COLOR1;
    float2 t0 : TEXCOORD0;
    float2 t3 : TEXCOORD1;
    float2 offTL : TEXCOORD2;
    float2 offTR : TEXCOORD3;
    float2 offBL : TEXCOORD4;
    float2 offBR : TEXCOORD5;
};

struct VSInputBTInstancing
{
    float4 p : POSITION;
    float4 foreground : COLOR;
    float4 background : COLOR1;
    float2 t0 : TEXCOORD0;
    float2 t3 : TEXCOORD1;
    float2 offTL : TEXCOORD2;
    float2 offTR : TEXCOORD3;
    float2 offBL : TEXCOORD4;
    float2 offBR : TEXCOORD5;
    float4 mr0 : TEXCOORD6;
    float4 mr1 : TEXCOORD7;
    float4 mr2 : TEXCOORD8;
    float4 mr3 : TEXCOORD9;

    float4 diffuseC : COLOR2;
    float2 tScale : TEXCOORD10;
    float2 tOffset : TEXCOORD11;
};

struct PSInputBT
{
    float4 p : SV_POSITION;
    float4 foreground : COLOR;
    float4 background : COLOR1;
    noperspective
	float2 t : TEXCOORD;
    float4 vEye : POSITION0;
};

//--------------------------------------------------------------------------------------
// Point Or Line VERTEX AND PIXEL SHADER INPUTS
//--------------------------------------------------------------------------------------
struct VSInputPS
{
    float4 p : POSITION;
    float4 c : COLOR;
    float4 mr0 : TEXCOORD0;
    float4 mr1 : TEXCOORD1;
    float4 mr2 : TEXCOORD2;
    float4 mr3 : TEXCOORD3;
};

struct GSInputPS
{
    float4 p : SV_POSITION;
    float4 wp : POSITION0;
    float4 c : COLOR;
    float4 vEye : POSITION1;
};

struct PSInputPS
{
    float4 p : SV_POSITION;
    float4 vEye : POSITION0;
    float4 c : COLOR;
    noperspective
	float3 t : TEXCOORD;
};

//--------------------------------------------------------------------------------------
// SHADER STRUCTURES
//--------------------------------------------------------------------------------------
struct HSConstantDataOutput
{
    float Edges[3] : SV_TessFactor;
    float Inside : SV_InsideTessFactor;
    float Sign : SIGN;
   
    float3 f3B210 : POSITION3;
    float3 f3B120 : POSITION4;
    float3 f3B021 : POSITION5;
    float3 f3B012 : POSITION6;
    float3 f3B102 : POSITION7;
    float3 f3B201 : POSITION8;
    float3 f3B111 : CENTER;
};

//--------------------------------------------------------------------------------------
struct HSConstantDataOutputQuads
{
    float Edges[4] : SV_TessFactor;
    float Inside[2] : SV_InsideTessFactor;
    float Sign : SIGN;
    float3 vEdgePos[8] : EDGEPOS;
    float3 vInteriorPos[4] : INTERIORPOS;
};

//--------------------------------------------------------------------------------------
struct HSInput
{
    float3 p : POSITION;
    float2 t : TEXCOORD0;
    float3 n : TEXCOORD1;
    float3 t1 : TEXCOORD2;
    float3 t2 : TEXCOORD3;
    float4 c : COLOR;
    float4 c2 : COLOR1;
    float tessF : TESS;
};

//--------------------------------------------------------------------------------------
struct VSIn
{
    float4 p : POSITION;
    float3 n : NORMAL;
    float3 t1 : TANGENT;
    float3 t2 : BINORMAL;
    float2 t : TEXCOORD;
    float4 c : COLOR;
};

//
struct PSInputScreenQuad
{
    float4 p : POSITION;
    float4 c : COLOR;
};

//----------------Particle---------------------
struct Particle
{
    float3 position;
    float initEnergy;
    float3 velocity;
    float energy;
    float4 color;
    float3 initAccelleration;
    float dissipRate;
    uint2 TexColRow;
};

//--------------------------------------------------------------------------------
// Inter-stage structures
//--------------------------------------------------------------------------------
struct ParticleVS_INPUT
{
    float4 mr0 : TEXCOORD1;
    float4 mr1 : TEXCOORD2;
    float4 mr2 : TEXCOORD3;
    float4 mr3 : TEXCOORD4;
};
//--------------------------------------------------------------------------------
struct ParticleGS_INPUT
{
    float3 position : Position;
    float energy : Energy;
    float4 color : COLOR0;
    float initEnergy : Energy1;
    uint2 texColRow : TexOff;
};
//--------------------------------------------------------------------------------
struct ParticlePS_INPUT
{
    float4 position : SV_Position;
    float4 color : COLOR0;
    noperspective
    float2 texcoords : TEXCOORD0;
    float opacity : OPACITY0;
    float z : Position0;
};

struct ScreenDupVS_INPUT
{
    float4 Pos : SV_POSITION;
    noperspective
    float2 Tex : TEXCOORD0;
};

struct MeshOutlinePS_INPUT
{
    float4 Pos : SV_POSITION;
    noperspective
    float2 Tex : TEXCOORD0;
};

struct SpriteVS_INPUT
{
    float2 Pos : POSITION;
    float2 UV : TEXCOORD0;
    float4 Color : COLOR0;
};

struct SpritePS_INPUT
{
    float4 Pos : SV_POSITION;
    float4 Color : COLOR0;
    float2 UV : TEXCOORD0;
};

struct VolumePS_INPUT
{
    float4 pos : SV_POSITION;
    float4 wp : POSITION0;
    float4 mPos : TEXCOORD0;
    noperspective
    float2 tex : TEXCOORD1;
};
#endif