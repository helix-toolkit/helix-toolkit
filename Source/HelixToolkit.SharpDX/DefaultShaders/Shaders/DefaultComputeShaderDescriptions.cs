namespace HelixToolkit.SharpDX.Shaders;

public static class DefaultComputeShaderDescriptions
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription CSParticleInsert = new(nameof(CSParticleInsert), ShaderStage.Compute, new ShaderReflector(),
        DefaultComputeShaders.CSParticleInsert);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription CSParticleUpdate = new(nameof(CSParticleUpdate), ShaderStage.Compute, new ShaderReflector(),
        DefaultComputeShaders.CSParticleUpdate);
}
