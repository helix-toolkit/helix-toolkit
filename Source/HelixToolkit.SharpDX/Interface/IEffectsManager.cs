using HelixToolkit.SharpDX.ShaderManager;
using HelixToolkit.SharpDX.Shaders;
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public interface IEffectsManager : IDeviceResources
{
    /// <summary>
    /// 
    /// </summary>
    IShaderPoolManager? ShaderManager
    {
        get;
    }

    IStructArrayPool? StructArrayPool
    {
        get;
    }
    /// <summary>
    /// Get list of existing technique names
    /// </summary>
    IEnumerable<string> RenderTechniques
    {
        get;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IRenderTechnique? GetTechnique(string name);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IRenderTechnique? this[string name] { get; }
    /// <summary>
    /// Reinitializes all resources after calling <see cref="DisposeAllResources"/>.
    /// </summary>
    void Reinitialize();
    /// <summary>
    /// Disposes all resources. This is used to handle such as DeviceLost or DeviceRemoved Error
    /// </summary>
    void DisposeAllResources();
    /// <summary>
    /// Add a technique by description
    /// </summary>
    /// <param name="description"></param>
    void AddTechnique(TechniqueDescription description);
    /// <summary>
    /// Remove a technique by its name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool RemoveTechnique(string name);
    /// <summary>
    /// Removes all techniques.
    /// </summary>
    void RemoveAllTechniques();
    /// <summary>
    /// Determines whether the specified name has technique.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>
    ///   <c>true</c> if the specified name has technique; otherwise, <c>false</c>.
    /// </returns>
    bool HasTechnique(string name);
    /// <summary>
    /// Occurs when [on invalidate renderer].
    /// </summary>
    event EventHandler<EventArgs> InvalidateRender;
    /// <summary>
    /// Invalidates the renderer.
    /// </summary>
    void RaiseInvalidateRender();
    /// <summary>
    /// Outputs the resource count summary.
    /// </summary>
    /// <returns></returns>
    string GetResourceCountSummary();
}
