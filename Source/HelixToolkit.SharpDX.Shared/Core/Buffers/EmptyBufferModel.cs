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
    using global::SharpDX.Direct3D;
    using global::SharpDX.Direct3D11;
    using System;
    using System.Collections.Generic;
    using Utilities;
    using Render;
    /// <summary>
    /// 
    /// </summary>
    public sealed class EmptyGeometryBufferModel : IGeometryBufferModel
    {
        public static readonly IGeometryBufferModel Empty = new EmptyGeometryBufferModel();
        /// <summary>
        /// Gets or sets the geometry.
        /// </summary>
        /// <value>
        /// The geometry.
        /// </value>
        public Geometry3D Geometry
        {
            set;get;
        }
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public Guid GUID
        {
            get;
        } = Guid.NewGuid();
        /// <summary>
        /// Gets the index buffer.
        /// </summary>
        /// <value>
        /// The index buffer.
        /// </value>
        public IElementsBufferProxy IndexBuffer
        {
            get
            {
                return null;
            }
        }
        /// <summary>
        /// Gets or sets the topology.
        /// </summary>
        /// <value>
        /// The topology.
        /// </value>
        public PrimitiveTopology Topology
        {
            get
            {
                return PrimitiveTopology.Undefined;
            }
            set { }
        }
        /// <summary>
        /// Gets the vertex buffer.
        /// </summary>
        /// <value>
        /// The vertex buffer.
        /// </value>
        public IElementsBufferProxy[] VertexBuffer
        {
            get;
        } = new IElementsBufferProxy[0];
        /// <summary>
        /// Gets the size of the vertex structure.
        /// </summary>
        /// <value>
        /// The size of the vertex structure.
        /// </value>
        public IEnumerable<int> VertexStructSize { get { yield return 0; } }
        /// <summary>
        /// Gets or sets the effects manager.
        /// </summary>
        /// <value>
        /// The effects manager.
        /// </value>
        public IEffectsManager EffectsManager { set; get; }
#pragma warning disable CS0067
        public event EventHandler OnVertexBufferUpdated;
        public event EventHandler OnIndexBufferUpdated;
#pragma warning restore CS0067
        /// <summary>
        /// Attaches this instance.
        /// </summary>
        public void Attach()
        {

        }
        /// <summary>
        /// Attaches the buffers.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="vertexBufferStartSlot">The vertex buffer start slot. Returns next available bind slot</param>
        /// <param name="deviceResources"></param>
        /// <returns></returns>
        public bool AttachBuffers(DeviceContextProxy context, ref int vertexBufferStartSlot, IDeviceResources deviceResources)
        {
            return true;
        }
        /// <summary>
        /// Attaches the render host.
        /// </summary>
        /// <param name="host">The host.</param>
        public void AttachRenderHost(IRenderHost host)
        {

        }

        /// <summary>
        /// Detaches this instance.
        /// </summary>
        public void Detach()
        {

        }
        /// <summary>
        /// Detaches the render host.
        /// </summary>
        /// <param name="host">The host.</param>
        public void DetachRenderHost(IRenderHost host)
        {

        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {

        }

        public bool UpdateBuffers(DeviceContextProxy context, IDeviceResources deviceResources)
        {
            return false;
        }
    }
}
