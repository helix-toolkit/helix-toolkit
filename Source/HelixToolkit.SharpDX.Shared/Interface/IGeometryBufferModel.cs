/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    using Render;
    using System.Collections.Generic;
    using Utilities;
    public interface IAttachableBufferModel : IGUID, IDisposable
    {
        /// <summary>
        /// Gets or sets the topology.
        /// </summary>
        /// <value>
        /// The topology.
        /// </value>
        PrimitiveTopology Topology { set; get; }

        /// <summary>
        /// Gets the vertex buffer.
        /// </summary>
        /// <value>
        /// The vertex buffer.
        /// </value>
        IElementsBufferProxy[] VertexBuffer { get; }
        /// <summary>
        /// Gets the size of the vertex structure.
        /// </summary>
        /// <value>
        /// The size of the vertex structure.
        /// </value>
        IEnumerable<int> VertexStructSize { get; }
        /// <summary>
        /// Gets the index buffer.
        /// </summary>
        /// <value>
        /// The index buffer.
        /// </value>
        IElementsBufferProxy IndexBuffer { get; }
        /// <summary>
        /// Attaches the buffers.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="vertexBufferStartSlot">The vertex buffer slot. It will be changed to next available slot after binding.</param>
        /// <param name="deviceResources"></param>
        /// <returns></returns>
        bool AttachBuffers(DeviceContextProxy context, ref int vertexBufferStartSlot, IDeviceResources deviceResources);
        /// <summary>
        /// Updates the buffers.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceResources">The device resources.</param>
        /// <returns>True if buffer updated.</returns>
        bool UpdateBuffers(DeviceContextProxy context, IDeviceResources deviceResources);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IGeometryBufferModel : IAttachableBufferModel
    {
        event EventHandler VertexBufferUpdated;
        event EventHandler IndexBufferUpdated;
        /// <summary>
        /// Gets or sets the effects manager.
        /// </summary>
        /// <value>
        /// The effects manager.
        /// </value>
        IEffectsManager EffectsManager { set; get; }
        /// <summary>
        /// Gets or sets the geometry.
        /// </summary>
        /// <value>
        /// The geometry.
        /// </value>
        Geometry3D Geometry { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IBillboardBufferModel : IDisposable
    {
        /// <summary>
        /// Gets the texture view.
        /// </summary>
        /// <value>
        /// The texture view.
        /// </value>
        ShaderResourceViewProxy TextureView { get; }
        /// <summary>
        /// Gets the billboard type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        BillboardType Type { get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IBoneSkinMeshBufferModel : IGeometryBufferModel
    {
        event EventHandler BoneIdBufferUpdated;
        IElementsBufferProxy BoneIdBuffer { get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IBoneSkinPreComputehBufferModel
    {
        bool CanPreCompute { get; }
        /// <summary>
        /// Binds the skinned vertex buffer to output.
        /// </summary>
        /// <param name="context">The context.</param>
        void BindSkinnedVertexBufferToOutput(DeviceContextProxy context);
        /// <summary>
        /// Uns the bind skinned vertex buffer to output.
        /// </summary>
        /// <param name="context">The context.</param>
        void UnBindSkinnedVertexBufferToOutput(DeviceContextProxy context);
        /// <summary>
        /// Resets the skinned vertex buffer.
        /// </summary>
        /// <param name="context">The context.</param>
        void ResetSkinnedVertexBuffer(DeviceContextProxy context);
    }
}