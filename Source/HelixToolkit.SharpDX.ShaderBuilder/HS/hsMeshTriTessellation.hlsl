#ifndef hsMESHTriTESSELLATION_HLSL
#define hsMESHTriTESSELLATION_HLSL

#define MESH
//#include"..\Common\CommonBuffers.hlsl"
#include"..\Common\DataStructs.hlsl"
#pragma pack_matrix( row_major )

//--------------------------------------------------------------------------------------
// Work-around for an optimization rule problem in the June 2010 HLSL Compiler
// (9.29.952.3111)
// see http://support.microsoft.com/kb/2448404
//--------------------------------------------------------------------------------------
#if D3DX_VERSION == 0xa2b
#pragma ruledisable 0x0802405f
#endif
//--------------------------------------------------------------------------------------
// HULL SHADER constant function for triangular patches
// called per patch
//--------------------------------------------------------------------------------------
HSConstantDataOutput HShaderTriConstant(InputPatch<HSInput, 3> inputPatch)
{
    HSConstantDataOutput output = (HSConstantDataOutput) 0;

	// edge control points
    output.f3B210 = 1;
    output.f3B120 = 1;
    output.f3B021 = 1;
    output.f3B012 = 1;
    output.f3B102 = 1;
    output.f3B201 = 1;

    // center control point
    output.f3B111 = 1;

	// tessellation factors
	
    output.Edges[0] = 0.5 * (inputPatch[1].tessF + inputPatch[2].tessF);
    output.Edges[1] = 0.5 * (inputPatch[2].tessF + inputPatch[0].tessF);
    output.Edges[2] = 0.5 * (inputPatch[0].tessF + inputPatch[1].tessF);
    
    output.Inside = output.Edges[0];
  //  (output.Edges[0] + output.Edges[1] + output.Edges[2]) / 3;


	// edge control points
    output.f3B210 = ((2.0f * inputPatch[0].p) + inputPatch[1].p - (dot((inputPatch[1].p - inputPatch[0].p), inputPatch[0].n) * inputPatch[0].n)) / 3.0f;
    output.f3B120 = ((2.0f * inputPatch[1].p) + inputPatch[0].p - (dot((inputPatch[0].p - inputPatch[1].p), inputPatch[1].n) * inputPatch[1].n)) / 3.0f;
    output.f3B021 = ((2.0f * inputPatch[1].p) + inputPatch[2].p - (dot((inputPatch[2].p - inputPatch[1].p), inputPatch[1].n) * inputPatch[1].n)) / 3.0f;
    output.f3B012 = ((2.0f * inputPatch[2].p) + inputPatch[1].p - (dot((inputPatch[1].p - inputPatch[2].p), inputPatch[2].n) * inputPatch[2].n)) / 3.0f;
    output.f3B102 = ((2.0f * inputPatch[2].p) + inputPatch[0].p - (dot((inputPatch[0].p - inputPatch[2].p), inputPatch[2].n) * inputPatch[2].n)) / 3.0f;
    output.f3B201 = ((2.0f * inputPatch[0].p) + inputPatch[2].p - (dot((inputPatch[2].p - inputPatch[0].p), inputPatch[0].n) * inputPatch[0].n)) / 3.0f;
    // center control point
    float3 f3E = (output.f3B210 + output.f3B120 + output.f3B021 + output.f3B012 + output.f3B102 + output.f3B201) / 6.0f;
    float3 f3V = (inputPatch[0].p + inputPatch[1].p + inputPatch[2].p) / 3.0f;
    output.f3B111 = f3E + ((f3E - f3V) / 2.0f);

	// -- culling in HS
    float2 t01 = inputPatch[1].t - inputPatch[0].t;
    float2 t02 = inputPatch[2].t - inputPatch[0].t;
    output.Sign = t01.x * t02.y - t01.y * t02.x > 0.0f ? 1 : -1;

    return output;
}


//--------------------------------------------------------------------------------------
// HULL SHADER main function
// called per output-control point
//--------------------------------------------------------------------------------------
[domain("tri")]
[partitioning("integer")]
[outputtopology("triangle_cw")]
[outputcontrolpoints(3)]
[patchconstantfunc("HShaderTriConstant")]
[maxtessfactor(64.0)]
HSInput main(InputPatch<HSInput, 3> inputPatch, uint cpID : SV_OutputControlPointID, uint patchID : SV_PrimitiveID)
{
    return inputPatch[cpID];
}
#endif