#ifndef CSPARTICLEINSERT_HLSL
#define CSPARTICLEINSERT_HLSL
#define PARTICLE
#include"..\Common\CommonBuffers.hlsl"
#include"..\Common\DataStructs.hlsl"

uint wang_hash(uint seed)
{
    seed = (seed ^ 61) ^ (seed >> 16);
    seed *= 9;
    seed = seed ^ (seed >> 4);
    seed *= 0x27d4eb2d;
    seed = seed ^ (seed >> 15);
    return seed;
}

uint rand_lcg(inout uint rng_state)
{
    // LCG values from Numerical Recipes
    rng_state = 1664525 * rng_state + 1013904223;
    return rng_state;
}

static const float3 direction[8] =
{
    normalize(float3(1.0f, 1.0f, 1.0f)),
	normalize(float3(-1.0f, 1.0f, 1.0f)),
	normalize(float3(-1.0f, -1.0f, 1.0f)),
	normalize(float3(1.0f, -1.0f, 1.0f)),
	normalize(float3(1.0f, 1.0f, -1.0f)),
	normalize(float3(-1.0f, 1.0f, -1.0f)),
	normalize(float3(-1.0f, -1.0f, -1.0f)),
	normalize(float3(1.0f, -1.0f, -1.0f))
};

[numthreads(8, 1, 1)]
void main(uint3 GroupThreadID : SV_GroupThreadID)
{
    Particle p;
    uint state = wang_hash(RandomSeed + GroupThreadID.x);
    float f0 = float(rand_lcg(state)) * (1.0 / 4294967296.0);
    float f1 = float(rand_lcg(state)) * (1.0 / 4294967296.0);
    float f2 = float(rand_lcg(state)) * (1.0 / 4294967296.0);

    float3 dir = direction[GroupThreadID.x];
	// Initialize position to the current emitter location
    p.position = EmitterLocation + dir * float3(f0 * EmitterRadius, f1 * EmitterRadius, f2 * EmitterRadius);

	// Initialize direction to a randomly reflected vector
    p.velocity = normalize(reflect(dir, RandomVector)) * InitialVelocity;

	// Initialize the lifetime of the particle in seconds
    p.energy = InitialEnergy;

    p.initEnergy = InitialEnergy;

    p.color = ParticleBlendColor;

    p.dissipRate = EnergyDissipationRate;

    p.initAccelleration = InitialAcceleration;

    if (AnimateByEnergyLevel)
    {
        p.TexColRow = uint2(0, 0);
    }
    else if (NumTexCol > 1 || NumTexRow > 1)
    {
        uint rndNumber1 = rand_lcg(state);
        uint rndNumber2 = rand_lcg(state);
        p.TexColRow = uint2(rndNumber1 % max(1, NumTexCol) - 1, rndNumber2 % max(1, NumTexRow) - 1);
    }
    else
    {
        p.TexColRow = uint2(0, 0);
    }
	// Append the new particle to the output buffer
    NewSimulationState.Append(p);
}

#endif