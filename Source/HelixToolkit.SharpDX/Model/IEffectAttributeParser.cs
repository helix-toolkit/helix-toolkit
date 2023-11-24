namespace HelixToolkit.SharpDX.Model;

/// <summary>
/// 
/// </summary>
public interface IEffectAttributeParser
{
    EffectAttributes[] Parse(string attString);
}
