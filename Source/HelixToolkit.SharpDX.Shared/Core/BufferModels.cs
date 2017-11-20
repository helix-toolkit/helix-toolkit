#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using global::SharpDX;
    using global::SharpDX.Direct3D;
    using global::SharpDX.Direct3D11;
    using Utilities;
    using global::SharpDX.DXGI;
    using System.Linq;
    using System.Collections.Generic;
    using System;

    public class InstanceBufferModel : DisposeObject, IGUID
    {
        public System.Guid GUID { get; } = System.Guid.NewGuid();
        public bool Initialized { private set; get; }
        public bool HasInstance { set; get; } = false;
        public DynamicBufferProxy<Matrix> InstanceBuffer { private set; get; }

        private EffectScalarVariable hasInstancesVar;

        private bool instanceChanged = true;

        private IList<Matrix> instances = null;
        public IList<Matrix> Instances
        {
            set
            {
                if(instances != value)
                {
                    instances = value;
                    instanceChanged = true;
                    HasInstance = instances != null && instances.Any();
                }
            }
            get { return instances; }
        }

        public void Initialize(Effect effect)
        {
            hasInstancesVar = Collect(effect.GetVariableByName("bHasInstances").AsScalar());
            InstanceBuffer = Collect(new DynamicBufferProxy<Matrix>(Matrix.SizeInBytes, BindFlags.VertexBuffer));
            Initialized = true;
        }

        public void Attach(DeviceContext context)
        {
            hasInstancesVar.Set(HasInstance);
            if (HasInstance && instanceChanged)
            {
                InstanceBuffer.UploadDataToBuffer(context, instances);
                instanceChanged = false;
            }
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            Initialized = false;
            base.Dispose(disposeManagedResources);
        }
    }

    public class GeometryBufferModel : DisposeObject, IGUID
    {
        public Guid GUID { get; } = Guid.NewGuid();
        public delegate void OnCreateBufferHandler(DeviceContext context, IBufferProxy buffer, Geometry3D geometry);
        protected OnCreateBufferHandler OnCreateVertexBuffer;
        protected OnCreateBufferHandler OnCreateIndexBuffer;
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

        public GeometryBufferModel(PrimitiveTopology topology, IBufferProxy vertexBuffer, IBufferProxy indexBuffer)
        {
            Topology = topology;
            VertexBuffer = Collect(vertexBuffer);
            if (indexBuffer != null)
            { IndexBuffer = Collect(indexBuffer); }
        }
        /// <summary>
        /// Attach buffers only
        /// </summary>
        /// <param name="context"></param>
        /// <param name="instanceModel"></param>
        /// <returns></returns>
        public bool AttachBuffers(DeviceContext context, InputLayout vertexLayout, InstanceBufferModel instanceModel)
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
            if (IndexBuffer.Buffer == null && VertexBuffer.Buffer == null)
            {
                return false;
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
                    context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer.Buffer, VertexBuffer.StructureSize, VertexBuffer.Offset * VertexBuffer.StructureSize));
                }
                else
                {
                    instanceModel.Attach(context);
                    context.InputAssembler.SetVertexBuffers(0, new[] {
                        new VertexBufferBinding(VertexBuffer.Buffer, VertexBuffer.StructureSize, VertexBuffer.Offset*VertexBuffer.StructureSize),
                        new VertexBufferBinding(instanceModel.InstanceBuffer.Buffer, instanceModel.InstanceBuffer.StructureSize, instanceModel.InstanceBuffer.Offset * instanceModel.InstanceBuffer.StructureSize)
                    });
                }
            }
            return true;
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
                if(instanceModel == null || !instanceModel.HasInstance)
                {
                    context.DrawIndexed(IndexBuffer.Count, 0, 0);
                }
                else
                {
                    context.DrawIndexedInstanced(IndexBuffer.Count, instanceModel.InstanceBuffer.Count, 0, 0, instanceModel.InstanceBuffer.Offset * instanceModel.InstanceBuffer.StructureSize);
                }
                return true;
            }
            else
            {
                return false;
            }
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
            return CreateBufferModel<VertexStruct>(PrimitiveTopology.LineList, structSize);
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
            return CreateBufferModel<VertexStruct>(PrimitiveTopology.PointList, structSize, false);
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

    public class MeshGeometryBufferModel<VertexStruct> : GeometryBufferModel where VertexStruct : struct
    {
        public delegate VertexStruct[] BuildVertexArrayHandler(MeshGeometry3D geometry);
        /// <summary>
        /// Create VertexStruct[] from geometry position, texturecoord, colors, etc.
        /// </summary>
        public BuildVertexArrayHandler OnBuildVertexArray;

        public MeshGeometryBufferModel(int structSize) : base(PrimitiveTopology.TriangleList, 
            new ImmutableBufferProxy<VertexStruct>(structSize, BindFlags.VertexBuffer), new ImmutableBufferProxy<int>(sizeof(int), BindFlags.VertexBuffer))
        {
            OnCreateVertexBuffer = (context, buffer, geometry) =>
            {
                // -- set geometry if given
                if (geometry != null && geometry.Positions != null && OnBuildVertexArray != null)
                {
                    // --- get geometry
                    var mesh = geometry as MeshGeometry3D;
                    var data = OnBuildVertexArray(mesh);
                    (buffer as IBufferProxy<VertexStruct>).CreateBufferFromDataArray(context.Device, data, geometry.Positions.Count);
                }
                else
                {
                    buffer.Dispose();
                }
            };
            OnCreateIndexBuffer = (context, buffer, geometry) => 
            {
                if (geometry.Indices != null)
                {
                    (buffer as IBufferProxy<int>).CreateBufferFromDataArray(context.Device, geometry.Indices);
                }
                else
                {
                    buffer.Dispose();
                }
            };
        }

        protected override bool IsVertexBufferChanged(string propertyName)
        {
            return base.IsVertexBufferChanged(propertyName) || propertyName.Equals(nameof(MeshGeometry3D.Colors)) || propertyName.Equals(nameof(MeshGeometry3D.TextureCoordinates));
        }
    }
}
