#ifndef GSPARTICLE_HLSL
#define GSPARTICLE_HLSL
#define PARTICLE
#include"..\Common\CommonBuffers.hlsl"
#include"..\Common\DataStructs.hlsl"
static float2 one = float2(1, 1);
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

//--------------------------------------------------------------------------------
[maxvertexcount(4)]
void main(point ParticleGS_INPUT input[1], inout TriangleStream<ParticlePS_INPUT> SpriteStream)
{
	ParticlePS_INPUT output;
	float opacity = saturate(input[0].energy / input[0].initEnergy);
    float3 vEye = vEyePos - input[0].position.xyz;
    float z = length(vEye); //Use wp for camera->vertex direction
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
			output.z = z;
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
			output.z = z;
			SpriteStream.Append(output);
		}
	}


	SpriteStream.RestartStrip();
}

#endif