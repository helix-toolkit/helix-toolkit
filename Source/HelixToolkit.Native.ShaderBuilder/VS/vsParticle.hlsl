#ifndef VSPARTICLE_HLSL
#define VSPARTICLE_HLSL
#define PARTICLE
#include "..\Common\CommonBuffers.hlsl"
#include "..\Common\DataStructs.hlsl"

//--------------------------------------------------------------------------------
ParticleGS_INPUT main(in ParticleVS_INPUT input, in uint vertexid : SV_VertexID)
{
	ParticleGS_INPUT output;
	Particle p = SimulationState[vertexid];
    float4 pos = mul(float4(p.position, 1), mWorld);
    if (bHasInstances)
    {
        matrix mInstance =
        {
            input.mr0,
			input.mr1,
			input.mr2,
			input.mr3
        };
        pos = mul(pos, mInstance);
    }
    output.position = pos.xyz;
	output.energy = p.energy;
	output.color = p.color;
	output.initEnergy = p.initEnergy;
	output.texColRow = p.TexColRow;
    return output;
}

#endif