using System;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    using global::SharpDX.Direct3D11;
    using Utilities;
    public interface IVertexExtraBufferModel : IGUID, IDisposable
    {
        bool Initialized { get; }

        bool Changed { get; }

        IBufferProxy Buffer { get; }

        void AttachBuffer(DeviceContext context, int vertexBufferSlot);
        void Initialize();
    }
}
