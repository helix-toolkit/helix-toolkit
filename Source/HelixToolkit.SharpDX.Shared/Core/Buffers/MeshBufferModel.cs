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
    /// Mesh Geometry Buffer Model.
    /// </summary>
    /// <typeparam name="VertexStruct"></typeparam>
    public abstract class MeshGeometryBufferModel<VertexStruct> : GeometryBufferModel where VertexStruct : struct
    {
        protected static readonly VertexStruct[] emptyVerts = new VertexStruct[0];
        protected static readonly int[] emptyIndices = new int[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="MeshGeometryBufferModel{VertexStruct}"/> class.
        /// </summary>
        /// <param name="structSize">Size of the structure.</param>
        /// <param name="dynamic">Create dynamic buffer or immutable buffer</param>
        public MeshGeometryBufferModel(int structSize, bool dynamic = false)
            : base(PrimitiveTopology.TriangleList,
                  dynamic ? new DynamicBufferProxy(structSize, BindFlags.VertexBuffer) : new ImmutableBufferProxy(structSize, BindFlags.VertexBuffer) as IElementsBufferProxy,
                  dynamic ? new DynamicBufferProxy(sizeof(int), BindFlags.IndexBuffer) : new ImmutableBufferProxy(sizeof(int), BindFlags.IndexBuffer) as IElementsBufferProxy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeshGeometryBufferModel{VertexStruct}"/> class.
        /// </summary>
        /// <param name="structSize">Size of the structure.</param>
        /// <param name="topology">The topology.</param>
        /// <param name="dynamic">Create dynamic buffer or immutable buffer</param>
        public MeshGeometryBufferModel(int structSize, PrimitiveTopology topology, bool dynamic = false)
            : base(topology,
                  dynamic ? new DynamicBufferProxy(structSize, BindFlags.VertexBuffer) : new ImmutableBufferProxy(structSize, BindFlags.VertexBuffer) as IElementsBufferProxy,
                  dynamic ? new DynamicBufferProxy(sizeof(int), BindFlags.IndexBuffer) : new ImmutableBufferProxy(sizeof(int), BindFlags.IndexBuffer) as IElementsBufferProxy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeshGeometryBufferModel{VertexStruct}"/> class.
        /// </summary>
        /// <param name="topology">The topology.</param>
        /// <param name="vertexBuffers"></param>
        /// <param name="dynamic">Create dynamic buffer or immutable buffer</param>
        public MeshGeometryBufferModel(PrimitiveTopology topology, IElementsBufferProxy[] vertexBuffers, bool dynamic = false)
            : base(topology,
                  vertexBuffers,
                  dynamic ? new DynamicBufferProxy(sizeof(int), BindFlags.IndexBuffer) : new ImmutableBufferProxy(sizeof(int), BindFlags.IndexBuffer) as IElementsBufferProxy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeshGeometryBufferModel{VertexStruct}"/> class.
        /// </summary>
        /// <param name="topology">The topology.</param>
        /// <param name="vertexBuffer">The vertex buffer.</param>
        /// <param name="indexBuffer">The index buffer.</param>
        protected MeshGeometryBufferModel(PrimitiveTopology topology, IElementsBufferProxy vertexBuffer, IElementsBufferProxy indexBuffer)
            : base(topology, vertexBuffer, indexBuffer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeshGeometryBufferModel{VertexStruct}"/> class.
        /// </summary>
        /// <param name="topology">The topology.</param>
        /// <param name="vertexBuffer">The vertex buffer.</param>
        /// <param name="indexBuffer">The index buffer.</param>
        protected MeshGeometryBufferModel(PrimitiveTopology topology, IElementsBufferProxy[] vertexBuffer, IElementsBufferProxy indexBuffer)
            : base(topology, vertexBuffer, indexBuffer)
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
            if (geometry != null && geometry.Indices != null && geometry.Indices.Count > 0)
            {
                buffer.UploadDataToBuffer(context, geometry.Indices, geometry.Indices.Count, 0, geometry.PreDefinedIndexCount);
            }
            else
            {
                buffer.UploadDataToBuffer(context, emptyIndices, 0);
                //buffer.DisposeAndClear();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DefaultMeshGeometryBufferModel : MeshGeometryBufferModel<DefaultVertex>
    {
        [ThreadStatic]
        private static DefaultVertex[] vertexArrayBuffer = null;
        private static readonly Vector2[] emptyTextureArray = new Vector2[0];
        private static readonly Vector4[] emptyColorArray = new Vector4[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMeshGeometryBufferModel"/> class.
        /// </summary>
        public DefaultMeshGeometryBufferModel() 
            : base(PrimitiveTopology.TriangleList,
                  new[]
                  {
                      new ImmutableBufferProxy(DefaultVertex.SizeInBytes, BindFlags.VertexBuffer),
                      new ImmutableBufferProxy(Vector2Helper.SizeInBytes, BindFlags.VertexBuffer),
                      new ImmutableBufferProxy(Vector4Helper.SizeInBytes, BindFlags.VertexBuffer)
                  } as IElementsBufferProxy[])
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMeshGeometryBufferModel"/> class.
        /// </summary>
        /// <param name="buffers">The buffers.</param>
        /// <param name="isDynamic"></param>
        public DefaultMeshGeometryBufferModel(IElementsBufferProxy[] buffers, bool isDynamic) 
            : base(PrimitiveTopology.TriangleList, buffers, isDynamic)
        {
        }
        /// <summary>
        /// Determines whether [is vertex buffer changed] [the specified property name].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="bufferIndex"></param>
        /// <returns>
        ///   <c>true</c> if [is vertex buffer changed] [the specified property name]; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsVertexBufferChanged(string propertyName, int bufferIndex)
        {
            switch (bufferIndex)
            {
                case 0:
                    return base.IsVertexBufferChanged(propertyName, bufferIndex);
                case 1:
                    return propertyName.Equals(nameof(MeshGeometry3D.TextureCoordinates));                  
                case 2:
                    return propertyName.Equals(nameof(MeshGeometry3D.Colors));
                default:
                    return false;
            }
        }
        /// <summary>
        /// Called when [create vertex buffer].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="bufferIndex">Index of the buffer.</param>
        /// <param name="geometry">The geometry.</param>
        /// <param name="deviceResources">The device resources.</param>
        protected override void OnCreateVertexBuffer(DeviceContextProxy context, IElementsBufferProxy buffer, int bufferIndex, Geometry3D geometry, IDeviceResources deviceResources)
        {
            if(geometry is MeshGeometry3D mesh)
            {
                switch (bufferIndex)
                {
                    case 0:
                        // -- set geometry if given
                        if (geometry.Positions != null && geometry.Positions.Count > 0)
                        {
                            // --- get geometry
                            var data = BuildVertexArray(mesh);
                            buffer.UploadDataToBuffer(context, data, geometry.Positions.Count, 0, geometry.PreDefinedVertexCount);
                        }
                        else
                        {
                            //buffer.DisposeAndClear();
                            buffer.UploadDataToBuffer(context, emptyVerts, 0);
                        }
                        break;
                    case 1:
                        if(mesh.TextureCoordinates != null && mesh.TextureCoordinates.Count > 0)
                        {
                            buffer.UploadDataToBuffer(context, mesh.TextureCoordinates, mesh.TextureCoordinates.Count, 0, geometry.PreDefinedVertexCount);
                        }
                        else
                        {
                            buffer.UploadDataToBuffer(context, emptyTextureArray, 0);
                        }
                        break;
                    case 2:
                        if (geometry.Colors != null && geometry.Colors.Count > 0)
                        {
                            buffer.UploadDataToBuffer(context, geometry.Colors, geometry.Colors.Count, 0, geometry.PreDefinedVertexCount);
                        }
                        else
                        {
                            buffer.UploadDataToBuffer(context, emptyColorArray, 0);
                        }
                        break;
                }
            }
        }
        /// <summary>
        /// Builds the vertex array.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <returns></returns>
        private DefaultVertex[] BuildVertexArray(MeshGeometry3D geometry)
        {
            //var geometry = this.geometryInternal as MeshGeometry3D;
            var positions = geometry.Positions.GetEnumerator();
            var vertexCount = geometry.Positions.Count;

            var normals = geometry.Normals != null ? geometry.Normals.GetEnumerator() : Enumerable.Repeat(Vector3.Zero, vertexCount).GetEnumerator();
            var tangents = geometry.Tangents != null ? geometry.Tangents.GetEnumerator() : Enumerable.Repeat(Vector3.Zero, vertexCount).GetEnumerator();
            var bitangents = geometry.BiTangents != null ? geometry.BiTangents.GetEnumerator() : Enumerable.Repeat(Vector3.Zero, vertexCount).GetEnumerator();

            var array = vertexArrayBuffer != null && vertexArrayBuffer.Length >= vertexCount ? vertexArrayBuffer : new DefaultVertex[vertexCount];
            vertexArrayBuffer = array;
            for (var i = 0; i < vertexCount; i++)
            {
                positions.MoveNext();
                normals.MoveNext();
                tangents.MoveNext();
                bitangents.MoveNext();
                array[i].Position = new Vector4(positions.Current, 1f);
                array[i].Normal = normals.Current;
                array[i].Tangent = tangents.Current;
                array[i].BiTangent = bitangents.Current;
            }
            normals.Dispose();
            tangents.Dispose();
            bitangents.Dispose();
            positions.Dispose();
            return array;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class DynamicMeshGeometryBufferModel : DefaultMeshGeometryBufferModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicMeshGeometryBufferModel"/> class.
        /// </summary>
        public DynamicMeshGeometryBufferModel()
            : base(new[]
                  {
                      new DynamicBufferProxy(DefaultVertex.SizeInBytes, BindFlags.VertexBuffer),
                      new DynamicBufferProxy(Vector2Helper.SizeInBytes, BindFlags.VertexBuffer),
                      new DynamicBufferProxy(Vector4Helper.SizeInBytes, BindFlags.VertexBuffer)
                  } as IElementsBufferProxy[], true)
        { }
    }
}
