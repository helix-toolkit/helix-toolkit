#ifndef VSPARTICLE_HLSL
#define VSPARTICLE_HLSL
#define PARTICLE
#include"..\Common\CommonBuffers.hlsl"
#include"..\Common\DataStructs.hlsl"

//--------------------------------------------------------------------------------
ParticleGS_INPUT main(in ParticleVS_INPUT input, in uint vertexid : SV_VertexID)
{
	ParticleGS_INPUT output;
	Particle p = SimulationState[vertexid];
    float4 pos = float4(p.position, 1);
    if (bHasInstances)
    {
        matrix mInstance =
        {
            input.mr0.x, input.mr1.x, input.mr2.x, input.mr3.x, // row 1
			input.mr0.y, input.mr1.y, input.mr2.y, input.mr3.y, // row 2
			input.mr0.z, input.mr1.z, input.mr2.z, input.mr3.z, // row 3
			input.mr0.w, input.mr1.w, input.mr2.w, input.mr3.w, // row 4
        };
        pos = mul(mInstance, pos);
    }
    output.position = pos.xyz;
	output.energy = p.energy;
	output.color = p.color;
	output.initEnergy = p.initEnergy;
	output.texColRow = p.TexColRow;
    return output;
}

#endif