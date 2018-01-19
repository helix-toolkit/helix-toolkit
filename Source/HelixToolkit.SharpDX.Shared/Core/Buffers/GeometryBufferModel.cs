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
    using Utilities;
    using global::SharpDX.DXGI;
    using System;
    using System.Linq;
    /// <summary>
    /// General Geometry Buffer Model.
    /// </summary>
    public abstract class GeometryBufferModel : DisposeObject, IGUID, IGeometryBufferModel
    {
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public Guid GUID { get; } = Guid.NewGuid();
        /// <summary>
        /// Occurs when [invalidate renderer].
        /// </summary>
        public event EventHandler<bool> InvalidateRenderer;
        /// <summary>
        /// change flags
        /// </summary>
        protected bool VertexChanged { private set; get; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether [index changed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [index changed]; otherwise, <c>false</c>.
        /// </value>
        protected bool IndexChanged { private set; get; } = true;
        /// <summary>
        /// Gets or sets the vertex buffer.
        /// </summary>
        /// <value>
        /// The vertex buffer.
        /// </value>
        public IElementsBufferProxy VertexBuffer { private set; get; }

        /// <summary>
        /// Gets the size of the vertex structure.
        /// </summary>
        /// <value>
        /// The size of the vertex structure.
        /// </value>
        public int VertexStructSize { get { return VertexBuffer.StructureSize; } }
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

        private Geometry3D geometry = null;
        /// <summary>
        /// Gets or sets the geometry.
        /// </summary>
        /// <value>
        /// The geometry.
        /// </value>
        public Geometry3D Geometry
        {
            set
            {
                if (geometry == value)
                { return; }
                if (geometry != null)
                {
                    geometry.PropertyChanged -= Geometry_PropertyChanged;
                }
                geometry = value;
                if (geometry != null)
                {
                    geometry.PropertyChanged += Geometry_PropertyChanged;
                }
                VertexChanged = true;
                IndexChanged = true;
                InvalidateRenderer?.Invoke(this, true);
            }
            get
            {
                return geometry;
            }
        }

        #region Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryBufferModel"/> class.
        /// </summary>
        /// <param name="topology">The topology.</param>
        /// <param name="vertexBuffer">The vertex buffer.</param>
        /// <param name="indexBuffer">The index buffer.</param>
        protected GeometryBufferModel(PrimitiveTopology topology, IElementsBufferProxy vertexBuffer, IElementsBufferProxy indexBuffer)
        {
            Topology = topology;
            VertexBuffer = Collect(vertexBuffer);
            if (indexBuffer != null)
            { IndexBuffer = Collect(indexBuffer); }
        }

        #endregion


        private void Geometry_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (IsVertexBufferChanged(e.PropertyName))
            {
                VertexChanged = true;
                InvalidateRenderer?.Invoke(this, true);
            }
            else if (IsIndexBufferChanged(e.PropertyName))
            {
                IndexChanged = true;
                InvalidateRenderer?.Invoke(this, true);
            }
        }

        /// <summary>
        /// Determines whether [is vertex buffer changed] [the specified property name].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        ///   <c>true</c> if [is vertex buffer changed] [the specified property name]; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsVertexBufferChanged(string propertyName)
        {
            return propertyName.Equals(Geometry3D.VertexBuffer) || propertyName.Equals(nameof(Geometry3D.Positions));
        }
        /// <summary>
        /// Determines whether [is index buffer changed] [the specified property name].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        ///   <c>true</c> if [is index buffer changed] [the specified property name]; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsIndexBufferChanged(string propertyName)
        {
            return propertyName.Equals(Geometry3D.TriangleBuffer) || propertyName.Equals(nameof(Geometry3D.Indices));
        }
        /// <summary>
        /// Attach buffers only
        /// </summary>
        /// <param name="context"></param>
        /// <param name="instanceModel"></param>
        /// <returns></returns>
        public bool AttachBuffers(DeviceContext context, InputLayout vertexLayout, int vertexBufferSlot)
        {
            if (VertexChanged)
            {
                OnCreateVertexBuffer(context, VertexBuffer, Geometry);
                VertexChanged = false;
            }
            if (IndexChanged)
            {
                OnCreateIndexBuffer(context, IndexBuffer, Geometry);
                IndexChanged = false;
            }
            return OnAttachBuffer(context, vertexLayout, vertexBufferSlot);
        }
        /// <summary>
        /// Called when [create vertex buffer].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="geometry">The geometry.</param>
        protected abstract void OnCreateVertexBuffer(DeviceContext context, IElementsBufferProxy buffer, Geometry3D geometry);
        /// <summary>
        /// Called when [create index buffer].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="geometry">The geometry.</param>
        protected abstract void OnCreateIndexBuffer(DeviceContext context, IElementsBufferProxy buffer, Geometry3D geometry);
        /// <summary>
        /// Called when [attach buffer].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="vertexLayout">The vertex layout.</param>
        /// <param name="vertexBufferSlot">The vertex buffer slot.</param>
        /// <returns></returns>
        protected virtual bool OnAttachBuffer(DeviceContext context, InputLayout vertexLayout, int vertexBufferSlot)
        {
            context.InputAssembler.InputLayout = vertexLayout;
            context.InputAssembler.PrimitiveTopology = Topology;
            if (IndexBuffer != null)
            {
                context.InputAssembler.SetIndexBuffer(IndexBuffer.Buffer, Format.R32_UInt, IndexBuffer.Offset);
            }
            else
            {
                context.InputAssembler.SetIndexBuffer(null, Format.Unknown, 0);
            }
            if (VertexBuffer != null)
            {
                context.InputAssembler.SetVertexBuffers(vertexBufferSlot, new VertexBufferBinding(VertexBuffer.Buffer, VertexBuffer.StructureSize, VertexBuffer.Offset));
            }
            return true;
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposeManagedResources"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposeManagedResources)
        {
            InvalidateRenderer = null;
            Geometry = null;// Release all events
            base.Dispose(disposeManagedResources);
        }
    }
}
