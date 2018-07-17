using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;    
    using Utilities;
    public interface IBatchedGeometry
    {
        Geometry3D Geometry { get; }
        Matrix ModelTransform { get; }
    }

    public abstract class StaticGeometryBatchingBufferBase<BatchedGeometry, VertStruct> : DisposeObject, IAttachableBufferModel
        where BatchedGeometry : struct, IBatchedGeometry where VertStruct : struct
    {
        public Guid GUID { get; } = Guid.NewGuid();
        public event EventHandler<EventArgs> OnInvalidateRender;
        private bool isGeometryChanged = true;
        private static readonly VertStruct[] EmptyArray = new VertStruct[0];
        private static readonly int[] EmptyIntArray = new int[0];
        /// <summary>
        /// Gets or sets the vertex buffer.
        /// </summary>
        /// <value>
        /// The vertex buffer.
        /// </value>
        public IElementsBufferProxy[] VertexBuffer { private set; get; }
        public IEnumerable<int> VertexStructSize { get { return VertexBuffer.Select(x => x != null ? x.StructureSize : 0); } }
        private VertexBufferBinding[] vertexBufferBindings = new VertexBufferBinding[0];
        /// <summary>
        /// Gets or sets the index buffer.
        /// </summary>
        /// <value>
        /// The index buffer.
        /// </value>
        public IElementsBufferProxy IndexBuffer { private set; get; }
        /// <summary>
        /// Gets or sets the topology.
        /// </summary>
        /// <value>
        /// The topology.
        /// </value>
        public PrimitiveTopology Topology { set; get; }

        private BatchedGeometry[] geometries;
        public BatchedGeometry[] Geometries
        {
            set
            {
                if(Set(ref geometries, value))
                {
                    InvalidateGeometries();
                }
            }
            get
            {
                return geometries;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvalidateGeometries()
        {
            isGeometryChanged = true;
            OnInvalidateRender?.Invoke(this, EventArgs.Empty);
        }

        public StaticGeometryBatchingBufferBase(PrimitiveTopology topology, IElementsBufferProxy vertexBuffer, IElementsBufferProxy indexBuffer)
        {
            Topology = topology;
            VertexBuffer = new IElementsBufferProxy[] { Collect(vertexBuffer) };
            if (indexBuffer != null)
            { IndexBuffer = Collect(indexBuffer); }
        }

        public bool Commit(DeviceContextProxy deviceContext)
        {
            if (isGeometryChanged)
            {
                OnSubmitGeometries(deviceContext);
                isGeometryChanged = false;
                OnInvalidateRender?.Invoke(this, EventArgs.Empty);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual void OnSubmitGeometries(DeviceContextProxy deviceContext)
        {
            if(Geometries == null)
            {
                VertexBuffer[0].UploadDataToBuffer(deviceContext, EmptyArray, 0);
                IndexBuffer?.UploadDataToBuffer(deviceContext, EmptyIntArray, 0);
                vertexBufferBindings = new VertexBufferBinding[0];
                return;
            }
            int totalVertex = 0;
            int totalIndices = 0;

            for(int i=0; i < Geometries.Length; ++i)
            {
                totalVertex += Geometries[i].Geometry.Positions.Count;
                if (Geometries[i].Geometry.Indices != null)
                {
                    totalIndices += Geometries[i].Geometry.Indices.Count;
                }
            }
            var tempVerts = new VertStruct[totalVertex];
            var tempIndices = new int[totalIndices];
            int vertOffset = 0;
            int indexOffset = 0;

            for(int i = 0; i < Geometries.Length; ++i)
            {
                var geo = Geometries[i];
                var transform = geo.ModelTransform;
                OnFillVertArray(tempVerts, vertOffset, ref geo, ref transform);
                vertOffset += geo.Geometry.Positions.Count;

                if(IndexBuffer != null && geo.Geometry.Indices != null)
                {
                    //Fill Indices, make sure to correct the offset
                    int count = geo.Geometry.Indices.Count;
                    int tempIdx = indexOffset;
                    for (int j = 0; j < count; ++j, ++tempIdx)
                    {
                        tempIndices[tempIdx] = geo.Geometry.Indices[j] + indexOffset;
                    }
                    indexOffset += geo.Geometry.Indices.Count;
                }                
            }
            VertexBuffer[0].UploadDataToBuffer(deviceContext, tempVerts, tempVerts.Length);
            IndexBuffer?.UploadDataToBuffer(deviceContext, tempIndices, tempIndices.Length);
            vertexBufferBindings = new[] { new VertexBufferBinding(VertexBuffer[0].Buffer, VertexBuffer[0].StructureSize, VertexBuffer[0].Offset) };
        }


        protected abstract void OnFillVertArray(VertStruct[] array, int offset, ref BatchedGeometry geometry, ref Matrix transform);

        /// <summary>
        /// Attaches the buffers.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="vertexLayout">The vertex layout.</param>
        /// <param name="vertexBufferStartSlot">The vertex buffer start slot.</param>
        /// <param name="deviceResources">The device resources.</param>
        /// <returns></returns>
        public bool AttachBuffers(DeviceContextProxy context, InputLayout vertexLayout, ref int vertexBufferStartSlot, IDeviceResources deviceResources)
        {        
            if (!Commit(context) && vertexBufferBindings.Length > 0)
            {
                context.SetVertexBuffers(vertexBufferStartSlot, vertexBufferBindings);
                ++vertexBufferStartSlot;
            }
            else
            {
                return false;
            }
            if (IndexBuffer != null)
            {
                context.SetIndexBuffer(IndexBuffer.Buffer, Format.R32_UInt, IndexBuffer.Offset);
            }
            else
            {
                context.SetIndexBuffer(null, Format.Unknown, 0);
            }
            context.InputLayout = vertexLayout;
            context.PrimitiveTopology = Topology;
            return true;
        }

        protected override void OnDispose(bool disposeManagedResources)
        {
            base.OnDispose(disposeManagedResources);
            OnInvalidateRender = null;           
        }
    }
}
