/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using global::SharpDX;
using global::SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Core
    {

        using Render;
        using Utilities;

        /// <summary>
        /// Used for managing instance buffer update
        /// </summary>
        public class ElementsBufferModel<T> : DisposeObject, IElementsBufferModel<T> where T : unmanaged
        {
            public static readonly ElementsBufferModel<T> Empty = new ElementsBufferModel<T>(0);
            public event EventHandler<EventArgs> ElementChanged;
            public Guid GUID { get; } = Guid.NewGuid();
            public bool Initialized
            {
                private set; get;
            }
            public bool HasElements { private set; get; } = false;
            public IElementsBufferProxy Buffer
            {
                get
                {
                    return elementBuffer;
                }
            }
            private IElementsBufferProxy elementBuffer;
            private VertexBufferBinding bufferBinding;

            public bool Changed
            {
                get
                {
                    return instanceChanged;
                }
            }
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
                        ElementChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
                get
                {
                    return elements;
                }
            }

            public int ElementCount
            {
                get
                {
                    return HasElements ? Elements.Count : 0;
                }
            }

            public int StructSize
            {
                private set; get;
            }

            public ElementsBufferModel(int structSize)
            {
                StructSize = structSize;
            }

            public void Initialize()
            {
                elementBuffer = new DynamicBufferProxy(StructSize, BindFlags.VertexBuffer);
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

            public void DisposeAndClear()
            {
                Initialized = false;
                RemoveAndDispose(ref elementBuffer);
            }

            protected override void OnDispose(bool disposeManagedResources)
            {
                DisposeAndClear();
                base.OnDispose(disposeManagedResources);
            }
        }

        public class MatrixInstanceBufferModel : ElementsBufferModel<Matrix>
        {
            public MatrixInstanceBufferModel()
                : base(Matrix.SizeInBytes)
            {
            }
        }

        public class InstanceParamsBufferModel<T> : ElementsBufferModel<T> where T : unmanaged
        {
            public InstanceParamsBufferModel(int structSize) : base(structSize)
            {
            }
        }

        public class VertexBoneIdBufferModel<T> : ElementsBufferModel<T> where T : unmanaged
        {
            public VertexBoneIdBufferModel(int structSize) : base(structSize) { }
        }
    }
}