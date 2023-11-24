namespace HelixToolkit.SharpDX.Shaders;

public static class DefaultHullShaderDescriptions
{
    public static readonly ShaderDescription HSMeshTessellation = new(nameof(HSMeshTessellation), ShaderStage.Hull, new ShaderReflector(),
        DefaultHullShaders.HSMeshTessellation);
}
