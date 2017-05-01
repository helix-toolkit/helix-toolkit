using HelixToolkit.Wpf.SharpDX.Model.Lights3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelixToolkit.Wpf.SharpDX
{
    /// <summary>
    /// 
    /// </summary>
    public interface IModelContainer : IRenderHost
    {
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<IRenderable> Renderables { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewport"></param>
        void AttachViewport3DX(Viewport3DX viewport);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewport"></param>
        void DettachViewport3DX(Viewport3DX viewport);
    /// <summary>
    /// 
    /// </summary>
        IRenderHost CurrentRenderHost { set; get; }
    }
}
