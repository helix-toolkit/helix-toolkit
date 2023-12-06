namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public interface IDynamicReflectable
{
    IDynamicReflector? DynamicReflector
    {
        set; get;
    }
}
