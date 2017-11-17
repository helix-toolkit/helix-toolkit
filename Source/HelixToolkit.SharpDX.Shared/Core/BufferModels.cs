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
    using System.Collections;
    using System.Collections.Generic;

    public class InstanceBufferModel : DisposeObject
    {
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
                    HasInstance = instances != null && instances.Count > 0;
                }
            }
            get { return instances; }
        }

        public InstanceBufferModel(Effect effect)
        {
            hasInstancesVar = Collect(effect.GetVariableByName("bHasInstances").AsScalar());
            InstanceBuffer = Collect(new DynamicBufferProxy<Matrix>(Matrix.SizeInBytes, BindFlags.VertexBuffer));
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
    }

    public class BufferModel : DisposeObject
    {       
        /// <summary>
        /// Vertex buffer layout, binds into vertex shader
        /// </summary>
        public InputLayout VertexLayout { private set; get; }

        public IBufferProxy VertexBuffer { private set; get; }
        public IBufferProxy IndexBuffer { private set; get; }
        public PrimitiveTopology Topology { private set; get; }

        public BufferModel(InputLayout layout, PrimitiveTopology topology, IBufferProxy vertexBuffer, IBufferProxy indexBuffer)
        {
            VertexLayout = layout;
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
        public bool Attach(DeviceContext context, InstanceBufferModel instanceModel)
        {
            if (IndexBuffer == null || VertexBuffer == null || IndexBuffer.Buffer == null || VertexBuffer.Buffer == null)
            {
                return false;
            }
            context.InputAssembler.InputLayout = VertexLayout;
            context.InputAssembler.PrimitiveTopology = Topology;
            if (IndexBuffer != null)
            {
                context.InputAssembler.SetIndexBuffer(IndexBuffer.Buffer, Format.R32_UInt, IndexBuffer.Offset * IndexBuffer.StructureSize);
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
        public bool AttachAndDraw(DeviceContext context, InstanceBufferModel instanceModel)
        {
            if(Attach(context, instanceModel))
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
        public static BufferModel CreateBufferModel<VertexStruct>(InputLayout layout, PrimitiveTopology topology, int structSize, bool hasIndexBuffer = true) where VertexStruct : struct
        {
            return new BufferModel(layout, topology, 
                new ImmutableBufferProxy<VertexStruct>(structSize, BindFlags.VertexBuffer), new ImmutableBufferProxy<int>(sizeof(int), BindFlags.VertexBuffer));
        }
        /// <summary>
        /// Create mesh geometry buffer model with Topology = TriangleList
        /// </summary>
        /// <typeparam name="VertexStruct"></typeparam>
        /// <param name="layout"></param>
        /// <param name="structSize"></param>
        /// <returns></returns>
        public static BufferModel CreateMeshBufferModel<VertexStruct>(InputLayout layout, int structSize) where VertexStruct : struct
        {
            return CreateBufferModel<VertexStruct>(layout, PrimitiveTopology.TriangleList, structSize);
        }
        /// <summary>
        /// Create line geometry buffer model with Topology = LineList
        /// </summary>
        /// <typeparam name="VertexStruct"></typeparam>
        /// <param name="layout"></param>
        /// <param name="structSize"></param>
        /// <returns></returns>
        public static BufferModel CreateLineBufferModel<VertexStruct>(InputLayout layout, int structSize) where VertexStruct : struct
        {
            return CreateBufferModel<VertexStruct>(layout, PrimitiveTopology.LineList, structSize);
        }
        /// <summary>
        /// Create point geometry buffer model with Topology = PointList
        /// </summary>
        /// <typeparam name="VertexStruct"></typeparam>
        /// <param name="layout"></param>
        /// <param name="structSize"></param>
        /// <returns></returns>
        public static BufferModel CreatePointBufferModel<VertexStruct>(InputLayout layout, int structSize) where VertexStruct : struct
        {
            return CreateBufferModel<VertexStruct>(layout, PrimitiveTopology.PointList, structSize, false);
        }
        /// <summary>
        /// Create billboard geometry buffer model with Topology = TriangleStrip
        /// </summary>
        /// <typeparam name="VertexStruct"></typeparam>
        /// <param name="layout"></param>
        /// <param name="structSize"></param>
        /// <returns></returns>
        public static BufferModel CreateBillboardBufferModel<VertexStruct>(InputLayout layout, int structSize) where VertexStruct : struct
        {
            return CreateBufferModel<VertexStruct>(layout, PrimitiveTopology.TriangleStrip, structSize);
        }
    }
}
