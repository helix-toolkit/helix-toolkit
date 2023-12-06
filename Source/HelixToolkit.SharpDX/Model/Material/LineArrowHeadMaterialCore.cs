namespace HelixToolkit.SharpDX.Model;

public class LineArrowHeadMaterialCore : LineMaterialCore
{
    private float arrowSize = 0.1f;
    /// <summary>
    /// Gets or sets the size of the arrow.
    /// </summary>
    /// <value>
    /// The size of the arrow.
    /// </value>
    public float ArrowSize
    {
        set
        {
            Set(ref arrowSize, value);
        }
        get
        {
            return arrowSize;
        }
    }

    public override MaterialVariable? CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
    {
        var tech = manager.GetTechnique(DefaultRenderTechniqueNames.LinesArrowHead);

        if (tech is null)
        {
            return null;
        }

        return new LineArrowMaterialVariable(manager, tech, this);
    }
}
