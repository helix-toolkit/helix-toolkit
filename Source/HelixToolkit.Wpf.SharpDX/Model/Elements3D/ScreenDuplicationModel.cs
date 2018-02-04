using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.Wpf.SharpDX
{
    public class ScreenDuplicationModel : Element3D
    {
        public ScreenDuplicationModel()
        {
            IsHitTestVisible = false;

        }

        protected override IRenderCore OnCreateRenderCore()
        {
            return new ScreenCloneRenderCore();
        }

        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.ScreenDuplication];
        }

        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }
    }
}
