namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public interface IResourceSharing : IDisposable
{
    /// <summary>
    /// Attaches the specified model unique identifier.
    /// </summary>
    /// <param name="modelGuid">The model unique identifier.</param>
    void Attach(Guid modelGuid);

    /// <summary>
    /// Detaches the specified model unique identifier.
    /// </summary>
    /// <param name="modelGuid">The model unique identifier.</param>
    void Detach(Guid modelGuid);
}
