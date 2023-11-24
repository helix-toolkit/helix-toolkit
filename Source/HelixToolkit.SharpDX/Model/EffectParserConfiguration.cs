namespace HelixToolkit.SharpDX.Model;

/// <summary>
/// 
/// </summary>
public static class EffectParserConfiguration
{
    public static IEffectAttributeParser Parser { get; set; } = new DefaultEffectAttributeParser();
}
