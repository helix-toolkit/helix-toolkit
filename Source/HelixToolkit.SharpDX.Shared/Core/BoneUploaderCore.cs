/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using Matrix = System.Numerics.Matrix4x4;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;
    using Utilities;
    using Shaders;
    using Mathematics;

    public sealed class BoneUploaderCore : RenderCore
    {
        public event EventHandler OnBoneChanged;
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
                    OnBoneChanged?.Invoke(this, EventArgs.Empty);
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

        public override void Update(RenderContext context, DeviceContextProxy deviceContext)
        {
            if (matricsChanged)
            {
                BoneSkinSB.UploadDataToBuffer(deviceContext, boneMatrices, boneMatrices.Length);
                matricsChanged = false;
            }
        }

        public override void RenderCustom(RenderContext context, DeviceContextProxy deviceContext)
        {
        }

        public override void RenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        {
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            BoneSkinSB = Collect(new StructuredBufferProxy(MatrixHelper.SizeInBytes, false));
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
                OnBoneChanged = null;
            }
            base.OnDispose(disposeManagedResources);
        }
    }
}
