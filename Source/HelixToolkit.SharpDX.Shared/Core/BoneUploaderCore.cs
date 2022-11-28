/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
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
        using Shaders;

        public sealed class BoneUploaderCore : RenderCore
        {
            public event EventHandler BoneChanged;
            private static readonly Matrix[] empty = new Matrix[0];
            private bool matricesChanged = true;
            private Matrix[] boneMatrices = empty;
            public Matrix[] BoneMatrices
            {
                set
                {
                    if (SetAffectsRender(ref boneMatrices, value))
                    {
                        matricesChanged = true;
                        if (value == null)
                        {
                            boneMatrices = empty;
                        }
                        BoneChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
                get
                {
                    return boneMatrices;
                }
            }
            public StructuredBufferProxy boneSkinSB = null;
            public StructuredBufferProxy BoneSkinSB => boneSkinSB;

            public BoneUploaderCore() : base(RenderType.None)
            {
                NeedUpdate = false;
            }

            public override void Render(RenderContext context, DeviceContextProxy deviceContext)
            {

            }

            protected override void OnUpdate(RenderContext context, DeviceContextProxy deviceContext)
            {
                if (matricesChanged && BoneSkinSB != null)
                {
                    BoneSkinSB.UploadDataToBuffer(deviceContext, boneMatrices, boneMatrices.Length);
                    matricesChanged = false;
                }
            }

            protected override bool OnAttach(IRenderTechnique technique)
            {
                boneSkinSB = new StructuredBufferProxy(Matrix.SizeInBytes, false);
                return true;
            }

            protected override void OnDetach()
            {
                RemoveAndDispose(ref boneSkinSB);
            }

            public void BindBuffer(DeviceContextProxy deviceContext, int slot)
            {
                if (BoneSkinSB != null)
                {
                    deviceContext.SetShaderResource(VertexShader.Type, slot, BoneSkinSB);
                }
            }

            public void InvalidateBoneMatrices()
            {
                matricesChanged = true;
            }

            protected override void OnDispose(bool disposeManagedResources)
            {
                if (disposeManagedResources)
                {
                    BoneChanged = null;
                }
                base.OnDispose(disposeManagedResources);
            }
        }
    }
}
