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

    /// <summary>
    /// General Geometry Buffer Model.
    /// </summary>
    public class GeometryBufferModel : DisposeObject, IGUID
    {
        public Guid GUID { get; } = Guid.NewGuid();
        public delegate void OnCreateBufferHandler(DeviceContext context, IBufferProxy buffer, Geometry3D geometry);
        public delegate bool OnAttachBufferHanlder(DeviceContext context, InputLayout vertexLayout, InstanceBufferModel instanceModel);
        public delegate void OnDrawHanlder(DeviceContext context, InstanceBufferModel instanceModel);
        public OnCreateBufferHandler OnCreateVertexBuffer;
        public OnCreateBufferHandler OnCreateIndexBuffer;
        public OnAttachBufferHanlder OnAttachBuffer;
        public OnDrawHanlder OnDraw;
        public event EventHandler<bool> InvalidateRenderer;
        /// <summary>
        /// change flags
        /// </summary>
        protected bool VertexChanged { private set; get; } = true;
        protected bool IndexChanged { private set; get; } = true;

        protected IBufferProxy VertexBuffer { private set; get; }
        protected IBufferProxy IndexBuffer { private set; get; }
        public PrimitiveTopology Topology { private set; get; }

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

            OnAttachBuffer = (context, vertexLayout, instanceModel) =>
            {
                if (VertexChanged)
                {
                    OnCreateVertexBuffer?.Invoke(context, VertexBuffer, Geometry);
                    VertexChanged = false;
                }
                if (IndexChanged)
                {
                    OnCreateIndexBuffer?.Invoke(context, IndexBuffer, Geometry);
                    IndexChanged = false;
                }
                context.InputAssembler.InputLayout = vertexLayout;
                context.InputAssembler.PrimitiveTopology = Topology;
                if (IndexBuffer != null)
                {
                    context.InputAssembler.SetIndexBuffer(IndexBuffer.Buffer, Format.R32_UInt, IndexBuffer.Offset * IndexBuffer.StructureSize);
                }
                else
                {
                    context.InputAssembler.SetIndexBuffer(null, Format.Unknown, 0);
                }
                if (VertexBuffer != null)
                {
                    if (instanceModel == null || !instanceModel.HasInstance)
                    {
                        context.InputAssembler.SetVertexBuffers(0, CreateBufferBindings());
                    }
                    else
                    {
                        instanceModel.Attach(context);
                        context.InputAssembler.SetVertexBuffers(0, CreateBufferBindings(instanceModel.InstanceBuffer));
                    }
                }
                return true;
            };
            OnDraw = (context, instanceModel) =>
            {
                if (IndexBuffer != null)
                {
                    if (instanceModel == null || !instanceModel.HasInstance)
                    {
                        context.DrawIndexed(IndexBuffer.Count, IndexBuffer.Offset, 0);
                    }
                    else
                    {
                        context.DrawIndexedInstanced(IndexBuffer.Count, instanceModel.InstanceBuffer.Count, IndexBuffer.Offset, 0, instanceModel.InstanceBuffer.Offset);
                    }
                }
                else if (VertexBuffer != null)
                {
                    if (instanceModel == null || !instanceModel.HasInstance)
                    {
                        context.Draw(VertexBuffer.Count, VertexBuffer.Offset);
                    }
                    else
                    {
                        context.DrawInstanced(VertexBuffer.Count, instanceModel.InstanceBuffer.Count,
                            VertexBuffer.Offset, instanceModel.InstanceBuffer.Offset);
                    }
                }
            };
        }


        public GeometryBufferModel(PrimitiveTopology topology, IBufferProxy vertexBuffer, IBufferProxy indexBuffer,
            OnCreateBufferHandler onCreateVertexBuffer, OnCreateBufferHandler onCreateIndexBuffer)
            : this(topology, vertexBuffer, indexBuffer)
        {
            OnCreateVertexBuffer = onCreateVertexBuffer;
            OnCreateIndexBuffer = onCreateIndexBuffer;
        }

        public GeometryBufferModel(PrimitiveTopology topology, IBufferProxy vertexBuffer, IBufferProxy indexBuffer, 
            OnCreateBufferHandler onCreateVertexBuffer, OnCreateBufferHandler onCreateIndexBuffer, 
            OnAttachBufferHanlder attachBufferHandler, OnDrawHanlder drawHandler) 
            : this(topology, vertexBuffer, indexBuffer, onCreateVertexBuffer, onCreateIndexBuffer)
        {
            OnAttachBuffer = attachBufferHandler;
            OnDraw = drawHandler;
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
        public bool AttachBuffers(DeviceContext context, InputLayout vertexLayout, InstanceBufferModel instanceModel)
        {
            return OnAttachBuffer != null ? OnAttachBuffer.Invoke(context, vertexLayout, instanceModel) : false;
        }

        protected virtual VertexBufferBinding[] CreateBufferBindings(IBufferProxy instanceBuffer = null)
        {
            if (instanceBuffer == null)
            {
                return new[] { new VertexBufferBinding(VertexBuffer.Buffer, VertexBuffer.StructureSize, VertexBuffer.Offset * VertexBuffer.StructureSize) };
            }
            else
            {
                return new[]
                {
                    new VertexBufferBinding(VertexBuffer.Buffer, VertexBuffer.StructureSize, VertexBuffer.Offset*VertexBuffer.StructureSize),
                    new VertexBufferBinding(instanceBuffer.Buffer, instanceBuffer.StructureSize, instanceBuffer.Offset * instanceBuffer.StructureSize)
                };
            }
        }
        /// <summary>
        /// Attach buffers and draw
        /// </summary>
        /// <param name="context"></param>
        /// <param name="instanceModel"></param>
        /// <returns></returns>
        public bool AttachBuffersAndDraw(DeviceContext context, InputLayout vertexLayout, InstanceBufferModel instanceModel)
        {           
            if(AttachBuffers(context, vertexLayout, instanceModel))
            {
                OnDraw?.Invoke(context, instanceModel);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Draw(DeviceContext context, InstanceBufferModel instanceModel)
        {
            OnDraw?.Invoke(context, instanceModel);
        }
        /// <summary>
        /// Create buffer model, contains vertex buffer and index buffer
        /// </summary>
        /// <typeparam name="VertexStruct"></typeparam>
        /// <param name="layout">Input layout for vertex shader binding</param>
        /// <param name="structSize">VertexStruct size by bytes</param>
        /// <returns></returns>
        public static GeometryBufferModel CreateBufferModel<VertexStruct>(PrimitiveTopology topology, int structSize, bool hasIndexBuffer = true) where VertexStruct : struct
        {
            return new GeometryBufferModel(topology, 
                new ImmutableBufferProxy<VertexStruct>(structSize, BindFlags.VertexBuffer), new ImmutableBufferProxy<int>(sizeof(int), BindFlags.VertexBuffer));
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
            return CreateBufferModel<VertexStruct>(PrimitiveTopology.TriangleStrip, structSize);
        }
    }






}
