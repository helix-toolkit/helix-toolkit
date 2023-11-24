using Assimp.Configs;
using Assimp;

namespace HelixToolkit.SharpDX.Assimp;

/// <summary>
/// 
/// </summary>
public class ExportConfiguration
{
    /// <summary>
    /// The post processing
    /// </summary>
    public PostProcessSteps PostProcessing =
        PostProcessSteps.FlipUVs;

    /// <summary>
    ///     The assimp property configuration
    /// </summary>
    public PropertyConfig[]? AssimpPropertyConfig = null;

    /// <summary>
    ///     The enable parallel processing, such as converting Assimp meshes into HelixToolkit meshes
    /// </summary>
    public bool EnableParallelProcessing;

    /// <summary>
    ///     The external context. Can be use to do more customized configuration for Assimp Importer
    /// </summary>
    public AssimpContext? ExternalContext = null;

    /// <summary>
    /// The global scale for model
    /// </summary>
    public float GlobalScale = 1f;
    /// <summary>
    /// The tickes per second. Only used when file does not contains tickes per second for animation.
    /// </summary>
    public float TickesPerSecond = 25f;

    /// <summary>
    /// The flip triangle winding order during import
    /// </summary>
    public bool FlipWindingOrder = false;

    /// <summary>
    /// Convert transform matrix to column major. Note: Most of software exported model defaults to be column major in transform matrix
    /// </summary>
    public bool ToSourceMatrixColumnMajor = true;
}
