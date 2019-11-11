/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using global::SharpDX.Direct3D;
using global::SharpDX.Direct3D11;
using global::SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

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
        using System.Runtime.InteropServices;
        using Utilities;
        /// <summary>
        /// 
        /// </summary>
        public sealed class BoneSkinnedMeshBufferModel : DefaultMeshGeometryBufferModel, IBoneSkinMeshBufferModel
        {
            public event EventHandler BoneIdBufferUpdated;
            public IElementsBufferProxy BoneIdBuffer { get; private set; }
            private bool boneIdChanged = true;

            public BoneSkinnedMeshBufferModel()
            {
                BoneIdBuffer = Collect(new ImmutableBufferProxy(BoneIds.SizeInBytes, BindFlags.VertexBuffer, ResourceOptionFlags.None));
            }

            protected override bool IsVertexBufferChanged(string propertyName, int bufferIndex)
            {
                if (propertyName.Equals(nameof(BoneSkinnedMeshGeometry3D.VertexBoneIds)))
                {
                    boneIdChanged = true;
                    return false;
                }
                else
                {
                    return base.IsVertexBufferChanged(propertyName, bufferIndex);
                }
            }

            public override bool UpdateBuffers(DeviceContextProxy context, IDeviceResources deviceResources)
            {
                if (boneIdChanged)
                {
                    lock (BoneIdBuffer)
                    {
                        if (boneIdChanged)
                        {
                            if (Geometry is BoneSkinnedMeshGeometry3D boneMesh
                                && boneMesh.VertexBoneIds != null && boneMesh.VertexBoneIds.Count == boneMesh.Positions.Count)
                            {
                                BoneIdBuffer.UploadDataToBuffer(context, boneMesh.VertexBoneIds, boneMesh.VertexBoneIds.Count);
                            }
                            else
                            {
                                BoneIdBuffer.UploadDataToBuffer(context, new BoneIds[0], 0);
                            }
                            boneIdChanged = false;
                            BoneIdBufferUpdated?.Invoke(this, EventArgs.Empty);
                        }
                    }
                }
                return base.UpdateBuffers(context, deviceResources);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class BoneSkinPreComputeBufferModel : DisposeObject, IAttachableBufferModel, IBoneSkinPreComputehBufferModel
        {
            public PrimitiveTopology Topology { get => meshBuffer.Topology; set => meshBuffer.Topology = value; }

            public IElementsBufferProxy[] VertexBuffer { private set; get; } = new IElementsBufferProxy[0];

            public IEnumerable<int> VertexStructSize { get { return VertexBuffer.Select(x => x != null ? x.StructureSize : 0); } }

            public IElementsBufferProxy IndexBuffer => meshBuffer.IndexBuffer;

            public bool CanPreCompute => meshBuffer.BoneIdBuffer.ElementCount != 0;

            public Guid GUID { get; } = new Guid();
            private bool vertexBufferUpdate = true;
            private bool stagingBufferValid = false;
            private readonly IBoneSkinMeshBufferModel meshBuffer;
            private readonly IElementsBufferProxy skinnedVertexBuffer;
            private IElementsBufferProxy originalVertexBuffer;
            private readonly IElementsBufferProxy skinnedVertexStagingBuffer;

            private VertexBufferBinding[] skinnedOutputBindings = new VertexBufferBinding[0];
            private VertexBufferBinding[] vertexBufferBindings = new VertexBufferBinding[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="BoneSkinPreComputeBufferModel"/> class.
            /// </summary>
            /// <param name="meshBuffer">The mesh buffer.</param>
            /// <param name="structSize">Size of the structure.</param>
            public BoneSkinPreComputeBufferModel(IBoneSkinMeshBufferModel meshBuffer, int structSize)
            {
                this.meshBuffer = Collect(meshBuffer);
                this.meshBuffer.VertexBufferUpdated += MeshBuffer_OnVertexBufferUpdated;
                this.meshBuffer.BoneIdBufferUpdated += MeshBuffer_OnBoneIdBufferUpdated;
                skinnedVertexBuffer = Collect(new ImmutableBufferProxy(structSize,
                    BindFlags.VertexBuffer | BindFlags.StreamOutput,
                    ResourceOptionFlags.None, ResourceUsage.Default));
                skinnedVertexStagingBuffer = Collect(new ImmutableBufferProxy(structSize,
                    BindFlags.None, CpuAccessFlags.Read, ResourceOptionFlags.BufferStructured,
                    ResourceUsage.Staging));
            }

            private void MeshBuffer_OnBoneIdBufferUpdated(object sender, EventArgs e)
            {
                if (originalVertexBuffer != null)
                {
                    skinnedOutputBindings = new VertexBufferBinding[]
                    {
                        new VertexBufferBinding(originalVertexBuffer.Buffer, originalVertexBuffer.StructureSize, originalVertexBuffer.Offset),
                        new VertexBufferBinding(meshBuffer.BoneIdBuffer.Buffer, meshBuffer.BoneIdBuffer.StructureSize, meshBuffer.BoneIdBuffer.Offset)
                    };
                }
            }

            private void MeshBuffer_OnVertexBufferUpdated(object sender, EventArgs e)
            {
                vertexBufferUpdate = true;
                stagingBufferValid = false;
            }

            /// <summary>
            /// Binds the skinned vertex buffer to output.
            /// </summary>
            /// <param name="context">The context.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void BindSkinnedVertexBufferToOutput(DeviceContextProxy context)
            {
                context.SetVertexBuffers(0, skinnedOutputBindings);
                context.SetIndexBuffer(null, Format.Unknown, 0);
                context.SetStreamOutputTarget(skinnedVertexBuffer.Buffer, skinnedVertexBuffer.Offset);
                stagingBufferValid = false;
            }

            /// <summary>
            /// Uns the bind skinned vertex buffer to gs stream output.
            /// </summary>
            /// <param name="context">The context.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UnBindSkinnedVertexBufferToOutput(DeviceContextProxy context)
            {
                context.SetStreamOutputTarget(null);
            }

            /// <summary>
            /// Attaches the buffers.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="vertexBufferStartSlot">The vertex buffer start slot.</param>
            /// <param name="deviceResources">The device resources.</param>
            /// <returns></returns>
            public bool AttachBuffers(DeviceContextProxy context, ref int vertexBufferStartSlot, IDeviceResources deviceResources)
            {
                UpdateBuffers(context, deviceResources);
                if (VertexBuffer.Length > 0)
                {
                    if (VertexBuffer.Length == vertexBufferBindings.Length)
                    {
                        context.SetVertexBuffers(vertexBufferStartSlot, vertexBufferBindings);
                        vertexBufferStartSlot += VertexBuffer.Length;
                    }
                    else
                    {
                        return false;
                    }
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

            /// <summary>
            /// Updates the buffers.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="deviceResources">The device resources.</param>
            /// <returns></returns>
            public bool UpdateBuffers(DeviceContextProxy context, IDeviceResources deviceResources)
            {
                bool updated = false;
                if (meshBuffer.UpdateBuffers(context, deviceResources) || vertexBufferUpdate)
                {
                    lock (skinnedVertexBuffer)
                    {
                        if (vertexBufferUpdate)
                        {
                            if (meshBuffer.VertexBuffer.Length > 0)
                            {
                                VertexBuffer = meshBuffer.VertexBuffer.ToArray();
                                originalVertexBuffer = VertexBuffer[0];
                                if (skinnedVertexBuffer.Buffer == null || skinnedVertexBuffer.ElementCount != originalVertexBuffer.ElementCount)
                                {
                                    var array = new float[originalVertexBuffer.ElementCount * originalVertexBuffer.StructureSize];
                                    skinnedVertexBuffer.UploadDataToBuffer(context, array, originalVertexBuffer.ElementCount);
                                    context.CopyResource(originalVertexBuffer.Buffer, skinnedVertexBuffer.Buffer);
                                }
                                VertexBuffer[0] = skinnedVertexBuffer;
                                vertexBufferBindings = VertexBuffer.Select(x => x != null ? new VertexBufferBinding(x.Buffer, x.StructureSize, x.Offset) : new VertexBufferBinding()).ToArray();
                                skinnedOutputBindings = new VertexBufferBinding[]
                                {
                                    new VertexBufferBinding(originalVertexBuffer.Buffer, originalVertexBuffer.StructureSize, originalVertexBuffer.Offset),
                                    new VertexBufferBinding(meshBuffer.BoneIdBuffer.Buffer, meshBuffer.BoneIdBuffer.StructureSize, meshBuffer.BoneIdBuffer.Offset)
                                };
                            }
                            else
                            {
                                VertexBuffer = new IElementsBufferProxy[0];
                                vertexBufferBindings = new VertexBufferBinding[0];
                            }
                            vertexBufferUpdate = false;
                            updated = true;
                            stagingBufferValid = false;
                        }
                    }
                }
                return updated;
            }

            public void ResetSkinnedVertexBuffer(DeviceContextProxy context)
            {
                if (skinnedVertexBuffer.Buffer != null && skinnedVertexBuffer.ElementCount == originalVertexBuffer.ElementCount)
                {
                    context.CopyResource(originalVertexBuffer.Buffer, skinnedVertexBuffer.Buffer);
                }
            }
            /// <summary>
            /// Copies the skinned to array.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="array">The array.</param>
            /// <returns>Number of vertex has been copied.</returns>
            public int CopySkinnedToArray(DeviceContextProxy context, global::SharpDX.Vector3[] array)
            {
                if (skinnedVertexBuffer.Buffer == null)
                {
                    return 0;
                }
                if (skinnedVertexStagingBuffer.Buffer == null || skinnedVertexStagingBuffer.ElementCount != skinnedVertexBuffer.ElementCount)
                {
                    skinnedVertexStagingBuffer.CreateBuffer(context, skinnedVertexBuffer.ElementCount);
                    stagingBufferValid = false;
                }
                if (skinnedVertexStagingBuffer.Buffer != null)
                {
                    var size = Math.Min(array.Length, skinnedVertexStagingBuffer.ElementCount);
                    if (!stagingBufferValid)
                    {
                        context.CopyResource(skinnedVertexBuffer.Buffer, skinnedVertexStagingBuffer.Buffer);
                        stagingBufferValid = true;
                    }
                    var box = context.MapSubresource(skinnedVertexStagingBuffer.Buffer, MapMode.Read,
                        global::SharpDX.Direct3D11.MapFlags.None, out var stream);
                    using (stream)
                    {
                        unsafe
                        {
                            byte* p = (byte*)box.DataPointer;
                            for (var i = 0; i < size; ++i)
                            {
                                array[i] = *(global::SharpDX.Vector3*)p;
                                p += skinnedVertexStagingBuffer.StructureSize;
                            }
                        }
                        return size;
                    }
                }
                return 0;
            }

            protected override void OnDispose(bool disposeManagedResources)
            {
                meshBuffer.BoneIdBufferUpdated -= MeshBuffer_OnBoneIdBufferUpdated;
                meshBuffer.VertexBufferUpdated -= MeshBuffer_OnVertexBufferUpdated;
                base.OnDispose(disposeManagedResources);
            }
        }
    }

}
