#ifndef CSPARTICLEUPDATE_HLSL
#define CSPARTICLEUPDATE_HLSL
#define PARTICLE
#include"..\Common\CommonBuffers.hlsl"
#include"..\Common\DataStructs.hlsl"

bool PointInBoundingBox(in float3 boundMax, in float3 boundMin, in float3 p)
{
    return p.x < boundMax.x && p.x > boundMin.x && p.y < boundMax.y && p.y > boundMin.y && p.z < boundMax.z && p.z > boundMin.z;
}

[numthreads(512, 1, 1)]
void main(uint3 DispatchThreadID : SV_DispatchThreadID)
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
                float gravityDrag = ConsumerGravity / (distance * distance);
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
#endif