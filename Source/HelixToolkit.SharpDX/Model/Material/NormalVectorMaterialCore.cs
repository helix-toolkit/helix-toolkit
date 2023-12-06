namespace HelixToolkit.SharpDX.Model;

/// <summary>
/// Vertex Normal Vector Material
/// </summary>
public sealed class NormalVectorMaterialCore : MaterialCore
{
    public static readonly NormalVectorMaterialCore Core = new();
    public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
    {
        return new PassOnlyMaterialVariable(DefaultPassNames.NormalVector, technique);
    }
}
