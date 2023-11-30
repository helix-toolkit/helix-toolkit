//#define OutputBuildTime
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Core
    {        
        using Render;
        using Utilities;
        public interface IBatchedGeometry
        {
            Geometry3D Geometry
            {
                get;
            }
            Matrix ModelTransform
            {
                get;
            }
        }

        public abstract class StaticGeometryBatchingBufferBase<BatchedGeometry, VertStruct> : DisposeObject, IAttachableBufferModel
            where BatchedGeometry : struct, IBatchedGeometry where VertStruct : unmanaged
        {
            static readonly ILogger logger = Logger.LogManager.Create<StaticGeometryBatchingBufferBase<BatchedGeometry, VertStruct>>();
            public Guid GUID { get; } = Guid.NewGuid();
            public event EventHandler<EventArgs> InvalidateRender;
            private bool isGeometryChanged = true;
            private static readonly VertStruct[] EmptyArray = new VertStruct[0];
            private static readonly int[] EmptyIntArray = new int[0];
            private static readonly IElementsBufferProxy[] emptyBuffer = new IElementsBufferProxy[0];
            private static readonly VertexBufferBinding[] emptyBindings = new VertexBufferBinding[0];

            private IElementsBufferProxy[] vertexBuffers = emptyBuffer;
            /// <summary>
            /// Gets or sets the vertex buffer.
            /// </summary>
            /// <value>
            /// The vertex buffer.
            /// </value>
            public IElementsBufferProxy[] VertexBuffer => vertexBuffers;
            public IEnumerable<int> VertexStructSize
            {
                get
                {
                    return VertexBuffer.Select(x => x != null ? x.StructureSize : 0);
                }
            }

            private VertexBufferBinding[] vertexBufferBindings = emptyBindings;

            private IElementsBufferProxy indexBuffer = null;
            /// <summary>
            /// Gets or sets the index buffer.
            /// </summary>
            /// <value>
            /// The index buffer.
            /// </value>
            public IElementsBufferProxy IndexBuffer => indexBuffer;
            /// <summary>
            /// Gets or sets the topology.
            /// </summary>
            /// <value>
            /// The topology.
            /// </value>
            public PrimitiveTopology Topology
            {
                set; get;
            }

            private BatchedGeometry[] geometries;
            public BatchedGeometry[] Geometries
            {
                set
                {
                    if (Set(ref geometries, value))
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
                InvalidateRender?.Invoke(this, EventArgs.Empty);
            }

            public StaticGeometryBatchingBufferBase(PrimitiveTopology topology, IElementsBufferProxy vertexBuffer, IElementsBufferProxy indexBuffer)
            {
                Topology = topology;
                vertexBuffers = new IElementsBufferProxy[] { vertexBuffer };
                this.indexBuffer = indexBuffer;
            }

            public bool Commit(DeviceContextProxy deviceContext)
            {
                if (isGeometryChanged)
                {
                    lock (VertexBuffer)
                    {
                        if (isGeometryChanged)
                        {
                            OnSubmitGeometries(deviceContext);
                            isGeometryChanged = false;
                            InvalidateRender?.Invoke(this, EventArgs.Empty);
                            return true;
                        }
                    }
                }
                return false;
            }

            protected virtual void OnSubmitGeometries(DeviceContextProxy deviceContext)
            {
                if (Geometries == null)
                {
                    VertexBuffer[0].UploadDataToBuffer(deviceContext, EmptyArray, 0);
                    IndexBuffer?.UploadDataToBuffer(deviceContext, EmptyIntArray, 0);
                    vertexBufferBindings = new VertexBufferBinding[0];
                    return;
                }
#if OutputBuildTime
                var time = System.Diagnostics.Stopwatch.GetTimestamp();
#endif
                var totalVertex = 0;
                var totalIndices = 0;
                var vertRange = new int[Geometries.Length];
                var idxRange = new int[Geometries.Length];
                for (var i = 0; i < Geometries.Length; ++i)
                {
                    vertRange[i] = totalVertex;
                    totalVertex += Geometries[i].Geometry.Positions.Count;
                    if (Geometries[i].Geometry.Indices != null)
                    {
                        idxRange[i] = totalIndices;
                        totalIndices += Geometries[i].Geometry.Indices.Count;
                    }
                }

                var tempVerts = new VertStruct[totalVertex];
                var tempIndices = new int[totalIndices];
                if (Geometries.Length > 50 && totalVertex > 5000)
                {
                    var partitionParams = Partitioner.Create(0, Geometries.Length);
                    Parallel.ForEach(partitionParams, (range) =>
                    {
                        for (var i = range.Item1; i < range.Item2; ++i)
                        {
                            var geo = Geometries[i];
                            var transform = geo.ModelTransform;
                            var vertStart = vertRange[i];
                            OnFillVertArray(tempVerts, vertStart, ref geo, ref transform);

                            if (IndexBuffer != null && geo.Geometry.Indices != null)
                            {
                                //Fill Indices, make sure to correct the offset
                                var count = geo.Geometry.Indices.Count;
                                var tempIdx = idxRange[i];
                                for (var j = 0; j < count; ++j, ++tempIdx)
                                {
                                    tempIndices[tempIdx] = geo.Geometry.Indices[j] + vertStart;
                                }
                            }
                        }
                    });
                }
                else
                {
                    var vertOffset = 0;
                    var indexOffset = 0;
                    for (var i = 0; i < Geometries.Length; ++i)
                    {
                        var geo = Geometries[i];
                        var transform = geo.ModelTransform;
                        OnFillVertArray(tempVerts, vertOffset, ref geo, ref transform);

                        if (IndexBuffer != null && geo.Geometry.Indices != null)
                        {
                            //Fill Indices, make sure to correct the offset
                            var count = geo.Geometry.Indices.Count;
                            var tempIdx = indexOffset;
                            for (var j = 0; j < count; ++j, ++tempIdx)
                            {
                                tempIndices[tempIdx] = geo.Geometry.Indices[j] + vertOffset;
                            }
                            indexOffset += geo.Geometry.Indices.Count;
                        }
                        vertOffset += geo.Geometry.Positions.Count;
                    }
                }
#if OutputBuildTime
                time = System.Diagnostics.Stopwatch.GetTimestamp() - time;
                logger.LogDebug($"Build Batch Time: {0} ms", (float)time / System.Diagnostics.Stopwatch.Frequency * 1000);
#endif
                VertexBuffer[0].UploadDataToBuffer(deviceContext, tempVerts, tempVerts.Length);
                IndexBuffer?.UploadDataToBuffer(deviceContext, tempIndices, tempIndices.Length);
                vertexBufferBindings = new[] { new VertexBufferBinding(VertexBuffer[0].Buffer, VertexBuffer[0].StructureSize, VertexBuffer[0].Offset) };
            }


            protected abstract void OnFillVertArray(VertStruct[] array, int offset, ref BatchedGeometry geometry, ref Matrix transform);

            /// <summary>
            /// Attaches the buffers.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="vertexBufferStartSlot">The vertex buffer start slot.</param>
            /// <param name="deviceResources">The device resources.</param>
            /// <returns></returns>
            public bool AttachBuffers(DeviceContextProxy context, ref int vertexBufferStartSlot, IDeviceResources deviceResources)
            {
                Commit(context);
                if (vertexBufferBindings.Length > 0)
                {
                    context.SetVertexBuffers(vertexBufferStartSlot, vertexBufferBindings);
                    vertexBufferStartSlot += vertexBufferBindings.Length;
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
                context.PrimitiveTopology = Topology;
                return true;
            }

            public bool UpdateBuffers(DeviceContextProxy context, IDeviceResources deviceResources)
            {
                return false;
            }

            protected override void OnDispose(bool disposeManagedResources)
            {
                for (var i = 0; i < vertexBuffers.Length; ++i)
                {
                    RemoveAndDispose(ref vertexBuffers[i]);
                }
                RemoveAndDispose(ref indexBuffer);
                base.OnDispose(disposeManagedResources);
                InvalidateRender = null;
            }
        }
    }
}
