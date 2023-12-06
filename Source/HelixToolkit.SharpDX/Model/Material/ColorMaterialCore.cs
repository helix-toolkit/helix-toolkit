namespace HelixToolkit.SharpDX.Model;

/// <summary>
/// Vertex Color Material
/// </summary>
public sealed class ColorMaterialCore : MaterialCore
{
    public static readonly ColorMaterialCore Core = new();
    public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
    {
        return new PassOnlyMaterialVariable(DefaultPassNames.Colors, technique);
    }
}
