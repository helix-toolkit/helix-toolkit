/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using global::SharpDX.Direct3D;
using global::SharpDX.Direct3D11;
using HelixToolkit.Mathematics;
using System.Numerics;
using System;
using System.Linq;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;
    using Utilities;
    /// <summary>
    /// Point Geometry Buffer Model. Use for point rendering
    /// </summary>
    /// <typeparam name="VertexStruct"></typeparam>
    public abstract class PointGeometryBufferModel<VertexStruct> : GeometryBufferModel where VertexStruct : struct
    {
        protected static readonly VertexStruct[] emptyVerts = new VertexStruct[0];
        /// <summary>
        /// Initializes a new instance of the <see cref="PointGeometryBufferModel{VertexStruct}"/> class.
        /// </summary>
        /// <param name="structSize">Size of the structure.</param>
        /// <param name="dynamic">Create dynamic buffer or immutable buffer</param>
        public PointGeometryBufferModel(int structSize, bool dynamic = false) 
            : base(PrimitiveTopology.PointList,
            dynamic ? new DynamicBufferProxy(structSize, BindFlags.VertexBuffer) : new ImmutableBufferProxy(structSize, BindFlags.VertexBuffer) as IElementsBufferProxy,
            null)
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
        /// Called when [create index buffer].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="geometry">The geometry.</param>
        /// <param name="deviceResources">The device resources.</param>
        protected override void OnCreateIndexBuffer(DeviceContextProxy context, IElementsBufferProxy buffer, Geometry3D geometry, IDeviceResources deviceResources)
        {
            
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DefaultPointGeometryBufferModel : PointGeometryBufferModel<PointsVertex>
    {
        [ThreadStatic]
        private static PointsVertex[] vertexArrayBuffer;
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPointGeometryBufferModel"/> class.
        /// </summary>
        public DefaultPointGeometryBufferModel() : base(PointsVertex.SizeInBytes) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPointGeometryBufferModel"/> class.
        /// </summary>
        /// <param name="isDynamic"></param>
        public DefaultPointGeometryBufferModel(bool isDynamic) : base(PointsVertex.SizeInBytes, isDynamic) { }

        /// <summary>
        /// Called when [create vertex buffer].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="geometry">The geometry.</param>
        /// <param name="deviceResources">The device resources.</param>
        /// <param name="bufferIndex"></param>
        protected override void OnCreateVertexBuffer(DeviceContextProxy context, IElementsBufferProxy buffer, int bufferIndex, Geometry3D geometry, IDeviceResources deviceResources)
        {
            // -- set geometry if given
            if (geometry != null && geometry.Positions != null && geometry.Positions.Count > 0)
            {
                // --- get geometry
                var data = OnBuildVertexArray(geometry);
                buffer.UploadDataToBuffer(context, data, geometry.Positions.Count, 0, geometry.PreDefinedVertexCount);
            }
            else
            {
                buffer.UploadDataToBuffer(context, emptyVerts, 0);
            }
        }


        protected override bool IsVertexBufferChanged(string propertyName, int vertexBufferIndex)
        {
            return base.IsVertexBufferChanged(propertyName, vertexBufferIndex) || propertyName.Equals(nameof(Geometry3D.Colors));
        }
        /// <summary>
        /// Called when [build vertex array].
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <returns></returns>
        private PointsVertex[] OnBuildVertexArray(Geometry3D geometry)
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

    /// <summary>
    /// 
    /// </summary>
    public sealed class DynamicPointGeometryBufferModel : DefaultPointGeometryBufferModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicPointGeometryBufferModel"/> class.
        /// </summary>
        public DynamicPointGeometryBufferModel() : base(true) { }
    }
}
