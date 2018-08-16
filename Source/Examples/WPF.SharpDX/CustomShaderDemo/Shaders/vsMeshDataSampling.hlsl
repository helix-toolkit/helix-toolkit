#ifndef VSMESHDEFAULT_HLSL
#define VSMESHDEFAULT_HLSL
#define MESH
#include"Common.hlsl"
#include"DataStructs.hlsl"
#pragma pack_matrix( row_major )

PSInput main(VSInput input)
{
    PSInput output = (PSInput)0;
    float4 inputp = input.p;
    float3 inputn = input.n;
    float3 inputt1 = input.t1;
    float3 inputt2 = input.t2;

    //set position into world space	
    output.p = mul(inputp, mWorld);

    //set normal for interpolation
    output.n = normalize(mul(inputn, (float3x3) mWorld));

    output.p.xyz += input.t.x* vParams.y * output.n;

    output.wp = output.p;
    //set position into clip space	
    output.p = mul(output.p, mViewProjection);
    output.vEye = output.wp - float4(vEyePos, 1);
    //set texture coords and color
    output.t = input.t;
    output.c = input.c;
    output.cDiffuse = vMaterialDiffuse;
    output.c2 = vMaterialEmissive + vMaterialAmbient * vLightAmbient;

    return output;
}

#endif