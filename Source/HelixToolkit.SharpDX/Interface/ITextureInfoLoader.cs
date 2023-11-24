namespace HelixToolkit.SharpDX;

/// <summary>
/// Loads texture info and uploads texture to GPU on demand.
/// </summary>
public interface ITextureInfoLoader
{
    /// <summary>
    /// Called before GPU texture resource creation.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    TextureInfo Load(Guid id);

    /// <summary>
    /// Called after GPU texture resource creation.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="info">The information.</param>
    /// <param name="succeeded">if set to <c>true</c> [succeeded].</param>
    void Complete(Guid id, TextureInfo? info, bool succeeded);
}
