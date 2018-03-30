/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Linq;
using SharpDX;
using global::SharpDX.Direct3D;
using global::SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Utilities;
    /// <summary>
    /// Point Geometry Buffer Model. Use for point rendering
    /// </summary>
    /// <typeparam name="VertexStruct"></typeparam>
    public abstract class PointGeometryBufferModel<VertexStruct> : GeometryBufferModel where VertexStruct : struct
    {
        /// <summary>
        /// Called when [build vertex array].
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <returns></returns>
        protected abstract VertexStruct[] OnBuildVertexArray(PointGeometry3D geometry);

        /// <summary>
        /// Initializes a new instance of the <see cref="PointGeometryBufferModel{VertexStruct}"/> class.
        /// </summary>
        /// <param name="structSize">Size of the structure.</param>
        public PointGeometryBufferModel(int structSize) : base(PrimitiveTopology.PointList,
            new ImmutableBufferProxy(structSize, BindFlags.VertexBuffer), null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointGeometryBufferModel{VertexStruct}"/> class.
        /// </summary>
        /// <param name="vertexBuffer"></param>
        public PointGeometryBufferModel(IElementsBufferProxy vertexBuffer) : base(PrimitiveTopology.PointList,
            vertexBuffer, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointGeometryBufferModel{VertexStruct}"/> class.
        /// </summary>
        /// <param name="vertexBuffer"></param>
        public PointGeometryBufferModel(IElementsBufferProxy[] vertexBuffer) : base(PrimitiveTopology.PointList,
            vertexBuffer, null)
        {
        }
        /// <summary>
        /// Called when [create vertex buffer].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="geometry">The geometry.</param>
        /// <param name="deviceResources">The device resources.</param>
        /// <param name="bufferIndex"></param>
        protected override void OnCreateVertexBuffer(DeviceContext context, IElementsBufferProxy buffer, int bufferIndex, Geometry3D geometry, IDeviceResources deviceResources)
        {
            // -- set geometry if given
            if (geometry != null && geometry.Positions != null && geometry.Positions.Count > 0)
            {
                // --- get geometry
                var mesh = geometry as PointGeometry3D;
                var data = OnBuildVertexArray(mesh);
                buffer.UploadDataToBuffer(context, data, geometry.Positions.Count);
            }
            else
            {
                buffer.DisposeAndClear();
            }
        }
        /// <summary>
        /// Called when [create index buffer].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="geometry">The geometry.</param>
        /// <param name="deviceResources">The device resources.</param>
        protected override void OnCreateIndexBuffer(DeviceContext context, IElementsBufferProxy buffer, Geometry3D geometry, IDeviceResources deviceResources)
        {
            
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class DefaultPointGeometryBufferModel : PointGeometryBufferModel<PointsVertex>
    {
        [ThreadStatic]
        private static PointsVertex[] vertexArrayBuffer;
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPointGeometryBufferModel"/> class.
        /// </summary>
        public DefaultPointGeometryBufferModel() : base(PointsVertex.SizeInBytes) { }

        /// <summary>
        /// Called when [build vertex array].
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <returns></returns>
        protected override PointsVertex[] OnBuildVertexArray(PointGeometry3D geometry)
        {
            var positions = geometry.Positions;
            var vertexCount = geometry.Positions.Count;
            var array = vertexArrayBuffer != null && vertexArrayBuffer.Length >= vertexCount ? vertexArrayBuffer : new PointsVertex[vertexCount];
            var colors = geometry.Colors != null ? geometry.Colors.GetEnumerator() : Enumerable.Repeat(Color4.White, vertexCount).GetEnumerator();
            vertexArrayBuffer = array;
            for (var i = 0; i < vertexCount; i++)
            {
                colors.MoveNext();
                array[i].Position = new Vector4(positions[i], 1f);
                array[i].Color = colors.Current;
            }
            colors.Dispose();
            return array;
        }
    }
}
