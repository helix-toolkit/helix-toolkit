using SharpDX;
using SharpDX.Direct3D11;
using System;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public sealed class EmptyRenderCore : RenderCoreBase
    {
        protected override void OnRender(IRenderMatrices context, IRenderHost host)
        {

        }
    }
}
