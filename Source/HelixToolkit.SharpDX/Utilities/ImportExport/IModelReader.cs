using Mesh3DGroup = System.Collections.Generic.List<HelixToolkit.SharpDX.Object3D>;

namespace HelixToolkit.SharpDX;

/// <summary>
/// Interface for model readers.
/// </summary>
public interface IModelReader
{
    /// <summary>
    /// Reads the model from the specified path.
    /// </summary>
    /// <param name="path">
    /// The path.
    /// </param>
    /// <param name="info">
    /// The model info.
    /// </param>
    /// <returns>
    /// The model.
    /// </returns>
    Mesh3DGroup? Read(string path, ModelInfo info = default);

    /// <summary>
    /// Reads the model from the specified stream.
    /// </summary>
    /// <param name="s">
    /// The stream.
    /// </param>
    /// <param name="info">
    /// The model info.
    /// </param>
    /// <returns>
    /// The model.
    /// </returns>
    Mesh3DGroup? Read(Stream s, ModelInfo info = default);
}
