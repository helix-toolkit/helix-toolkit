namespace HelixToolkit.SharpDX;

/// <summary>
/// Interface for struct array
/// </summary>
public interface IStructArrayPool : IDisposable
{
    ArrayStorage? Register(int structSize);
}
