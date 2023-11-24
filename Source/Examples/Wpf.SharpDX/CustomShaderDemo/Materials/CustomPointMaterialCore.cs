using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model;

namespace CustomShaderDemo.Materials;

public class CustomPointMaterialCore : PointMaterialCore
{
    public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
    {
        return new CustomPointMaterialVariable(manager, technique, this);
    }
}
