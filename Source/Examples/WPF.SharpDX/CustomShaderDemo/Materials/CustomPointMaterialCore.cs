using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX.Model;
using SharpDX;

namespace CustomShaderDemo.Materials
{
    public class CustomPointMaterialCore : PointMaterialCore
    {
        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new CustomPointMaterialVariable(manager, technique, this);
        }
    }
}