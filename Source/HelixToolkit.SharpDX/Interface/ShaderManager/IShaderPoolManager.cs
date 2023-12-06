using HelixToolkit.SharpDX.Shaders;

namespace HelixToolkit.SharpDX.ShaderManager;

/// <summary>
/// 
/// </summary>
public interface IShaderPoolManager : IDisposable
{
    /// <summary>
    /// Registers the shader. Shader object live time is managed by ShaderPoolManager. Shader should not be disposed manually.
    /// </summary>
    /// <param name="description">The description.</param>
    /// <returns></returns>
    ShaderBase? RegisterShader(ShaderDescription? description);

    /// <summary>
    /// Registers the input layout. Input layout object live time is managed by ShaderPoolManager. Input layout should not be disposed manually
    /// </summary>
    /// <param name="description">The description.</param>
    /// <returns></returns>
    InputLayoutProxy? RegisterInputLayout(InputLayoutDescription? description);
}
