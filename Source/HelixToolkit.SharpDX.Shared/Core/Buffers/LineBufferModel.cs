/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using global::SharpDX.Direct3D;
using global::SharpDX.Direct3D11;
using HelixToolkit.Mathematics;
using System;
using System.Linq;
using System.Numerics;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;
    using Utilities;
    /// <summary>
    /// Line Geometry Buffer Model. Used for line rendering
    /// </summary>
    /// <typeparam name="VertexStruct"></typeparam>
    public abstract class LineGeometryBufferModel<VertexStruct> : GeometryBufferModel where VertexStruct : struct
    {
        protected static readonly VertexStruct[] emptyVertices = new VertexStruct[0];
        protected static readonly int[] emptyIndices = new int[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="LineGeometryBufferModel{VertexStruct}"/> class.
        /// </summary>
        /// <param name="structSize">Size of the structure.</param>
        /// <param name="dynamic">Create dynamic buffer or immutable buffer</param>
        public LineGeometryBufferModel(int structSize, bool dynamic = false)
            : base(PrimitiveTopology.LineList,
            dynamic ? new DynamicBufferProxy(structSize, BindFlags.VertexBuffer) : new ImmutableBufferProxy(structSize, BindFlags.VertexBuffer) as IElementsBufferProxy, 
            dynamic ? new DynamicBufferProxy(sizeof(int), BindFlags.IndexBuffer) : new ImmutableBufferProxy(sizeof(int), BindFlags.IndexBuffer) as IElementsBufferProxy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineGeometryBufferModel{VertexStruct}"/> class.
        /// </summary>
        /// <param name="vertexBuffer"></param>
        /// <param name="dynamic">Create dynamic buffer or immutable buffer</param> 
        public LineGeometryBufferModel(IElementsBufferProxy vertexBuffer, bool dynamic = false)
            : base(PrimitiveTopology.LineList,
            vertexBuffer,
            dynamic ? new DynamicBufferProxy(sizeof(int), BindFlags.IndexBuffer) : new ImmutableBufferProxy(sizeof(int), BindFlags.IndexBuffer) as IElementsBufferProxy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineGeometryBufferModel{VertexStruct}"/> class.
        /// </summary>
        /// <param name="vertexBuffer"></param>
        /// <param name="dynamic">Create dynamic buffer or immutable buffer</param> 
        public LineGeometryBufferModel(IElementsBufferProxy[] vertexBuffer, bool dynamic = false) 
            : base(PrimitiveTopology.LineList,
            vertexBuffer,
            dynamic ? new DynamicBufferProxy(sizeof(int), BindFlags.IndexBuffer) : new ImmutableBufferProxy(sizeof(int), BindFlags.IndexBuffer) as IElementsBufferProxy)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="LineGeometryBufferModel{VertexStruct}"/> class.
        /// </summary>
        /// <param name="vertexBuffer"></param>
        /// <param name="indexBuffer"></param>
        public LineGeometryBufferModel(IElementsBufferProxy vertexBuffer, IElementsBufferProxy indexBuffer)
            : base(PrimitiveTopology.LineList,
            vertexBuffer, indexBuffer)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="LineGeometryBufferModel{VertexStruct}"/> class.
        /// </summary>
        /// <param name="vertexBuffer"></param>
        /// <param name="indexBuffer"></param>
        public LineGeometryBufferModel(IElementsBufferProxy[] vertexBuffer, IElementsBufferProxy indexBuffer) 
            : base(PrimitiveTopology.LineList,
            vertexBuffer, indexBuffer)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DefaultLineGeometryBufferModel : LineGeometryBufferModel<LinesVertex>
    {
        [ThreadStatic]
        private static LinesVertex[] vertexArrayBuffer = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultLineGeometryBufferModel"/> class.
        /// </summary>
        public DefaultLineGeometryBufferModel() : base(LinesVertex.SizeInBytes) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultLineGeometryBufferModel"/> class.
        /// </summary>
        /// <param name="isDynamic"></param>
        public DefaultLineGeometryBufferModel(bool isDynamic) : base(LinesVertex.SizeInBytes, isDynamic) { }

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
                //buffer.DisposeAndClear();
                buffer.UploadDataToBuffer(context, emptyVertices, 0);
            }
        }

        protected override bool IsVertexBufferChanged(string propertyName, int vertexBufferIndex)
        {
            return base.IsVertexBufferChanged(propertyName, vertexBufferIndex) || propertyName.Equals(nameof(Geometry3D.Colors));
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
            if (geometry != null && geometry.Indices != null && geometry.Indices.Count > 0)
            {
                buffer.UploadDataToBuffer(context, geometry.Indices, geometry.Indices.Count, 0, geometry.PreDefinedIndexCount);
            }
            else
            {
                buffer.UploadDataToBuffer(context, emptyIndices, 0);
            }
        }
        /// <summary>
        /// Called when [build vertex array].
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <returns></returns>
        private LinesVertex[] OnBuildVertexArray(Geometry3D geometry)
        {
            var positions = geometry.Positions;
            var vertexCount = geometry.Positions.Count;
            var array =  vertexArrayBuffer != null && vertexArrayBuffer.Length >= vertexCount ? vertexArrayBuffer : new LinesVertex[vertexCount];
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
    public sealed class DynamicLineGeometryBufferModel : DefaultLineGeometryBufferModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicLineGeometryBufferModel"/> class.
        /// </summary>
        public DynamicLineGeometryBufferModel() : base(true) { }
    }
}
