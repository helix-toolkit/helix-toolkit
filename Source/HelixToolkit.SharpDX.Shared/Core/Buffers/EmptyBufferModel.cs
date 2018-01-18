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
    using Utilities;
    /// <summary>
    /// 
    /// </summary>
    public sealed class EmptyGeometryBufferModel : IGeometryBufferModel
    {
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
        public IElementsBufferProxy VertexBuffer
        {
            get
            {
                return null;
            }
        }
#pragma warning disable 0067
        /// <summary>
        /// Occurs when [invalidate renderer].
        /// </summary>
        public event EventHandler<bool> InvalidateRenderer;
#pragma warning restore 0067
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
        /// <param name="vertexLayout">The vertex layout.</param>
        /// <param name="vertexBufferSlot">The vertex buffer slot.</param>
        /// <returns></returns>
        public bool AttachBuffers(DeviceContext context, InputLayout vertexLayout, int vertexBufferSlot)
        {
            return true;
        }
        /// <summary>
        /// Detaches this instance.
        /// </summary>
        public void Detach()
        {

        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {

        }
    }
}
