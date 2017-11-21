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
    /// Point Geometry Buffer Model. Use for point rendering
    /// </summary>
    /// <typeparam name="VertexStruct"></typeparam>
    public class PointGeometryBufferModel<VertexStruct> : GeometryBufferModel where VertexStruct : struct
    {
        public delegate VertexStruct[] BuildVertexArrayHandler(PointGeometry3D geometry);
        /// <summary>
        /// Create VertexStruct[] from geometry position, colors etc.
        /// </summary>
        public BuildVertexArrayHandler OnBuildVertexArray;

        public PointGeometryBufferModel(int structSize) : base(PrimitiveTopology.PointList,
            new ImmutableBufferProxy<VertexStruct>(structSize, BindFlags.VertexBuffer), null)
        {
            OnCreateVertexBuffer = (context, buffer, geometry) =>
            {
                // -- set geometry if given
                if (geometry != null && geometry.Positions != null && OnBuildVertexArray != null)
                {
                    // --- get geometry
                    var mesh = geometry as PointGeometry3D;
                    var data = OnBuildVertexArray(mesh);
                    (buffer as IBufferProxy<VertexStruct>).CreateBufferFromDataArray(context.Device, data, geometry.Positions.Count);
                }
                else
                {
                    buffer.Dispose();
                }
            };
        }
    }
}
