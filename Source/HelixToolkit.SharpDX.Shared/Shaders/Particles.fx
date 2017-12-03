//--------------------------------------------------------------------------------------
// File: Particle Functions for HelixToolkitDX
// Author: Lunci Hua
// Date: 05/10/17
// Reference: https://github.com/spazzarama/Direct3D-Rendering-Cookbook
// Copyright (c) 2017 Helix Toolkit contributors

#ifndef PARTICALES_FX
#define PARTICALES_FX
#include "Common.fx"
#include "Material.fx"

static const float scale = 0.5f;

static const float4 g_positions[4] =
{
	float4(scale, scale, 0, 0),
    float4(-scale, scale, 0, 0),
	float4(scale, -scale, 0, 0),
    float4(-scale, -scale, 0, 0),
};

static const float2 g_texcoords[4] =
{
	float2(1, 0),
    float2(0, 0),
    float2(1, 1),
    float2(0, 1),
};

struct Particle
{
	float3 position;
	float initEnergy;
    float3 velocity;
	float energy;	
	float4 color;
	float3 initAccelleration;
	float dissipRate;
	uint2 TexColRow;
};

cbuffer ParticleRandoms
{
	uint RandomSeed;
	float3 RandomVector;
	float2 ParticleSize;
	uint NumTexCol;
	uint NumTexRow;
	bool AnimateByEnergyLevel;
};

cbuffer ParticleFrame : register(b1)
{
	uint NumParticles;
	float3 ExtraAccelation;

    float TimeFactors; 
	float3 DomainBoundsMax;

	float3 DomainBoundsMin;         
	uint CumulateAtBound;

	float3 ConsumerLocation;
	float ConsumerGravity;

	float ConsumerRadius;
	float3 Pad;
};


ConsumeStructuredBuffer<Particle> CurrentSimulationState : register(u0);
AppendStructuredBuffer<Particle> NewSimulationState : register(u1);

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

bool PointInBoundingBox(in float3 boundMax, in float3 boundMin, in float3 p)
{
	return p.x < boundMax.x && p.x > boundMin.x && p.y < boundMax.y && p.y > boundMin.y && p.z < boundMax.z && p.z > boundMin.z;
}

cbuffer ParticleCreateParameters : register(b1)
{
	float3 EmitterLocation;
	float InitialEnergy;

	float EmitterRadius;
	float2 Pad1;
	float InitialVelocity;

	float4 ParticleBlendColor;

	float EnergyDissipationRate; //Energy dissipation rate per second
	float3 InitialAcceleration;
};

uint rand_lcg(inout uint rng_state)
{
    // LCG values from Numerical Recipes
	rng_state = 1664525 * rng_state + 1013904223;
	return rng_state;
}
uint wang_hash(uint seed)
{
	seed = (seed ^ 61) ^ (seed >> 16);
	seed *= 9;
	seed = seed ^ (seed >> 4);
	seed *= 0x27d4eb2d;
	seed = seed ^ (seed >> 15);
	return seed;
}

[numthreads(8, 1, 1)]
void ParticleInsertCSMAIN(uint3 GroupThreadID : SV_GroupThreadID)
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
	else if(NumTexCol > 1 || NumTexRow > 1)
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


[numthreads(512, 1, 1)]
void ParticleUpdateCSMAIN(uint3 DispatchThreadID : SV_DispatchThreadID)
{
	// Check for if this thread should run or not.
    uint myID = DispatchThreadID.x + DispatchThreadID.y * 512 + DispatchThreadID.z * 512 * 512;

    if (myID < NumParticles)
    {
		// Get the current particle
        Particle p = CurrentSimulationState.Consume();

		// Calculate the new velocity, accounting for the acceleration from
		// the gravitational force over the current time step.
		if (ConsumerGravity < 1e-7)
		{
			p.velocity += (p.initAccelleration + ExtraAccelation) * TimeFactors;
		}
		else
		{
			float distance = length(ConsumerLocation - p.position);
			if (distance > ConsumerRadius)
			{
				float gravityDrag = ConsumerGravity / (distance*distance);
				p.velocity += (p.initAccelleration + ExtraAccelation + normalize(ConsumerLocation - p.position) * gravityDrag) * TimeFactors;			
			}
			else
			{
				return;
			}
		}

		// Calculate the new position, accounting for the new velocity value
		// over the current time step.
		float3 pnew = p.position + p.velocity * TimeFactors;
		bool inBound = PointInBoundingBox(DomainBoundsMax, DomainBoundsMin, pnew);

		p.position = inBound ? pnew : p.position;
		// Update the life time left for the particle.
		p.energy -= TimeFactors * p.dissipRate;

		// Test to see how close the particle is to the black hole, and 
		// don't pass it to the output list if it is too close.
		if (p.energy > 0 && (inBound || CumulateAtBound != 0))
        {
            NewSimulationState.Append(p);
        }
    }
}

