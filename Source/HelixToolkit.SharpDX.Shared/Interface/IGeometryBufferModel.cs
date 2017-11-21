using System;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    using Utilities;
    public interface IGeometryBufferModel : IGUID, IDisposable
    {
        Geometry3D Geometry { get; set; }
        PrimitiveTopology Topology { get; }

        event EventHandler<bool> InvalidateRenderer;
        IBufferProxy VertexBuffer { get; }
        IBufferProxy IndexBuffer { get; }

        bool AttachBuffers(DeviceContext context, InputLayout vertexLayout, IInstanceBufferModel instanceModel);
    }

    public interface IBillboardBufferModel
    {
        ShaderResourceView TextureView { get; }
        ShaderResourceView AlphaTextureView { get; }

        BillboardType Type { get; }
    }
}