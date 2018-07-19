/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using System.Linq;
using global::SharpDX.Direct3D11;
using Matrix = System.Numerics.Matrix4x4;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using HelixToolkit.Mathematics;
    using Render;
    using Utilities;

    /// <summary>
    /// Used for managing instance buffer update
    /// </summary>
    public class ElementsBufferModel<T> : DisposeObject, IElementsBufferModel<T> where T : struct
    {
        public event EventHandler<EventArgs> OnElementChanged;
        public Guid GUID { get; } = Guid.NewGuid();
        public bool Initialized { private set; get; }
        public bool HasElements { private set; get; } = false;
        public IElementsBufferProxy Buffer { get { return elementBuffer; } }
        private IElementsBufferProxy elementBuffer;
        private VertexBufferBinding bufferBinding;

        public bool Changed { get { return instanceChanged; } }
        private volatile bool instanceChanged = true;

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
                    OnElementChanged?.Invoke(this, EventArgs.Empty);
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

        public virtual void AttachBuffer(DeviceContextProxy context, ref int vertexBufferStartSlot)
        {
            if (HasElements)
            {
                if (instanceChanged)
                {
                    lock (elementBuffer)
                    {
                        if (instanceChanged)
                        {
                            elementBuffer.UploadDataToBuffer(context, elements, elements.Count);
                            instanceChanged = false;
                            bufferBinding = new VertexBufferBinding(Buffer.Buffer, Buffer.StructureSize, Buffer.Offset);
                        }
                    }
                }
                context.SetVertexBuffers(vertexBufferStartSlot, bufferBinding);
            }
            ++vertexBufferStartSlot;
        }

        protected override void OnDispose(bool disposeManagedResources)
        {
            Initialized = false;
            base.OnDispose(disposeManagedResources);
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
            : base(MatrixHelper.SizeInBytes)
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