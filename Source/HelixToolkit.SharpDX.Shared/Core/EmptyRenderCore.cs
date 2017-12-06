using SharpDX;
using SharpDX.Direct3D11;
using System;
using HelixToolkit.Wpf.SharpDX.Shaders;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public sealed class EmptyRenderCore : RenderCoreBase<ModelStruct>
    {
        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return DefaultConstantBufferDescriptions.ModelCB;
        }

        protected override void OnRender(IRenderMatrices context)
        {

        }

        protected override void OnUpdateModelStruct(IRenderMatrices context)
        {
           
        }
    }
}
