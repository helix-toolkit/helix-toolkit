#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class EmptyRenderCore2D : RenderCore2DBase
    {
        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="matrices">The matrices.</param>
        protected override void OnRender(RenderContext2D matrices)
        {
            
        }
    }
}
