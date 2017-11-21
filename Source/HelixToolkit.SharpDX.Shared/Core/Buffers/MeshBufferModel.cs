#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using global::SharpDX.Direct3D;
    using global::SharpDX.Direct3D11;
    using Utilities;
    /// <summary>
    /// Mesh Geometry Buffer Model.
    /// </summary>
    /// <typeparam name="VertexStruct"></typeparam>
    public class MeshGeometryBufferModel<VertexStruct> : GeometryBufferModel where VertexStruct : struct
    {
        public delegate VertexStruct[] BuildVertexArrayHandler(MeshGeometry3D geometry);
        /// <summary>
        /// Create VertexStruct[] from geometry position, texturecoord, colors, etc.
        /// </summary>
        public BuildVertexArrayHandler OnBuildVertexArray;

        public MeshGeometryBufferModel(int structSize) : base(PrimitiveTopology.TriangleList,
            new ImmutableBufferProxy<VertexStruct>(structSize, BindFlags.VertexBuffer), new ImmutableBufferProxy<int>(sizeof(int), BindFlags.VertexBuffer))
        {
            OnCreateVertexBuffer = (context, buffer, geometry) =>
            {
                // -- set geometry if given
                if (geometry != null && geometry.Positions != null && OnBuildVertexArray != null)
                {
                    // --- get geometry
                    var mesh = geometry as MeshGeometry3D;
                    var data = OnBuildVertexArray(mesh);
                    (buffer as IBufferProxy<VertexStruct>).CreateBufferFromDataArray(context.Device, data, geometry.Positions.Count);
                }
                else
                {
                    buffer.Dispose();
                }
            };
            OnCreateIndexBuffer = (context, buffer, geometry) =>
            {
                if (geometry.Indices != null)
                {
                    (buffer as IBufferProxy<int>).CreateBufferFromDataArray(context.Device, geometry.Indices);
                }
                else
                {
                    buffer.Dispose();
                }
            };
        }

        protected override bool IsVertexBufferChanged(string propertyName)
        {
            return base.IsVertexBufferChanged(propertyName) || propertyName.Equals(nameof(MeshGeometry3D.Colors)) || propertyName.Equals(nameof(MeshGeometry3D.TextureCoordinates));
        }
    }
}
