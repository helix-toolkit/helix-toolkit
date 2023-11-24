namespace HelixToolkit.SharpDX.Shaders;

/// <summary>
/// Default Geometry Shaders
/// </summary>
public static class DefaultGSShaderDescriptions
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription GSPoint = new(nameof(GSPoint), ShaderStage.Geometry, new ShaderReflector(),
        DefaultGSShaderByteCodes.GSPoint);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription GSLine = new(nameof(GSLine), ShaderStage.Geometry, new ShaderReflector(),
        DefaultGSShaderByteCodes.GSLine);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription GSLineArrowHead = new(nameof(GSLineArrowHead), ShaderStage.Geometry, new ShaderReflector(),
        DefaultGSShaderByteCodes.GSLineArrowHead);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription GSLineArrowHeadTail = new(nameof(GSLineArrowHeadTail), ShaderStage.Geometry, new ShaderReflector(),
        DefaultGSShaderByteCodes.GSLineArrowHeadTail);
    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription GSBillboard = new(nameof(GSBillboard), ShaderStage.Geometry, new ShaderReflector(),
        DefaultGSShaderByteCodes.GSBillboard);

    /// <summary>
    /// 
    /// </summary>
    public static readonly ShaderDescription GSParticle = new(nameof(GSParticle), ShaderStage.Geometry, new ShaderReflector(),
        DefaultGSShaderByteCodes.GSParticle);
    /// <summary>
    /// The gs mesh normal vector
    /// </summary>
    public static readonly ShaderDescription GSMeshNormalVector = new(nameof(GSMeshNormalVector), ShaderStage.Geometry, new ShaderReflector(),
        DefaultGSShaderByteCodes.GSMeshNormalVector);

    /// <summary>
    /// The gs mesh bone skinned out
    /// </summary>
    public static readonly ShaderDescription GSMeshBoneSkinnedOut = new(nameof(GSMeshBoneSkinnedOut), ShaderStage.Geometry, new ShaderReflector(),
        DefaultGSShaderByteCodes.GSMeshBoneSkinnedOut)
    {
        IsGSStreamOut = true,
        GSSOElement = new global::SharpDX.Direct3D11.StreamOutputElement[]
        {
                    new global::SharpDX.Direct3D11.StreamOutputElement(0, "POSITION", 0, 0, 4, 0),
                    new global::SharpDX.Direct3D11.StreamOutputElement(0, "NORMAL", 0, 0, 3, 0),
                    new global::SharpDX.Direct3D11.StreamOutputElement(0, "TANGENT", 0, 0, 3, 0),
                    new global::SharpDX.Direct3D11.StreamOutputElement(0, "BINORMAL", 0, 0, 3, 0),
        },
        GSSOStrides = new int[]
        {
                    DefaultVertex.SizeInBytes
        }
    };
}
