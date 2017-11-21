#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using global::SharpDX.Direct3D;
    using global::SharpDX.Direct3D11;
    using System;
    using Utilities;
    public class BillboardBufferModel<VertexStruct> : GeometryBufferModel where VertexStruct : struct
    {
        public BillboardBufferModel(int structSize)
            : base(PrimitiveTopology.TriangleStrip, new ImmutableBufferProxy<VertexStruct>(structSize, BindFlags.VertexBuffer), null)
        {
        }

        protected override void OnCreateIndexBuffer(DeviceContext context, IBufferProxy buffer, Geometry3D geometry)
        {
            throw new NotImplementedException();
        }

        protected override void OnCreateVertexBuffer(DeviceContext context, IBufferProxy buffer, Geometry3D geometry)
        {
            throw new NotImplementedException();
        }
    }
}
