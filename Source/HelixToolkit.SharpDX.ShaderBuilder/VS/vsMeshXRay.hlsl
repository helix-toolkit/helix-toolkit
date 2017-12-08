#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Material.hlsl"

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
            input.mr0.x, input.mr1.x, input.mr2.x, input.mr3.x, // row 1
			input.mr0.y, input.mr1.y, input.mr2.y, input.mr3.y, // row 2
			input.mr0.z, input.mr1.z, input.mr2.z, input.mr3.z, // row 3
			input.mr0.w, input.mr1.w, input.mr2.w, input.mr3.w, // row 4
        };
        inputp = mul(mInstance, input.p);
        inputn = mul((float3x3) mInstance, inputn);
    }

	//set position into camera clip space	
    output.p = mul(inputp, mWorld);
    output.vEye = float4(normalize(vEyePos - output.p.xyz), 1); //Use wp for camera->vertex direction
    output.p = mul(output.p, mViewProjection);

    	//set normal for interpolation	
    output.n = normalize(mul(inputn, (float3x3) mWorld));
    return output;
}
