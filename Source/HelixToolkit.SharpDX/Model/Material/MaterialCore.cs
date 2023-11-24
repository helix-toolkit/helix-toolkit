namespace HelixToolkit.SharpDX.Model;

/// <summary>
/// 
/// </summary>
public abstract class MaterialCore : ObservableObject, IMaterial
{
    private string name = "Material";
    public string Name
    {
        set
        {
            Set(ref name, value);
        }
        get
        {
            return name;
        }
    }

    public Guid Guid { get; } = Guid.NewGuid();

    /// <summary>
    /// Creates the material variables.
    /// </summary>
    /// <param name="manager">The manager.</param>
    /// <param name="technique">The technique.</param>
    /// <returns></returns>
    public abstract MaterialVariable? CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique);
}
