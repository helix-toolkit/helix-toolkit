#ifndef VSMESHBATCHEDSHADOW_HLSL
#define VSMESHBATCHEDSHADOW_HLSL

#define MESH
#include"..\Common\Common.hlsl"
#include"..\Common\DataStructs.hlsl"
#pragma pack_matrix( row_major )

PSShadow main(VSInputBatched input)
{
    PSShadow output = (PSShadow) 0;
    output.p = input.p;
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
   //     output.p = mul(input.p, mInstance);
   // }

	//set position into world space	
    output.p = mul(output.p, mul(mWorld, vLightViewProjection));
    return output;
}
#endif