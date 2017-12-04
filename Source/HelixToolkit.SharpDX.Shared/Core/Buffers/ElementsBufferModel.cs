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
    public class ElementsBufferModel<T> : DisposeObject, IElementsBufferModel<T> where T : struct
    {
        public Guid GUID { get; } = Guid.NewGuid();
        public bool Initialized { private set; get; }
        public bool HasElements { private set; get; } = false;
        public IBufferProxy Buffer { get { return elementBuffer; } }
        private IBufferProxy<T> elementBuffer;

        private EffectScalarVariable hasElementsVar;

        public bool Changed { get { return instanceChanged; } }
        private bool instanceChanged = true;

        private IList<T> elements = null;
        public IList<T> Elements
        {
            set
            {
                if (elements != value)
                {
                    elements = value;
                    instanceChanged = true;
                    HasElements = elements != null && elements.Any();
                }
            }
            get { return elements; }
        }

        public int StructSize { private set; get; }

        public string HasElementsVariableName { private set; get; }

        public ElementsBufferModel(int structSize, string hasElementsVarName)
        {
            StructSize = structSize;
            HasElementsVariableName = hasElementsVarName;
        }

        public void Initialize(Effect effect)
        {
            hasElementsVar = Collect(effect.GetVariableByName(HasElementsVariableName).AsScalar());
            elementBuffer = Collect(new DynamicBufferProxy<T>(StructSize, BindFlags.VertexBuffer));
            Initialized = true;
            instanceChanged = true;
        }

        public virtual void AttachBuffer(DeviceContext context, int vertexBufferSlot)
        {
            hasElementsVar.Set(HasElements);
            if (HasElements)
            {
                if (instanceChanged)
                {
                    elementBuffer.UploadDataToBuffer(context, elements);
                    instanceChanged = false;
                }
                context.InputAssembler.SetVertexBuffers(vertexBufferSlot, new VertexBufferBinding(Buffer.Buffer, Buffer.StructureSize, Buffer.Offset));
            }
        }

        public void ResetHasElementsVariable()
        {
            hasElementsVar.Set(false);
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            Initialized = false;
            base.Dispose(disposeManagedResources);
        }
    }

    public class MatrixInstanceBufferModel : ElementsBufferModel<Matrix>
    {
        public MatrixInstanceBufferModel()
            : base(Matrix.SizeInBytes, ShaderVariableNames.HasInstance)
        {
        }
    }

    public class InstanceParamsBufferModel<T> : ElementsBufferModel<T> where T : struct
    {
        public InstanceParamsBufferModel(int structSize) : base(structSize, ShaderVariableNames.HasInstanceParams)
        { }
    }

    public class VertexBoneIdBufferModel<T> : ElementsBufferModel<T> where T : struct
    {
        public VertexBoneIdBufferModel(int structSize) : base(structSize, ShaderVariableNames.HasBones) { }
    }
}