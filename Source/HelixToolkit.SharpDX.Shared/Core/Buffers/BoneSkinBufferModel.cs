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
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;
    using Utilities;

    /// <summary>
    /// 
    /// </summary>
    public sealed class BoneSkinMeshBufferModel : DisposeObject, IAttachableBufferModel, IBoneSkinMeshBufferModel
    {
        public PrimitiveTopology Topology { get => MeshBuffer.Topology; set => MeshBuffer.Topology = value; }

        public IElementsBufferProxy[] VertexBuffer { private set; get; } = new IElementsBufferProxy[0];

        public IEnumerable<int> VertexStructSize { get { return VertexBuffer.Select(x => x != null ? x.StructureSize : 0); } }

        public IElementsBufferProxy IndexBuffer => MeshBuffer.IndexBuffer;

        public Guid GUID { get; } = new Guid();
        private bool vertexBoneIdUpdated = true;
        private bool vertexBufferUpdate = true;
        private readonly GeometryBufferModel MeshBuffer;
        private IElementsBufferProxy skinnedVertexBuffer;
        private IElementsBufferProxy boneIdBuffer;
        private IElementsBufferProxy originalVertexBuffer;
        private VertexBufferBinding[] skinnedOutputBindings = new VertexBufferBinding[0];
        private VertexBufferBinding[] VertexBufferBindings = new VertexBufferBinding[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="BoneSkinMeshBufferModel"/> class.
        /// </summary>
        /// <param name="meshBuffer">The mesh buffer.</param>
        /// <param name="structSize">Size of the structure.</param>
        public BoneSkinMeshBufferModel(GeometryBufferModel meshBuffer, int structSize)
        {
            MeshBuffer = Collect(meshBuffer);
            if (MeshBuffer.Geometry != null)
            { MeshBuffer.Geometry.PropertyChanged += Geometry_PropertyChanged; }
            MeshBuffer.OnVertexBufferUpdated += MeshBuffer_OnVertexBufferUpdated;
            skinnedVertexBuffer = Collect(new ImmutableBufferProxy(structSize, BindFlags.VertexBuffer | BindFlags.StreamOutput, ResourceOptionFlags.None, ResourceUsage.Default));
            boneIdBuffer = Collect(new ImmutableBufferProxy(BoneIds.SizeInBytes, BindFlags.VertexBuffer, ResourceOptionFlags.None));
        }

        private void Geometry_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(BoneSkinnedMeshGeometry3D.VertexBoneIds)))
            {
                vertexBoneIdUpdated = true;
            }
        }

        private void MeshBuffer_OnVertexBufferUpdated(object sender, EventArgs e)
        {
            vertexBufferUpdate = true;
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
                if (VertexBuffer.Length == VertexBufferBindings.Length)
                {
                    context.SetVertexBuffers(vertexBufferStartSlot, VertexBufferBindings);
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
            if(MeshBuffer.UpdateBuffers(context, deviceResources) || vertexBufferUpdate)
            {
                lock (skinnedVertexBuffer)
                {
                    if (vertexBufferUpdate)
                    {
                        if (MeshBuffer.VertexBuffer.Length > 0)
                        {
                            VertexBuffer = MeshBuffer.VertexBuffer.ToArray();
                            originalVertexBuffer = VertexBuffer[0];
                            if (skinnedVertexBuffer.Buffer == null || skinnedVertexBuffer.ElementCount != originalVertexBuffer.ElementCount)
                            {
                                skinnedVertexBuffer.UploadDataToBuffer(context, new float[originalVertexBuffer.ElementCount * originalVertexBuffer.StructureSize], originalVertexBuffer.ElementCount);
                            }
                            VertexBuffer[0] = skinnedVertexBuffer;
                            VertexBufferBindings = VertexBuffer.Select(x => x != null ? new VertexBufferBinding(x.Buffer, x.StructureSize, x.Offset) : new VertexBufferBinding()).ToArray();
                        }
                        else
                        {
                            VertexBuffer = new IElementsBufferProxy[0];
                            VertexBufferBindings = new VertexBufferBinding[0];
                        }
                        vertexBufferUpdate = false;
                        updated = true;
                    }
                }
            }
            if (vertexBoneIdUpdated)
            {
                lock (boneIdBuffer)
                {
                    if (vertexBoneIdUpdated)
                    {
                        if (MeshBuffer.Geometry is BoneSkinnedMeshGeometry3D boneMesh && boneMesh.VertexBoneIds != null && boneMesh.VertexBoneIds.Count == MeshBuffer.Geometry.Positions.Count)
                        {
                            boneIdBuffer.UploadDataToBuffer(context, boneMesh.VertexBoneIds, boneMesh.VertexBoneIds.Count);
                        }
                        else
                        {
                            boneIdBuffer.UploadDataToBuffer(context, new BoneIds[0], 0);
                        }
                        skinnedOutputBindings = new VertexBufferBinding[]
                        {
                            new VertexBufferBinding(originalVertexBuffer.Buffer, originalVertexBuffer.StructureSize, originalVertexBuffer.Offset),
                            new VertexBufferBinding(boneIdBuffer.Buffer, boneIdBuffer.StructureSize, boneIdBuffer.Offset)
                        };
                        vertexBoneIdUpdated = false;
                        updated = true;
                    }
                }               
            }
            return updated;
        }

        protected override void OnDispose(bool disposeManagedResources)
        {
            if (MeshBuffer.Geometry != null)
            { MeshBuffer.Geometry.PropertyChanged -= Geometry_PropertyChanged; }
            MeshBuffer.OnVertexBufferUpdated -= MeshBuffer_OnVertexBufferUpdated;
            base.OnDispose(disposeManagedResources);        
        }
    }
}
