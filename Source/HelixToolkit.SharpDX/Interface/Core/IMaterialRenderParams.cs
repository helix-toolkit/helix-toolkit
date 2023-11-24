using HelixToolkit.SharpDX.Model;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public interface IMaterialRenderParams
{
    /// <summary>
    /// Gets or sets the material variables used for rendering.
    /// </summary>
    /// <value>
    /// The material variable.
    /// </value>
    MaterialVariable? MaterialVariables
    {
        set; get;
    }
}
