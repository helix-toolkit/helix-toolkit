using HelixToolkit.SharpDX.Core.Components;
using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Shaders;
using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using SharpDX.Direct3D11;
using PixelShader = HelixToolkit.SharpDX.Shaders.PixelShader;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
///
/// </summary>
public class PostEffectBlurCore : DisposeObject
{
    public enum BlurDepth
    {
        One = 1, Two = 3
    }

    #region Variables
    private const int NumPingPongBlurBuffer = 2;
    private ShaderPass screenBlurPassVertical;
    private ShaderPass screenBlurPassHorizontal;
    private readonly int textureSlot;
    private readonly int samplerSlot;
    private ConstantBufferComponent? modelCB;
    private SamplerStateProxy? sampler;
    private static readonly Color4 Transparent = new(0, 0, 0, 0);
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="PostEffectMeshOutlineBlurCore"/> class.
    /// </summary>
    public PostEffectBlurCore(ShaderPass blurVerticalPass, ShaderPass blurHorizontalPass, int textureSlot, int samplerSlot,
        SamplerStateDescription sampler, IEffectsManager? manager)
    {
        screenBlurPassVertical = blurVerticalPass;
        screenBlurPassHorizontal = blurHorizontalPass;
        this.textureSlot = textureSlot;
        this.samplerSlot = samplerSlot;
        this.sampler = manager?.StateManager?.Register(sampler);
        modelCB = new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.BorderEffectCB, BorderEffectStruct.SizeInBytes));
    }

    /// <summary>
    /// Runs the blur procedure
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="deviceContext">The device context.</param>
    /// <param name="source">The source.</param>
    /// <param name="depth">The depth.</param>
    /// <param name="sourceViewport"></param>
    /// <param name="modelStruct"></param>
    public virtual void Run(RenderContext context, DeviceContextProxy deviceContext,
        ShaderResourceViewProxy source, ref ViewportF sourceViewport, BlurDepth depth, ref BorderEffectStruct modelStruct)
    {
        deviceContext.SetSampler(PixelShader.Type, samplerSlot, sampler);
        if ((depth & BlurDepth.One) != 0)
        {
            using var target1 = context.GetOffScreenRT(OffScreenTextureSize.Half,
                global::SharpDX.DXGI.Format.R8G8B8A8_UNorm, out var width, out var height);
            modelStruct.ViewportScale = (int)OffScreenTextureSize.Half;
            modelCB?.Upload(deviceContext, ref modelStruct);
            //Full -> Half Vertical
            deviceContext.SetRenderTarget(target1);
            deviceContext.SetViewport(0, 0, width, height);
            deviceContext.SetScissorRectangle(0, 0, width, height);
            screenBlurPassVertical.BindShader(deviceContext);
            screenBlurPassVertical.BindStates(deviceContext, StateType.All);
            screenBlurPassVertical.PixelShader.BindTexture(deviceContext, textureSlot, source);
            deviceContext.Draw(4, 0);

            if ((depth & BlurDepth.Two) != 0)
            {
                using var target2 = context.GetOffScreenRT(OffScreenTextureSize.Quarter,
                    global::SharpDX.DXGI.Format.R8G8B8A8_UNorm, out var width2, out var height2);
                // Half to Quater Vertical
                modelStruct.ViewportScale = (int)OffScreenTextureSize.Quarter;
                modelCB?.Upload(deviceContext, ref modelStruct);
                deviceContext.SetRenderTarget(target2);
                deviceContext.SetViewport(0, 0, width2, height2);
                deviceContext.SetScissorRectangle(0, 0, width2, height2);
                screenBlurPassVertical.BindShader(deviceContext);
                screenBlurPassVertical.PixelShader.BindTexture(deviceContext, textureSlot, target1);
                deviceContext.Draw(4, 0);

                // Quater to Half Horizontal
                modelStruct.ViewportScale = (int)OffScreenTextureSize.Half;
                modelCB?.Upload(deviceContext, ref modelStruct);
                deviceContext.SetRenderTarget(target1);
                deviceContext.SetViewport(0, 0, width, height);
                deviceContext.SetScissorRectangle(0, 0, width, height);
                screenBlurPassHorizontal.BindShader(deviceContext);
                screenBlurPassHorizontal.PixelShader.BindTexture(deviceContext, textureSlot, target2);
                deviceContext.Draw(4, 0);
            }
            // Half to Full Horizontal
            modelStruct.ViewportScale = (int)OffScreenTextureSize.Full;
            modelCB?.Upload(deviceContext, ref modelStruct);
            deviceContext.SetRenderTarget(source);
            deviceContext.SetViewport(ref sourceViewport);
            deviceContext.SetScissorRectangle(ref sourceViewport);
            screenBlurPassHorizontal.BindShader(deviceContext);
            screenBlurPassHorizontal.PixelShader.BindTexture(deviceContext, textureSlot, target1);
            deviceContext.Draw(4, 0);
        }
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        RemoveAndDispose(ref sampler);
        RemoveAndDispose(ref modelCB);
        base.OnDispose(disposeManagedResources);
    }
}
