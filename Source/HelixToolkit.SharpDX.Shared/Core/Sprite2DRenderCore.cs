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
        using Shaders;
        using Components;
        using Utilities;

        public sealed class Sprite2DRenderCore : RenderCore
        {
            public IAttachableBufferModel Buffer
            {
                set; get;
            }

            public Matrix ProjectionMatrix
            {
                set; get;
            } = Matrix.Identity;

            private ShaderResourceViewProxy textureView;

            private int texSlot;

            private int samplerSlot;

            private ShaderPass spritePass;

            private SamplerStateProxy sampler;

            private readonly ConstantBufferComponent globalTransformCB;
            public Sprite2DRenderCore()
                : base(RenderType.ScreenSpaced)
            {
                globalTransformCB = AddComponent(new ConstantBufferComponent(
                    new ConstantBufferDescription(DefaultBufferNames.GlobalTransformCB, GlobalTransformStruct.SizeInBytes)));
            }

            public void UpdateTexture(TextureModel texture, ITextureResourceManager manager)
            {
                var tex = manager.Register(texture, true);
                RemoveAndDispose(ref textureView);
                textureView = tex;
            }

            public override void Render(RenderContext context, DeviceContextProxy deviceContext)
            {
                if (Buffer == null || textureView == null || spritePass.IsNULL)
                {
                    return;
                }
                var slot = 0;
                if (!Buffer.AttachBuffers(deviceContext, ref slot, EffectTechnique.EffectsManager))
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

            protected override bool OnAttach(IRenderTechnique technique)
            {
                spritePass = technique[DefaultPassNames.Default];
                texSlot = spritePass.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.SpriteTB);
                samplerSlot = spritePass.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.SpriteSampler);
                sampler = EffectTechnique.EffectsManager.StateManager.Register(DefaultSamplers.LinearSamplerClampAni1);
                return true;
            }

            protected override void OnDetach()
            {
                RemoveAndDispose(ref textureView);
                RemoveAndDispose(ref sampler);
            }
        }
    }
}
