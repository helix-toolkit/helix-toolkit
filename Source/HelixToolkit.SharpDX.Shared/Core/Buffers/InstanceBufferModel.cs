#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using global::SharpDX;
    using global::SharpDX.Direct3D11;
    using Utilities;
    using System.Linq;
    using System.Collections.Generic;
    using System;
    /// <summary>
    /// Used for managing instance buffer update
    /// </summary>
    public class InstanceBufferModel : DisposeObject, IGUID
    {
        public Guid GUID { get; } = Guid.NewGuid();
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
                if (instances != value)
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
}