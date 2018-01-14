#ifndef VSMESHXRAY_HLSL
#define VSMESHXRAY_HLSL
#define MATERIAL
#define MESH
#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"

PSInputXRay main(VSInput input)
{
    PSInputXRay output = (PSInputXRay) 0;
    float4 inputp = input.p;
    float3 inputn = input.n;
    if (bInvertNormal)
    {
        inputn = -inputn;
    }
	// compose instance matrix
    if (bHasInstances)
    {
        matrix mInstance =
        {
            input.mr0,
			input.mr1,
			input.mr2,
			input.mr3
        };
        inputp = mul(input.p, mInstance);
        inputn = mul(inputn, (float3x3) mInstance);
    }

	//set position into camera clip space	
    output.p = mul(inputp, mWorld);
    output.vEye = float4(normalize(vEyePos - output.p.xyz), 1); //Use wp for camera->vertex direction
    output.p = mul(output.p, mViewProjection);

    	//set normal for interpolation	
    output.n = normalize(mul(inputn, (float3x3) mWorld));
    return output;
}

#endif