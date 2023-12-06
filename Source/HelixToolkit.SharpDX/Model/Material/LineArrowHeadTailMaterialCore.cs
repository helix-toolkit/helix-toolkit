namespace HelixToolkit.SharpDX.Model;

public class LineArrowHeadTailMaterialCore : LineArrowHeadMaterialCore
{
    public override MaterialVariable? CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
    {
        var tech = manager.GetTechnique(DefaultRenderTechniqueNames.LinesArrowHeadTail);

        if (tech is null)
        {
            return null;
        }

        return new LineArrowMaterialVariable(manager, tech, this);
    }
}
