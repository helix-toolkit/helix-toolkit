using System;
using System.Collections.Generic;
using System.Text;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    public interface IRenderCore : IGUID
    {
        void Attach(IRenderHost host, RenderTechnique technique);

        void Detach();

        void Render(IRenderMatrices context);
    }
}
