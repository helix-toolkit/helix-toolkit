using HelixToolkit.Wpf.SharpDX;

using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace CustomShaderDemo
{
    public class CustomEffectsManager : DefaultEffectsManager
    {
        public CustomEffectsManager(IRenderTechniquesManager renderTechniquesManager) : base(renderTechniquesManager) { }

        protected override void InitEffects()
        {
            var custom = renderTechniquesManager.RenderTechniques["RenderCustom"];
            var lines = renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Lines];
            var points = renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Points];
            var text = renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.BillboardText];
            var blinn = renderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn];

            RegisterEffect(Properties.Resources._custom, new[] { custom, lines, points, text, blinn});

            var customInputLayout = new InputLayout(device, GetEffect(custom).GetTechniqueByName("RenderCustom").GetPassByIndex(0).Description.Signature, new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float,       InputElement.AppendAligned, 0),
                new InputElement("NORMAL",   0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                new InputElement("TANGENT",  0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                new InputElement("BINORMAL", 0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                new InputElement("COLOR",    1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),

                //INSTANCING: die 4 texcoords sind die matrix, die mit jedem buffer reinwandern
                new InputElement("TEXCOORD", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 4, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
            });

            RegisterLayout(new[] { custom, lines, points, text, blinn}, customInputLayout);

            var linesInputLayout = new InputLayout(device, GetEffect(lines).GetTechniqueByName(DefaultRenderTechniqueNames.Lines).GetPassByIndex(0).Description.Signature, new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                new InputElement("COLOR",    1, Format.R32G32B32A32_Float,    InputElement.AppendAligned, 0),

                //INSTANCING: die 4 texcoords sind die matrix, die mit jedem buffer reinwandern
                new InputElement("TEXCOORD", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 4, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
            });
            RegisterLayout(new[]{lines},linesInputLayout);

            var pointsInputLayout = new InputLayout(device, GetEffect(points).GetTechniqueByName(DefaultRenderTechniqueNames.Points).GetPassByIndex(0).Description.Signature, new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                new InputElement("COLOR",    1, Format.R32G32B32A32_Float,    InputElement.AppendAligned, 0),
            });
            RegisterLayout(new[] { points }, pointsInputLayout);

            var textInputLayout = new InputLayout(device, GetEffect(text).GetTechniqueByName(DefaultRenderTechniqueNames.BillboardText).GetPassByIndex(0).Description.Signature, new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float,  InputElement.AppendAligned, 0),
                new InputElement("COLOR",    0, Format.R32G32B32A32_Float,  InputElement.AppendAligned, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32B32A32_Float,  InputElement.AppendAligned, 0),
            });
            RegisterLayout(new[] { text }, textInputLayout);
        }
    }
}
