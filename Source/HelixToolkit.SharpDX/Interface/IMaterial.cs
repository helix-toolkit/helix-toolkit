using HelixToolkit.SharpDX.Model;
using System.ComponentModel;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public interface IMaterial : INotifyPropertyChanged
{
    string Name
    {
        set; get;
    }
    Guid Guid
    {
        get;
    }
    MaterialVariable? CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique);
}
