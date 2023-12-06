using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Shaders;

namespace CustomShaderDemo;

/// <summary>
/// Build using Nuget Micorsoft.HLSL.Microsoft.HLSL.CSharpVB automatically during project build
/// </summary>
public static class CustomVSShaderDescription
{
    public static byte[] VSMeshDataSamplerByteCode
    {
        get
        {
            return ShaderHelper.LoadShaderCode(@"Shaders\vsMeshDataSampling.cso");
        }
    }

    public static readonly ShaderDescription VSDataSampling = new(
        nameof(VSDataSampling),
        ShaderStage.Vertex,
        new ShaderReflector(),
        VSMeshDataSamplerByteCode);
}
