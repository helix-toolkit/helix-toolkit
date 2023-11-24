using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Shaders;

namespace CustomShaderDemo;

/// <summary>
/// Build using Nuget Micorsoft.HLSL.Microsoft.HLSL.CSharpVB automatically during project build
/// </summary>
public static class CustomPSShaderDescription
{
    public static readonly ShaderDescription PSDataSampling = new(
        nameof(PSDataSampling),
        ShaderStage.Pixel,
        new ShaderReflector(),
        ShaderHelper.LoadShaderCode(@"Shaders\psMeshDataSampling.cso"));

    public static readonly ShaderDescription PSNoiseMesh = new(
        nameof(PSNoiseMesh),
        ShaderStage.Pixel,
        new ShaderReflector(),
        ShaderHelper.LoadShaderCode(@"Shaders\psMeshNoiseBlinnPhong.cso"));

    public static readonly ShaderDescription PSCustomPoint = new(
        nameof(PSCustomPoint),
        ShaderStage.Pixel,
        new ShaderReflector(),
        ShaderHelper.LoadShaderCode(@"Shaders\psCustomPoint.cso"));
}
