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
    public abstract class GeometryBufferModel : ResourceSharedObject, IGUID, IGeometryBufferModel
    {
        public Guid GUID { get; } = Guid.NewGuid();

        public event EventHandler<bool> InvalidateRenderer;
        /// <summary>
        /// change flags
        /// </summary>
        protected bool VertexChanged { private set; get; } = true;
        protected bool IndexChanged { private set; get; } = true;

        public IBufferProxy VertexBuffer { private set; get; }
        public IBufferProxy IndexBuffer { private set; get; }
        public PrimitiveTopology Topology { set; get; }

        private Geometry3D geometry = null;
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
        public GeometryBufferModel(PrimitiveTopology topology, IBufferProxy vertexBuffer, IBufferProxy indexBuffer)
        {
            Topology = topology;
            VertexBuffer = Collect(vertexBuffer);
            if (indexBuffer != null)
            { IndexBuffer = Collect(indexBuffer); }
        }

        #endregion

        public void Attach()
        {
            if(AddReference() == 1)
            {
                VertexChanged = true;
                IndexChanged = true;
            }
        }

        public void Detach()
        {
            RemoveReference();
        }


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


        protected virtual bool IsVertexBufferChanged(string propertyName)
        {
            return propertyName.Equals(Geometry3D.VertexBuffer) || propertyName.Equals(nameof(Geometry3D.Positions));
        }

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

        protected abstract void OnCreateVertexBuffer(DeviceContext context, IBufferProxy buffer, Geometry3D geometry);
        protected abstract void OnCreateIndexBuffer(DeviceContext context, IBufferProxy buffer, Geometry3D geometry);

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
        /// Create mesh geometry buffer model with Topology = TriangleList
        /// </summary>
        /// <typeparam name="VertexStruct"></typeparam>
        /// <param name="layout"></param>
        /// <param name="structSize"></param>
        /// <returns></returns>
        public static GeometryBufferModel CreateMeshBufferModel<VertexStruct>(int structSize) where VertexStruct : struct
        {
            return new MeshGeometryBufferModel<VertexStruct>(structSize);
        }
        /// <summary>
        /// Create line geometry buffer model with Topology = LineList
        /// </summary>
        /// <typeparam name="VertexStruct"></typeparam>
        /// <param name="layout"></param>
        /// <param name="structSize"></param>
        /// <returns></returns>
        public static GeometryBufferModel CreateLineBufferModel<VertexStruct>(int structSize) where VertexStruct : struct
        {
            return new LineGeometryBufferModel<VertexStruct>(structSize);
        }
        /// <summary>
        /// Create point geometry buffer model with Topology = PointList
        /// </summary>
        /// <typeparam name="VertexStruct"></typeparam>
        /// <param name="layout"></param>
        /// <param name="structSize"></param>
        /// <returns></returns>
        public static GeometryBufferModel CreatePointBufferModel<VertexStruct>(int structSize) where VertexStruct : struct
        {
            return new PointGeometryBufferModel<VertexStruct>(structSize);
        }
        /// <summary>
        /// Create billboard geometry buffer model with Topology = TriangleStrip
        /// </summary>
        /// <typeparam name="VertexStruct"></typeparam>
        /// <param name="layout"></param>
        /// <param name="structSize"></param>
        /// <returns></returns>
        public static GeometryBufferModel CreateBillboardBufferModel<VertexStruct>(int structSize) where VertexStruct : struct
        {
            return new BillboardBufferModel<VertexStruct>(structSize);
        }
    }
}
