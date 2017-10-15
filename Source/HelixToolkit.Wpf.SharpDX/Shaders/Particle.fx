#ifndef PARTICLE_FX
#define PARTICLE_FX

//--------------------------------------------------------------------------------
// PhongShading.hlsl
//
// This set of shaders implements the most basic phong shading.
//
// Copyright (C) 2010 Jason Zink.  All rights reserved.
//--------------------------------------------------------------------------------

//--------------------------------------------------------------------------------
// Resources
//--------------------------------------------------------------------------------


#include "./Shaders/Common.fx"
#include "./Shaders/Material.fx"

static const float scale = 0.5f;

static const float4 g_positions[4] =
{
	float4(-scale, scale, 0, 0),
	float4(scale, scale, 0, 0),
	float4(-scale, -scale, 0, 0),
	float4(scale, -scale, 0, 0),
};

static const float2 g_texcoords[4] =
{
	float2(0, 1),
	float2(1, 1),
	float2(0, 0),
	float2(1, 0),
};


struct Particle
{
	float3 position;
	float3 direction;
	float  time;
};

StructuredBuffer<Particle> SimulationState;

//--------------------------------------------------------------------------------
// Inter-stage structures
//--------------------------------------------------------------------------------
struct VS_INPUT_Particle
{
	uint vertexid			: SV_VertexID;
};
//--------------------------------------------------------------------------------
struct GS_INPUT_Particle
{
	float3 position			: Position;
};
//--------------------------------------------------------------------------------
struct PS_INPUT_Particle
{
	float4 position			: SV_Position;
	float2 texcoords		: TEXCOORD0;
	float4 color			: Color;
};
//--------------------------------------------------------------------------------
GS_INPUT_Particle VSParticle(in VS_INPUT_Particle input)
{
	GS_INPUT_Particle output;

	output.position.xyz = SimulationState[input.vertexid].position;

	return output;
}
//--------------------------------------------------------------------------------
[maxvertexcount(4)]
void GSParticle(point GS_INPUT_Particle input[1], inout TriangleStream<PS_INPUT_Particle> SpriteStream)
{
	PS_INPUT_Particle output;

	float dist = saturate(length(input[0].position - ConsumerLocation.xyz) / 100.0f);
	//float4 color = float4( 0.2f, 1.0f, 0.2f, 0.0f ) * dist + float4( 1.0f, 0.1f, 0.1f, 0.0f ) * ( 1.0f - dist ); 
	//float4 color = float4( 0.2f, 1.0f, 1.0f, 0.0f ) * dist + float4( 1.0f, 0.1f, 0.1f, 0.0f ) * ( 1.0f - dist ); 
	float4 color = float4(0.2f, 0.2f, 1.0f, 0.0f) * dist + float4(1.0f, 0.1f, 0.1f, 0.0f) * (1.0f - dist);

	// Transform to view space
	float4 viewposition = mul(float4(input[0].position, 1.0f), mWorld);

	// Emit two new triangles
	for (int i = 0; i < 4; i++)
	{
		// Transform to clip space
		output.position = mul(viewposition + g_positions[i], mProjection);
		output.texcoords = g_texcoords[i];
		output.color = color;

		SpriteStream.Append(output);
	}

	SpriteStream.RestartStrip();
}
//--------------------------------------------------------------------------------
float4 PSParticle(in PS_INPUT_Particle input) : SV_Target
{
	float4 color = texDiffuseMap.Sample(LinearSampler, input.texcoords);
	color = color * input.color;

	return(color);
}
//--------------------------------------------------------------------------------


AppendStructuredBuffer<Particle>	NewSimulationState : register(u0);
ConsumeStructuredBuffer<Particle>   CurrentSimulationState  : register(u1);

cbuffer SimulationParameters
{
	float4 TimeFactors;
	float4 EmitterLocation;
	float4 ConsumerLocation;
};

cbuffer ParticleCount
{
	uint4 NumParticles;
};

static const float G = 10.0f;
static const float m1 = 10.0f;
static const float m2 = 500.0f;
static const float m1m2 = m1 * m2;
static const float eventHorizon = 5.0f;

[numthreads(512, 1, 1)]
void CSParticle(uint3 DispatchThreadID : SV_DispatchThreadID)
{
	// Check for if this thread should run or not.
	uint myID = DispatchThreadID.x + DispatchThreadID.y * 512 + DispatchThreadID.z * 512 * 512;

	if (myID < NumParticles.x)
	{
		// Get the current particle
		Particle p = CurrentSimulationState.Consume();

		// Calculate the current gravitational force applied to it
		float3 d = ConsumerLocation.xyz - p.position;
		float r = length(d);
		float3 Force = (G * m1m2 / (r*r)) * normalize(d);

		// Calculate the new velocity, accounting for the acceleration from
		// the gravitational force over the current time step.
		p.velocity = p.velocity + (Force / m1) * TimeFactors.x;

		// Calculate the new position, accounting for the new velocity value
		// over the current time step.
		p.position += p.velocity * TimeFactors.x;

		// Update the life time left for the particle.
		p.time = p.time + TimeFactors.x;

		// Test to see how close the particle is to the black hole, and 
		// don't pass it to the output list if it is too close.
		if (r > eventHorizon)
		{
			if (p.time < 30.0f)
			{
				NewSimulationState.Append(p);
			}
		}
	}
}

cbuffer ParticleInsertParameters
{
	float4 EmitterLocation;
	float4 RandomVector;
};

static const float3 direction[8] =
{
	normalize(float3(1.0f,  1.0f,  1.0f)),
	normalize(float3(-1.0f,  1.0f,  1.0f)),
	normalize(float3(-1.0f, -1.0f,  1.0f)),
	normalize(float3(1.0f, -1.0f,  1.0f)),
	normalize(float3(1.0f,  1.0f, -1.0f)),
	normalize(float3(-1.0f,  1.0f, -1.0f)),
	normalize(float3(-1.0f, -1.0f, -1.0f)),
	normalize(float3(1.0f, -1.0f, -1.0f))
};


[numthreads(8, 1, 1)]
void CSParticleInitialize(uint3 GroupThreadID : SV_GroupThreadID)
{
	Particle p;

	// Initialize position to the current emitter location
	p.position = EmitterLocation.xyz;

	// Initialize direction to a randomly reflected vector
	p.direction = reflect(direction[GroupThreadID.x], RandomVector.xyz) * 5.0f;

	// Initialize the lifetime of the particle in seconds
	p.time = 0.0f;

	// Append the new particle to the output buffer
	CurrentSimulationState.Append(p);
}
#endif