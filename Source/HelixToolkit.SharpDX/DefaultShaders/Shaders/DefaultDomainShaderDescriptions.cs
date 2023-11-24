namespace HelixToolkit.SharpDX.Shaders;

public static class DefaultDomainShaderDescriptions
{
    public static readonly ShaderDescription DSMeshTessellation = new(nameof(DSMeshTessellation), ShaderStage.Domain, new ShaderReflector(),
        DefaultDomainShaders.DSMeshTessellation);
}
