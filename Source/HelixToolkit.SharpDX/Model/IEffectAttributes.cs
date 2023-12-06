namespace HelixToolkit.SharpDX.Model;

/// <summary>
/// 
/// </summary>
public interface IEffectAttributes
{
    string EffectName
    {
        get;
    }
    void AddAttribute(string attName, object parameter);
    void RemoveAttribute(string attName);
    object? GetAttribute(string attName);
    bool TryGetAttribute(string attName, out object? attribute);
}
