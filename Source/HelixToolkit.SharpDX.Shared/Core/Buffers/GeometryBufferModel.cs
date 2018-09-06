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
    using global::SharpDX.DXGI;
    using Render;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Utilities;

    /// <summary>
    /// General Geometry Buffer Model.
    /// </summary>
    public abstract class GeometryBufferModel : ReferenceCountDisposeObject, IGUID, IGeometryBufferModel
    {
        public event EventHandler VertexBufferUpdated;
        public event EventHandler IndexBufferUpdated;
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public Guid GUID { get; } = Guid.NewGuid();

        /// <summary>
        /// change flags
        /// </summary>
        protected volatile uint VertexChanged;
        /// <summary>
        /// Gets or sets a value indicating whether [index changed].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [index changed]; otherwise, <c>false</c>.
        /// </value>
        protected volatile bool IndexChanged = true;
        /// <summary>
        /// Gets or sets the vertex buffer.
        /// </summary>
        /// <value>
        /// The vertex buffer.
        /// </value>
        public IElementsBufferProxy[] VertexBuffer { private set; get; } = new IElementsBufferProxy[0];
        private static readonly VertexBufferBinding[] emptyBinding = new VertexBufferBinding[0];
        protected VertexBufferBinding[] VertexBufferBindings { private set; get; } = emptyBinding;
        /// <summary>
        /// Gets the size of the vertex structure.
        /// </summary>
        /// <value>
        /// The size of the vertex structure.
        /// </value>
        public IEnumerable<int> VertexStructSize { get { return VertexBuffer.Select(x=> x != null ? x.StructureSize : 0); } }
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
                for(int i = 0; i < VertexBuffer.Length; ++i)
                {
                    VertexChanged |= 1u << i;
                }
                IndexChanged = true;
                InvalidateRenderer();
            }
            get
            {
                return geometry;
            }
        }

        public IEffectsManager EffectsManager { set; get; }

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
            VertexBuffer = new IElementsBufferProxy[] { Collect(vertexBuffer) };
            VertexChanged = 1u;
            if (indexBuffer != null)
            { IndexBuffer = Collect(indexBuffer); }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryBufferModel"/> class.
        /// </summary>
        /// <param name="topology">The topology.</param>
        /// <param name="vertexBuffer">The vertex buffer.</param>
        /// <param name="indexBuffer">The index buffer.</param>
        protected GeometryBufferModel(PrimitiveTopology topology, IElementsBufferProxy[] vertexBuffer, IElementsBufferProxy indexBuffer)
        {
            Topology = topology;
            for(int i = 0; i < vertexBuffer.Length; ++i)
            {
                Collect(vertexBuffer[i]);
                VertexChanged |= 1u << i;
            }
            VertexBuffer = vertexBuffer;
            if (indexBuffer != null)
            { IndexBuffer = Collect(indexBuffer); }
        }
        #endregion


        private void Geometry_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            bool vertChanged = false;
            for(int i = 0; i < VertexBuffer.Length; ++i)
            {
                if (IsVertexBufferChanged(e.PropertyName, i))
                {
                    VertexChanged |= 1u << i;
                    InvalidateRenderer();
                    vertChanged = true;
                    break;
                }
            }
            if (!vertChanged && IsIndexBufferChanged(e.PropertyName))
            {
                IndexChanged = true;
                InvalidateRenderer();
            }
        }
        /// <summary>
        /// Invalidates the renderer.
        /// </summary>
        protected void InvalidateRenderer()
        {
            EffectsManager?.RaiseInvalidateRender();
        }
        /// <summary>
        /// Determines whether [is vertex buffer changed] [the specified property name].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="vertexBufferIndex"></param>
        /// <returns>
        ///   <c>true</c> if [is vertex buffer changed] [the specified property name]; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsVertexBufferChanged(string propertyName,  int vertexBufferIndex)
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
        /// Attaches the buffers.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="vertexBufferStartSlot">The vertex buffer slot.</param>
        /// <param name="deviceResources">The device resources.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AttachBuffers(DeviceContextProxy context, ref int vertexBufferStartSlot, IDeviceResources deviceResources)
        {
            UpdateBuffers(context, deviceResources);
            return OnAttachBuffer(context, ref vertexBufferStartSlot);
        }
        /// <summary>
        /// Updates the buffers.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceResources">The device resources.</param>
        /// <returns></returns>
        public virtual bool UpdateBuffers(DeviceContextProxy context, IDeviceResources deviceResources)
        {
            bool bufferUpdated = false;
            if(VertexChanged != 0)
            {
                lock (VertexBuffer)
                {
                    bool updateVBinding = false;
                    if (VertexChanged != 0)
                    {
                        for(int i = 0; i < VertexBuffer.Length && VertexChanged != 0; ++i)
                        {
                            if((VertexChanged & (1u << i)) != 0)
                            {
                                if(VertexBuffer[i] != null)
                                {
                                    OnCreateVertexBuffer(context, VertexBuffer[i], i, Geometry, deviceResources);
                                }
                                VertexChanged &= ~(1u << i);
                                updateVBinding = true;  
                            }
                        }      
                    }  
                    if (updateVBinding)
                    {
                        VertexBufferBindings = OnCreateVertexBufferBinding();
                        updateVBinding = false;
                        VertexBufferUpdated?.Invoke(this, EventArgs.Empty);
                        bufferUpdated = true;
                    }
                }
            }
            if (IndexChanged && IndexBuffer != null)
            {
                lock (IndexBuffer)
                {
                    if (IndexChanged)
                    {
                        OnCreateIndexBuffer(context, IndexBuffer, Geometry, deviceResources);
                        bufferUpdated = true;
                    }
                    IndexChanged = false;                    
                    IndexBufferUpdated?.Invoke(this, EventArgs.Empty);
                }               
            }
            return bufferUpdated;
        }

        protected virtual VertexBufferBinding[] OnCreateVertexBufferBinding()
        {
            return VertexBuffer.Select(x => x != null ? new VertexBufferBinding(x.Buffer, x.StructureSize, x.Offset) : new VertexBufferBinding()).ToArray();
        }
        /// <summary>
        /// Called when [create vertex buffer].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="geometry">The geometry.</param>
        /// <param name="deviceResources">The device resources.</param>
        /// <param name="bufferIndex"></param>
        protected abstract void OnCreateVertexBuffer(DeviceContextProxy context, IElementsBufferProxy buffer, int bufferIndex, Geometry3D geometry, IDeviceResources deviceResources);
        /// <summary>
        /// Called when [create index buffer].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="geometry">The geometry.</param>
        /// <param name="deviceResources">The device resources.</param>
        protected abstract void OnCreateIndexBuffer(DeviceContextProxy context, IElementsBufferProxy buffer, Geometry3D geometry, IDeviceResources deviceResources);
        /// <summary>
        /// Called when [attach buffer].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="vertexBufferStartSlot">The vertex buffer start slot. It will be changed to the next available slot after binding</param>
        /// <returns></returns>
        protected virtual bool OnAttachBuffer(DeviceContextProxy context, ref int vertexBufferStartSlot)
        {
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
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposeManagedResources"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void OnDispose(bool disposeManagedResources)
        {
            if (geometry != null)
            {
                geometry.PropertyChanged -= Geometry_PropertyChanged;
            }
            geometry = null;
            VertexBufferBindings = emptyBinding;
            base.OnDispose(disposeManagedResources);
        }
    }
}
