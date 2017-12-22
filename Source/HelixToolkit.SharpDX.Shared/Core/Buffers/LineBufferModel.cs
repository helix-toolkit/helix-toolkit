/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using System;
    using global::SharpDX.Direct3D;
    using global::SharpDX.Direct3D11;
    using Utilities;
    /// <summary>
    /// Line Geometry Buffer Model. Used for line rendering
    /// </summary>
    /// <typeparam name="VertexStruct"></typeparam>
    public class LineGeometryBufferModel<VertexStruct> : GeometryBufferModel where VertexStruct : struct
    {
        public delegate VertexStruct[] BuildVertexArrayHandler(LineGeometry3D geometry);
        /// <summary>
        /// Create VertexStruct[] from geometry position, colors etc.
        /// </summary>
        public BuildVertexArrayHandler OnBuildVertexArray;

        public LineGeometryBufferModel(int structSize) : base(PrimitiveTopology.LineList,
            new ImmutableBufferProxy(structSize, BindFlags.VertexBuffer), new ImmutableBufferProxy(sizeof(int), BindFlags.VertexBuffer))
        {
        }

        protected override void OnCreateVertexBuffer(DeviceContext context, IBufferProxy buffer, Geometry3D geometry)
        {
            // -- set geometry if given
            if (geometry != null && geometry.Positions != null && OnBuildVertexArray != null)
            {
                // --- get geometry
                var mesh = geometry as LineGeometry3D;
                var data = OnBuildVertexArray(mesh);
                buffer.CreateBufferFromDataArray(context.Device, data, geometry.Positions.Count);
            }
            else
            {
                buffer.Dispose();
            }
        }

        protected override void OnCreateIndexBuffer(DeviceContext context, IBufferProxy buffer, Geometry3D geometry)
        {
            if (geometry.Indices != null)
            {
                buffer.CreateBufferFromDataArray(context.Device, geometry.Indices);
            }
            else
            {
                buffer.Dispose();
            }
        }
    }
}
