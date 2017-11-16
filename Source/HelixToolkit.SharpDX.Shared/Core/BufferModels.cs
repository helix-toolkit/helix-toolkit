#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using global::SharpDX;
    using global::SharpDX.Direct3D;
    using global::SharpDX.Direct3D11;
    using Utilities;
    public class InstanceBufferModel : DisposeObject
    {
        public readonly DynamicBufferProxy<Matrix> InstanceBuffer = new DynamicBufferProxy<Matrix>(Matrix.SizeInBytes, BindFlags.VertexBuffer);

        public InstanceBufferModel()
        {
            Collect(InstanceBuffer);
        }
    }

    public class MeshBufferModel : InstanceBufferModel
    {
        public PrimitiveTopology PrimitiveType = PrimitiveTopology.TriangleList;
        public InputLayout InputLayout;
        public readonly ImmutableBufferProxy<DefaultVertex> VertexBuffer = new ImmutableBufferProxy<DefaultVertex>(DefaultVertex.SizeInBytes, BindFlags.VertexBuffer);
        public readonly ImmutableBufferProxy<int> IndexBuffer = new ImmutableBufferProxy<int>(sizeof(int), BindFlags.IndexBuffer);

        public MeshBufferModel(InputLayout layout)
            :base()
        {
            Collect(VertexBuffer);
            Collect(IndexBuffer);
            InputLayout = layout;
        }
    }
}
