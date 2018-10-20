#ifndef dsMESHTriTESSELLATION_HLSL
#define dsMESHTriTESSELLATION_HLSL

#define MESH
#include"..\Common\CommonBuffers.hlsl"
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
// DOMAIN SHADER function for triangle-patches
// called per uv-coordinate/ouput vertex
//--------------------------------------------------------------------------------------
[domain("tri")]
PSInput main(HSConstantDataOutput input, float3 barycentricCoords : SV_DomainLocation, OutputPatch<HSInput, 3> inputPatch)
{
    PSInput output = (PSInput) 0;

    // --- The barycentric coordinates
    float fU = barycentricCoords.x;
    float fV = barycentricCoords.y;
    float fW = barycentricCoords.z;

    // --- Precompute squares and squares * 3 
    float fUU = fU * fU;
    float fVV = fV * fV;
    float fWW = fW * fW;
    float fUU3 = fUU * 3.0f;
    float fVV3 = fVV * 3.0f;
    float fWW3 = fWW * 3.0f;

	// --- Compute position from cubic control points and barycentric coords
    float3 position = inputPatch[0].p * fWW * fW +
						inputPatch[1].p * fUU * fU +
						inputPatch[2].p * fVV * fV +
						input.f3B210 * fWW3 * fU +
						input.f3B120 * fW * fUU3 +
						input.f3B201 * fWW3 * fV +
						input.f3B021 * fUU3 * fV +
						input.f3B102 * fW * fVV3 +
						input.f3B012 * fU * fVV3 +
						input.f3B111 * 6.0f * fW * fU * fV;

	// Compute normal from barycentric coords
    output.n = normalize(inputPatch[0].n * barycentricCoords.z + inputPatch[1].n * barycentricCoords.x + inputPatch[2].n * barycentricCoords.y);
		
	// --- interpolate texture coordinates
    output.t = inputPatch[0].t * barycentricCoords.z + inputPatch[1].t * barycentricCoords.x + inputPatch[2].t * barycentricCoords.y;
	
	// ---  interpolated per-vertex colors
    output.c = inputPatch[0].c * barycentricCoords.z + inputPatch[1].c * barycentricCoords.x + inputPatch[2].c * barycentricCoords.y;
    output.c2 = inputPatch[0].c2;
    output.cDiffuse = vMaterialDiffuse;
	// --- Classical vertex-shader transforms: 
	// --- output position in the clip-space	
    output.p = float4(position, 1); //mul(float4(position, 1.0f), mWorld);
    float3 vEye = vEyePos - output.p.xyz;
    output.vEye = float4(normalize(vEye), length(vEye)); //Use wp for camera->vertex direction
    if (bHasDisplacementMap)
    {
        const float mipInterval = 20;
        float mipLevel = clamp((distance(output.p.xyz, vEyePos) - mipInterval) / mipInterval, 0, 6);
        float3 h = texDisplacementMap.SampleLevel(samplerDisplace, output.t, mipLevel);
        output.p.xyz += output.n * mul(h, displacementMapScaleMask.xyz);
    }

    output.wp = output.p;
    if (bHasShadowMap)
    {
        output.sp = mul(output.wp, vLightViewProjection);
    }
    output.p = mul(output.p, mView);
    output.p = mul(output.p, mProjection);

	// --- interpolated normals    
    output.n = normalize(mul(output.n, (float3x3) mWorld));

    if (bHasNormalMap)
    {
        if (!bAutoTengent)
        {
			    // Compute tangent-space
            output.t1 = normalize(inputPatch[0].t1 * barycentricCoords.z + inputPatch[1].t1 * barycentricCoords.x + inputPatch[2].t1 * barycentricCoords.y);
            output.t2 = normalize(inputPatch[0].t2 * barycentricCoords.z + inputPatch[1].t2 * barycentricCoords.x + inputPatch[2].t2 * barycentricCoords.y);
		    // transform the tangents by the world matrix and normalize
            output.t1 = normalize(mul(output.t1, (float3x3) mWorld));
            output.t2 = normalize(mul(output.t2, (float3x3) mWorld));
        }
    }
    return output;
}
#endif