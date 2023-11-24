using HelixToolkit.SharpDX.Core.Components;
using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Shaders;
using HelixToolkit.SharpDX.Utilities;
using SharpDX;

namespace HelixToolkit.SharpDX.Core;

public sealed class Sprite2DRenderCore : RenderCore
{
    public IAttachableBufferModel? Buffer
    {
        set; get;
    }

    public Matrix ProjectionMatrix
    {
        set; get;
    } = Matrix.Identity;

    private ShaderResourceViewProxy? textureView;

    private int texSlot;

    private int samplerSlot;

    private ShaderPass? spritePass;

    private SamplerStateProxy? sampler;

    private readonly ConstantBufferComponent globalTransformCB;
    public Sprite2DRenderCore()
        : base(RenderType.ScreenSpaced)
    {
        globalTransformCB = AddComponent(new ConstantBufferComponent(
            new ConstantBufferDescription(DefaultBufferNames.GlobalTransformCB, GlobalTransformStruct.SizeInBytes)));
    }

    public void UpdateTexture(TextureModel? texture, ITextureResourceManager manager)
    {
        var tex = manager.Register(texture, true);
        RemoveAndDispose(ref textureView);
        textureView = tex;
    }

    public override void Render(RenderContext context, DeviceContextProxy deviceContext)
    {
        if (Buffer == null || textureView == null || spritePass is null || spritePass.IsNULL || Buffer?.IndexBuffer is null)
        {
            return;
        }
        var slot = 0;
        if (!Buffer.AttachBuffers(deviceContext, ref slot, EffectTechnique?.EffectsManager))
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

    protected override bool OnAttach(IRenderTechnique? technique)
    {
        spritePass = technique?[DefaultPassNames.Default];
        texSlot = spritePass?.PixelShader.ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.SpriteTB) ?? 0;
        samplerSlot = spritePass?.PixelShader.SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.SpriteSampler) ?? 0;
        sampler = EffectTechnique?.EffectsManager?.StateManager?.Register(DefaultSamplers.LinearSamplerClampAni1);
        return true;
    }

    protected override void OnDetach()
    {
        RemoveAndDispose(ref textureView);
        RemoveAndDispose(ref sampler);
    }
}
