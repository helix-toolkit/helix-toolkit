#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using global::SharpDX.Direct3D11;
    using Utilities;
    using System.Linq;
    using System.Collections.Generic;
    using System;
    using global::SharpDX;

    /// <summary>
    /// Used for managing instance buffer update
    /// </summary>
    public class InstanceBufferModel<T> : DisposeObject, IInstanceBufferModel<T> where T : struct
    {
        public Guid GUID { get; } = Guid.NewGuid();
        public bool Initialized { private set; get; }
        public bool HasInstance { private set; get; } = false;
        public IBufferProxy InstanceBuffer { get { return instanceBuffer; } }
        private IBufferProxy<T> instanceBuffer;

        private EffectScalarVariable hasInstancesVar;

        public bool InstanceChanged { get { return instanceChanged; } }
        private bool instanceChanged = true;

        private IList<T> instances = null;
        public IList<T> Instances
        {
            set
            {
                if (instances != value)
                {
                    instances = value;
                    instanceChanged = true;
                    HasInstance = instances != null && instances.Any();
                }
            }
            get { return instances; }
        }

        public int StructSize { private set; get; }

        public string HasInstanceVariableName { private set; get; }

        public InstanceBufferModel(int structSize, string hasInstanceVariableName)
        {
            StructSize = structSize;
            HasInstanceVariableName = hasInstanceVariableName;
        }

        public void Initialize(Effect effect)
        {
            hasInstancesVar = Collect(effect.GetVariableByName(HasInstanceVariableName).AsScalar());
            instanceBuffer = Collect(new DynamicBufferProxy<T>(StructSize, BindFlags.VertexBuffer));
            Initialized = true;
        }

        public virtual void AttachBuffer(DeviceContext context, int vertexBufferSlot)
        {
            hasInstancesVar.Set(HasInstance);
            if (HasInstance)
            {
                if (instanceChanged)
                {
                    instanceBuffer.UploadDataToBuffer(context, instances);
                    instanceChanged = false;
                }
                context.InputAssembler.SetVertexBuffers(vertexBufferSlot, new VertexBufferBinding(InstanceBuffer.Buffer, InstanceBuffer.StructureSize, InstanceBuffer.Offset));
            }
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            Initialized = false;
            base.Dispose(disposeManagedResources);
        }
    }

    public class MatrixInstanceBufferModel : InstanceBufferModel<Matrix>
    {
        public MatrixInstanceBufferModel()
            :base(Matrix.SizeInBytes, ShaderVariableNames.HasInstance)
        {
        }
    }

    public class InstanceParamsBufferModel<T> : InstanceBufferModel<T> where T : struct
    {
        public InstanceParamsBufferModel(int structSize):base(structSize, ShaderVariableNames.HasInstanceParams)
        { }
    }
}