#ifndef DATASTRUCTS_FX
#define DATASTRUCTS_FX
#pragma pack_matrix( row_major )
//--------------------------------------------------------------------------------------
// VERTEX AND PIXEL SHADER INPUTS
//--------------------------------------------------------------------------------------
struct VSInput
{
	float4 p : POSITION;
	float4 c : COLOR;
	float2 t : TEXCOORD;
	float3 n : NORMAL;
	float3 t1 : TANGENT;
	float3 t2 : BINORMAL;

	float4 mr0 : TEXCOORD1;
	float4 mr1 : TEXCOORD2;
	float4 mr2 : TEXCOORD3;
	float4 mr3 : TEXCOORD4;
};

struct VSBoneSkinInput
{
	float4 p : POSITION;
	float4 c : COLOR;
	float2 t : TEXCOORD;
	float3 n : NORMAL;
	float3 t1 : TANGENT;
	float3 t2 : BINORMAL;

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
	float4 c : COLOR;
	float2 t : TEXCOORD;
	float3 n : NORMAL;
	float3 t1 : TANGENT;
	float3 t2 : BINORMAL;

	float4 mr0 : TEXCOORD1;
	float4 mr1 : TEXCOORD2;
	float4 mr2 : TEXCOORD3;
	float4 mr3 : TEXCOORD4;

	float4 diffuseC : COLOR1;
	float4 ambientC : COLOR2;
	float4 emissiveC : COLOR3;
	float2 tOffset : TEXCOORD5;
};

//--------------------------------------------------------------------------------------
struct PSInput
{
	float4 p : SV_POSITION;
	float4 wp : POSITION0;
	float4 sp : TEXCOORD1;
	float3 n : NORMAL; // normal
	float2 t : TEXCOORD0; // tex coord	
	float3 t1 : TANGENT; // tangent
	float3 t2 : BINORMAL; // bi-tangent	
	float4 c : COLOR; // solid color (for debug)
	float4 c2 : COLOR1;
};

struct PSInputXRay
{
	float4 p : SV_POSITION;
	float4 vEye : POSITION0;
	float3 n : NORMAL; // normal
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
struct VSInputBT
{
	float4 p : POSITION;
    float4 foreground : COLOR;
    float4 background : COLOR1;
	float4 t : TEXCOORD0; // t.xy = texture coords, t.zw = offset in pixels.
	float4 mr0 : TEXCOORD1;
	float4 mr1 : TEXCOORD2;
	float4 mr2 : TEXCOORD3;
	float4 mr3 : TEXCOORD4;
};

struct VSInputBTInstancing
{
	float4 p : POSITION;
    float4 foreground : COLOR;
    float4 background : COLOR1;
	float4 t : TEXCOORD0; // t.xy = texture coords, t.zw = offset in pixels.
	float4 mr0 : TEXCOORD1;
	float4 mr1 : TEXCOORD2;
	float4 mr2 : TEXCOORD3;
	float4 mr3 : TEXCOORD4;

	float4 diffuseC : COLOR1;
	float2 tScale : TEXCOORD5;
	float2 tOffset : TEXCOORD6;
};

struct PSInputBT
{
	float4 p : SV_POSITION;
	float4 foreground : COLOR;
    float4 background : COLOR1;
	float2 t : TEXCOORD;
};

//--------------------------------------------------------------------------------------
// Point Or Line VERTEX AND PIXEL SHADER INPUTS
//--------------------------------------------------------------------------------------
struct VSInputPS
{
	float4 p : POSITION;
	float4 c : COLOR;
	float4 mr0 : TEXCOORD1;
	float4 mr1 : TEXCOORD2;
	float4 mr2 : TEXCOORD3;
	float4 mr3 : TEXCOORD4;
};

struct GSInputPS
{
	float4 p : POSITION;
	float4 c : COLOR;
};

struct PSInputPS
{
	float4 p : SV_POSITION;
	noperspective
		float3 t : TEXCOORD;
	float4 c : COLOR;
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
};

//--------------------------------------------------------------------------------------
struct VSIn
{
	float4 p : POSITION;
	float4 c : COLOR;
	float2 t : TEXCOORD;
	float3 n : NORMAL;
	float3 t1 : TANGENT;
	float3 t2 : BINORMAL;
};
#endif