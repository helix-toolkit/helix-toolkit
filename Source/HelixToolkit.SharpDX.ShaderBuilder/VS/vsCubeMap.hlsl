#ifndef VSCUBMAP_HLSL
#define VSCUBMAP_HLSL
#define MESH
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

PSInputCube main(float4 p : POSITION)
{
	PSInputCube output = (PSInputCube) 0;

	//set position into clip space		
	output.p = mul(p, mWorld);
	output.p = mul(output.p, mView);
	output.p = mul(output.p, mProjection).xyww;

	//set texture coords and color
	//output.t = input.t;	
	//output.c = p;

	//Set Pos to xyww instead of xyzw, so that z will always be 1 (furthest from camera)	
	output.t = p.xyz;

	return output;
}

#endif