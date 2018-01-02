using System.Collections.Generic;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
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
        void AttachViewport3DX(IRenderer viewport);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewport"></param>
        void DettachViewport3DX(IRenderer viewport);
    /// <summary>
    /// 
    /// </summary>
        IRenderHost CurrentRenderHost { set; get; }

        void Attach(IRenderHost host);
        void Detach();
    }
}
