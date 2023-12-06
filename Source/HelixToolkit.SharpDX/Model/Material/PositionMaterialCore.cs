namespace HelixToolkit.SharpDX.Model;

/// <summary>
/// Vertex Position Material
/// </summary>
public sealed class PositionMaterialCore : MaterialCore
{
    public static readonly PositionMaterialCore Core = new();
    public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
    {
        return new PassOnlyMaterialVariable(DefaultPassNames.Positions, technique);
    }
}
