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
        IBufferProxy<Matrix> InstanceBuffer { get; }
        IList<Matrix> Instances { get; set; }

        void Attach(DeviceContext context);
        void Initialize(Effect effect);
    }
}