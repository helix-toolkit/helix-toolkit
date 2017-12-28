/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
using System;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Shaders;
    public sealed class EmptyRenderCore : RenderCoreBase<ModelStruct>
    {
        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return new ConstantBufferDescription(DefaultBufferNames.ModelCB, ModelStruct.SizeInBytes);
        }

        protected override void OnRender(IRenderContext context)
        {

        }

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, IRenderContext context)
        {
           
        }
    }
}
