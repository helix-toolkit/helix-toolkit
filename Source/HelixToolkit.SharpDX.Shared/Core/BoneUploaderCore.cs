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
            private bool matricsChanged = true;
            private Matrix[] boneMatrices = empty;
            public Matrix[] BoneMatrices
            {
                set
                {
                    if (SetAffectsRender(ref boneMatrices, value))
                    {
                        matricsChanged = true;
                        if (value == null)
                        {
                            boneMatrices = empty;
                        }
                        BoneChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
                get { return boneMatrices; }
            }

            public StructuredBufferProxy BoneSkinSB { private set; get; }

            public BoneUploaderCore() : base(RenderType.None)
            {
                NeedUpdate = false;
            }

            public override void Render(RenderContext context, DeviceContextProxy deviceContext)
            {

            }

            protected override void OnUpdate(RenderContext context, DeviceContextProxy deviceContext)
            {
                if (matricsChanged)
                {
                    BoneSkinSB.UploadDataToBuffer(deviceContext, boneMatrices, boneMatrices.Length);
                    matricsChanged = false;
                }
            }

            protected override bool OnAttach(IRenderTechnique technique)
            {
                BoneSkinSB = Collect(new StructuredBufferProxy(Matrix.SizeInBytes, false));
                return true;
            }

            protected override void OnDetach()
            {
                BoneSkinSB = null;
                base.OnDetach();
            }
        
            public void BindBuffer(DeviceContextProxy deviceContext, int slot)
            {
                deviceContext.SetShaderResource(VertexShader.Type, slot, BoneSkinSB);
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
