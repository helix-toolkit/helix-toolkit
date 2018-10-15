/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using System.IO;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;
    using Shaders;
    using Components;
    using Utilities;

    public sealed class Sprite2DRenderCore : RenderCore
    {
        public IAttachableBufferModel Buffer { set; get; }

        private Stream texture;
        public Stream Texture
        {
            set
            {
                if(SetAffectsRender(ref texture, value) && IsAttached)
                {
                    RemoveAndDispose(ref textureView);
                    if (value != null)
                    {
                        textureView = EffectTechnique.EffectsManager.MaterialTextureManager.Register(value, true);
                    }
                }
            }
            get { return texture; }
        }

        public Matrix ProjectionMatrix
        {
            set; get;
        } = Matrix.Identity;

        private int texSlot;

        private int samplerSlot;

        private ShaderPass spritePass;

        private ShaderResourceViewProxy textureView;

        private SamplerStateProxy sampler;

        private readonly ConstantBufferComponent globalTransformCB;
        public Sprite2DRenderCore()
            : base(RenderType.ScreenSpaced)
        {
            globalTransformCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.GlobalTransformCB, GlobalTransformStruct.SizeInBytes)));
        }

        public override void Render(RenderContext context, DeviceContextProxy deviceContext)
        {
            if(Buffer == null || textureView == null || spritePass.IsNULL)
            {
                return;
            }
            int slot = 0;
            if(!Buffer.AttachBuffers(deviceContext, ref slot, EffectTechnique.EffectsManager))
            {
                return;
            }
            var globalTrans = context.GlobalTransform;
            globalTrans.Projection = ProjectionMatrix;
            globalTransformCB.Upload(deviceContext, ref globalTrans);
            spritePass.BindShader(deviceContext);
            spritePass.BindStates(deviceContext, StateType.All);
            spritePass.PixelShader.BindTexture(deviceContext, texSlot, textureView);
            spritePass.PixelShader.BindSampler(deviceContext, samplerSlot, sampler);
            deviceContext.SetViewport(0, 0, (float)context.ActualWidth, (float)context.ActualHeight);
            deviceContext.SetScissorRectangle(0, 0, (int)context.ActualWidth, (int)context.ActualHeight);
            deviceContext.DrawIndexed(Buffer.IndexBuffer.ElementCount, 0, 0);
            RaiseInvalidateRender();
        }

        public override void RenderCustom(RenderContext context, DeviceContextProxy deviceContext)
        {
            
        }

        public override void RenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        {
            
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            spritePass = technique[DefaultPassNames.Default];
            texSlot = spritePass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.SpriteTB);
            samplerSlot = spritePass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.SpriteSampler);
            if (texture != null)
            {
                textureView = Collect(EffectTechnique.EffectsManager.MaterialTextureManager.Register(texture, true));
            }
            sampler = Collect(EffectTechnique.EffectsManager.StateManager.Register(DefaultSamplers.LinearSamplerClampAni1));
            return true;
        }

        protected override void OnDetach()
        {
            textureView = null;
            sampler = null;
            base.OnDetach();
        }
    }
}