//--------------------------------------------------------------------------------
// Inter-stage structures
//--------------------------------------------------------------------------------
struct ParticleVS_INPUT
{
    uint vertexid : SV_VertexID;
};
//--------------------------------------------------------------------------------
struct ParticleGS_INPUT
{
    float3 position : Position;
	float energy : Energy;
	float4 color : COLOR0;
	float initEnergy : Energy1;
	uint2 texColRow : TexOff;
};
//--------------------------------------------------------------------------------
struct ParticlePS_INPUT
{
    float4 position : SV_Position;
	noperspective
	float4 color : COLOR0;
    float2 texcoords : TEXCOORD0;
    float opacity : OPACITY0;
	float pad0 : PAD;
};

StructuredBuffer<Particle> SimulationState;

//--------------------------------------------------------------------------------
ParticleGS_INPUT ParticleVSMAIN(in ParticleVS_INPUT input)
{
	ParticleGS_INPUT output;
	Particle p = SimulationState[input.vertexid];
    output.position.xyz = p.position;
	output.energy = p.energy;
	output.color = p.color;
	output.initEnergy = p.initEnergy;
	output.texColRow = p.TexColRow;
    return output;
}

static float2 one = float2(1, 1);


//--------------------------------------------------------------------------------
[maxvertexcount(4)]
void ParticleGSMAIN(point ParticleGS_INPUT input[1], inout TriangleStream<ParticlePS_INPUT> SpriteStream)
{
    ParticlePS_INPUT output;
	float opacity = saturate(input[0].energy / input[0].initEnergy);

	//// Transform to view space
    float4 viewposition = mul(mul(float4(input[0].position, 1.0f), mWorld), mView);
	float2 texScale = float2(1.0f / max(1, NumTexCol), 1.0f / max(1, NumTexRow));
	if (AnimateByEnergyLevel)
	{
		float colrow = floor((1 - opacity) * (NumTexCol * NumTexRow - 1));
		float column = colrow % NumTexCol;
		float row = floor(colrow / NumTexCol);
	
		// Emit two new triangles
		for (int i = 0; i < 4; i++)
		{
			// Transform to clip space

			output.position = mul(viewposition + g_positions[i] * float4(ParticleSize, 0, 0), mProjection);
			output.texcoords = (g_texcoords[i] + float2(column, row)) * texScale;
			output.opacity = opacity;
			output.color = input[0].color;
			output.pad0 = 0;
			SpriteStream.Append(output);
		}		
	}
	else
	{
				// Emit two new triangles
		for (int i = 0; i < 4; i++)
		{
			// Transform to clip space

			output.position = mul(viewposition + g_positions[i] * float4(ParticleSize, 0, 0), mProjection);
			output.texcoords = (g_texcoords[i] + input[0].texColRow) * texScale;
			output.opacity = opacity;
			output.color = input[0].color;
			output.pad0 = 0;
			SpriteStream.Append(output);
		}
	}


    SpriteStream.RestartStrip();
}
//--------------------------------------------------------------------------------
float4 ParticlePSMAIN(in ParticlePS_INPUT input) : SV_Target
{
	float4 color = input.color * input.opacity;
    if (bHasDiffuseMap)
    {
        color *= texDiffuseMap.Sample(LinearSampler, input.texcoords);        
    }
    return color;
}
//--------------------------------------------------------------------------------

#endif