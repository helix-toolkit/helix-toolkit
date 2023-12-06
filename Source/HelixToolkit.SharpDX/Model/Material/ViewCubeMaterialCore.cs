namespace HelixToolkit.SharpDX.Model;

public sealed class ViewCubeMaterialCore : DiffuseMaterialCore
{
    public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
    {
        return new DiffuseMaterialVariables(DefaultPassNames.ViewCube, manager, technique, this);
    }
}
