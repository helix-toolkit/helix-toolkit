using HelixToolkit.SharpDX.Model;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public interface IMaterialVariablePool : IDisposable
{
    int Count
    {
        get;
    }

    MaterialVariable? Register(IMaterial? material, IRenderTechnique? technique);
}
