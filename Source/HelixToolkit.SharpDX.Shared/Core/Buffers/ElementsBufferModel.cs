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
        public event EventHandler<bool> OnElementChanged;
        public Guid GUID { get; } = Guid.NewGuid();
        public bool Initialized { private set; get; }
        public bool HasElements { private set; get; } = false;
        public IElementsBufferProxy Buffer { get { return elementBuffer; } }
        private IElementsBufferProxy elementBuffer;

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
                    OnElementChanged?.Invoke(this, true);
                }
            }
            get { return elements; }
        }

        public int StructSize { private set; get; }

        public ElementsBufferModel(int structSize)
        {
            StructSize = structSize;
        }

        public void Initialize()
        {
            elementBuffer = Collect(new DynamicBufferProxy(StructSize, BindFlags.VertexBuffer));
            Initialized = true;
            instanceChanged = true;
        }

        public virtual void AttachBuffer(DeviceContext context, int vertexBufferSlot)
        {
            if (HasElements)
            {
                if (instanceChanged)
                {
                    elementBuffer.UploadDataToBuffer(context, elements, elements.Count);
                    instanceChanged = false;
                }
                context.InputAssembler.SetVertexBuffers(vertexBufferSlot, new VertexBufferBinding(Buffer.Buffer, Buffer.StructureSize, Buffer.Offset));
            }
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            Initialized = false;
            base.Dispose(disposeManagedResources);
        }
        /// <summary>
        /// Disposes the internal resources. Object is reusable.
        /// </summary>
        public override void DisposeAndClear()
        {
            Initialized = false;
            base.DisposeAndClear();
        }
    }

    public class MatrixInstanceBufferModel : ElementsBufferModel<Matrix>
    {
        public MatrixInstanceBufferModel()
            : base(Matrix.SizeInBytes)
        {
        }
    }

    public class InstanceParamsBufferModel<T> : ElementsBufferModel<T> where T : struct
    {
        public InstanceParamsBufferModel(int structSize) : base(structSize)
        { }
    }

    public class VertexBoneIdBufferModel<T> : ElementsBufferModel<T> where T : struct
    {
        public VertexBoneIdBufferModel(int structSize) : base(structSize) { }
    }
}