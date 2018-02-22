using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Windows;

namespace HelixToolkit.Wpf.SharpDX.PostEffects
{
    public class PostEffectMeshBorderHighlight : Element3D
    {
        protected override IRenderCore OnCreateRenderCore()
        {
            return new PostEffectMeshBorderHighlightCore();
        }

        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.PostEffectMeshOutline];
        }

        protected override bool CanHitTest(IRenderContext context)
        {
            return false;
        }
        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            throw new NotImplementedException();
        }
    }
}
