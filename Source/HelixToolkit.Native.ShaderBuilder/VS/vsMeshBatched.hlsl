#ifndef VSMESHBATCHED_HLSL
#define VSMESHBATCHED_HLSL

#define MESH
#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"
#pragma pack_matrix( row_major )

PSInput main(VSInputBatched input)
{
    PSInput output = (PSInput) 0;
    float4 inputp = input.p;
    float3 inputn = input.n;
    float3 inputt1 = input.t1;
    float3 inputt2 = input.t2;
    if (bInvertNormal)
    {
        inputn = -inputn;
    }

	// compose instance matrix
   // if (bHasInstances)
   // {
   //     matrix mInstance =
   //     {
   //         input.mr0,
			//input.mr1,
			//input.mr2,
			//input.mr3
   //     };
   //     inputp = mul(input.p, mInstance);
   //     inputn = mul(inputn, (float3x3) mInstance);
   //     if (bHasNormalMap)
   //     {
   //         inputt1 = mul(inputt1, (float3x3) mInstance);
   //         inputt2 = mul(inputt2, (float3x3) mInstance);
   //     }
   // }

	//set position into world space	
    output.p = mul(inputp, mWorld);
    float3 vEye = vEyePos - output.p.xyz;
    output.vEye = float4(normalize(vEye), length(vEye)); //Use wp for camera->vertex direction
	//set normal for interpolation	
    output.n = normalize(mul(inputn, (float3x3) mWorld));
    	//set texture coords
    output.t = mul(float2x4(uvTransformR1, uvTransformR2), float4(input.t, 0, 1)).xy;
    if (bHasDisplacementMap)
    {
        const float mipInterval = 20;
        float mipLevel = clamp((distance(output.p.xyz, vEyePos) - mipInterval) / mipInterval, 0, 6);
        float3 h = texDisplacementMap.SampleLevel(samplerDisplace, output.t, mipLevel);
        output.p.xyz += output.n * mul(h, displacementMapScaleMask.xyz);
    }
    output.wp = output.p;
	//set position into clip space	
    output.p = mul(output.p, mViewProjection);

	//set position into light-clip space
    if (bHasShadowMap)
    {
        output.sp = mul(output.wp, vLightViewProjection);
    }

	//set color
    output.c = input.c;
    if (!bRenderPBR)
    {
        output.cDiffuse = FloatToRGB(input.c.x);
        output.cDiffuse.a = input.c1.z;
        output.c2 = mad(FloatToRGB(input.c1.x), vLightAmbient, FloatToRGB(input.c.y));
        output.c.x = input.c1.y; // switch element to shininess
    }
    else
    {
        output.cDiffuse = input.c;
        output.c2 = input.c1;
    }

    if (bHasNormalMap)
    {
        if (!bAutoTengent)
        {
		    // transform the tangents by the world matrix and normalize
            output.t1 = normalize(mul(inputt1, (float3x3) mWorld));
            output.t2 = normalize(mul(inputt2, (float3x3) mWorld));
        }
    }
    return output;
}

#endif