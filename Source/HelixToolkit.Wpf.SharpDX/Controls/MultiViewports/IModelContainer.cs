using HelixToolkit.Wpf.SharpDX.Model.Lights3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelixToolkit.Wpf.SharpDX
{
    public interface IModelContainer : IRenderHost
    {
        IEnumerable<IRenderable> Renderables { get; }

        void AttachViewport3DX(Viewport3DX viewport);

        void DettachViewport3DX(Viewport3DX viewport);

        IRenderHost CurrentRenderHost { set; get; }
    }
}
