namespace HelixToolkit.SharpDX.Model;

/// <summary>
/// Vertex Normal Material
/// </summary>
public sealed class NormalMaterialCore : MaterialCore
{
    public static readonly NormalMaterialCore Core = new();

    public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
    {
        return new PassOnlyMaterialVariable(DefaultPassNames.Normals, technique);
    }
}
