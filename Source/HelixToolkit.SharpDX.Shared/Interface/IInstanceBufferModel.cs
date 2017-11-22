using System.Collections.Generic;
using HelixToolkit.Wpf.SharpDX.Utilities;
using SharpDX;
using SharpDX.Direct3D11;
using System;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    public interface IInstanceBufferModel : IGUID, IDisposable
    {
        bool HasInstance { get; }
        bool Initialized { get; }

        bool InstanceChanged { get; }

        IBufferProxy InstanceBuffer { get; }

        void AttachBuffer(DeviceContext context, int vertexBufferSlot);
        void Initialize(Effect effect);
    }

    public interface IInstanceBufferModel<T> : IInstanceBufferModel
    {
        IList<T> Instances { get; set; }
    }
}