using System.Collections.Generic;

using HelixToolkit.Wpf.SharpDX;

namespace CustomShaderDemo
{
    public class CustomRenderTechniquesManager : DefaultRenderTechniquesManager
    {
        private Dictionary<string, RenderTechnique> renderTechniques = new Dictionary<string, RenderTechnique>();

        protected override void InitTechniques()
        {
            AddRenderTechnique(DefaultRenderTechniqueNames.Blinn, Properties.Resources._custom);
            AddRenderTechnique(DefaultRenderTechniqueNames.Points,Properties.Resources._custom);
            AddRenderTechnique(DefaultRenderTechniqueNames.Lines, Properties.Resources._custom);
            AddRenderTechnique(DefaultRenderTechniqueNames.BillboardText, Properties.Resources._custom);
            AddRenderTechnique("RenderCustom", Properties.Resources._custom);
        }
    }
}
