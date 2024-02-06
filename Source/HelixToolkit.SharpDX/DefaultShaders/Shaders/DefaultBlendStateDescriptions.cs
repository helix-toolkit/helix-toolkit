using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Shaders;

/// <summary>
/// 
/// </summary>
public static class DefaultBlendStateDescriptions
{
    public readonly static BlendStateDescription BSAlphaBlend;
    public readonly static BlendStateDescription BSSourceAlways;
    public readonly static BlendStateDescription NoBlend;
    public readonly static BlendStateDescription BSOverlayBlending;
    public readonly static BlendStateDescription AdditiveBlend;
    public readonly static BlendStateDescription BSScreenDupCursorBlend;
    public readonly static BlendStateDescription BSOITBlend = new() { IndependentBlendEnable = true };
    public readonly static BlendStateDescription BSOTISortingBlend;
    public readonly static BlendStateDescription BSMeshOITBlendQuad;
    public readonly static BlendStateDescription VolumeBlending;
    public readonly static BlendStateDescription BSGlowBlending;
    public readonly static BlendStateDescription BSOITDP;
    public readonly static BlendStateDescription BSOITDPMaxBlending;
    public readonly static BlendStateDescription BSOITDPFinal;

    static DefaultBlendStateDescriptions()
    {
        BSAlphaBlend.RenderTarget[0] = new RenderTargetBlendDescription()
        {
            AlphaBlendOperation = BlendOperation.Add,
            BlendOperation = BlendOperation.Add,
            SourceBlend = BlendOption.SourceAlpha,
            DestinationBlend = BlendOption.InverseSourceAlpha,

            SourceAlphaBlend = BlendOption.SourceAlpha,
            DestinationAlphaBlend = BlendOption.DestinationAlpha,
            IsBlendEnabled = true,
            RenderTargetWriteMask = ColorWriteMaskFlags.All
        };

        BSSourceAlways.RenderTarget[0] = new RenderTargetBlendDescription()
        {
            IsBlendEnabled = false,
            RenderTargetWriteMask = ColorWriteMaskFlags.All
        };

        NoBlend.RenderTarget[0] = new RenderTargetBlendDescription() { IsBlendEnabled = false };

        BSOverlayBlending.RenderTarget[0] = new RenderTargetBlendDescription()
        {
            IsBlendEnabled = true,
            SourceBlend = BlendOption.One,
            DestinationBlend = BlendOption.One,
            BlendOperation = BlendOperation.Add,
            SourceAlphaBlend = BlendOption.Zero,
            DestinationAlphaBlend = BlendOption.One,
            AlphaBlendOperation = BlendOperation.Add,
            RenderTargetWriteMask = ColorWriteMaskFlags.All
        };

        AdditiveBlend.RenderTarget[0] = new RenderTargetBlendDescription()
        {
            IsBlendEnabled = true,
            SourceBlend = BlendOption.One,
            DestinationBlend = BlendOption.One,
            BlendOperation = BlendOperation.Add,
            SourceAlphaBlend = BlendOption.One,
            DestinationAlphaBlend = BlendOption.One,
            AlphaBlendOperation = BlendOperation.Add,
            RenderTargetWriteMask = ColorWriteMaskFlags.All
        };

        BSScreenDupCursorBlend.RenderTarget[0] = new RenderTargetBlendDescription()
        {
            SourceBlend = BlendOption.SourceAlpha,
            DestinationBlend = BlendOption.InverseSourceAlpha,
            BlendOperation = BlendOperation.Add,
            SourceAlphaBlend = BlendOption.One,
            DestinationAlphaBlend = BlendOption.Zero,
            AlphaBlendOperation = BlendOperation.Add,
            RenderTargetWriteMask = ColorWriteMaskFlags.All,
            IsBlendEnabled = true
        };

        BSOITBlend.RenderTarget[0] = new RenderTargetBlendDescription()
        {
            IsBlendEnabled = true,
            SourceBlend = BlendOption.One,
            DestinationBlend = BlendOption.One,
            BlendOperation = BlendOperation.Add,
            SourceAlphaBlend = BlendOption.One,
            DestinationAlphaBlend = BlendOption.One,
            AlphaBlendOperation = BlendOperation.Add,
            RenderTargetWriteMask = ColorWriteMaskFlags.All
        };
        BSOITBlend.RenderTarget[1] = new RenderTargetBlendDescription()
        {
            IsBlendEnabled = true,
            SourceBlend = BlendOption.Zero,
            DestinationBlend = BlendOption.InverseSourceAlpha,
            BlendOperation = BlendOperation.Add,
            SourceAlphaBlend = BlendOption.Zero,
            DestinationAlphaBlend = BlendOption.InverseSourceAlpha,
            AlphaBlendOperation = BlendOperation.Add,
            RenderTargetWriteMask = ColorWriteMaskFlags.Alpha
        };

        BSMeshOITBlendQuad.RenderTarget[0] = new RenderTargetBlendDescription()
        {
            IsBlendEnabled = true,
            SourceBlend = BlendOption.InverseSourceAlpha,
            DestinationBlend = BlendOption.SourceAlpha,
            BlendOperation = BlendOperation.Add,
            SourceAlphaBlend = BlendOption.InverseSourceAlpha,
            DestinationAlphaBlend = BlendOption.DestinationAlpha,
            AlphaBlendOperation = BlendOperation.Add,
            RenderTargetWriteMask = ColorWriteMaskFlags.Red | ColorWriteMaskFlags.Green | ColorWriteMaskFlags.Blue
        };

        VolumeBlending.RenderTarget[0] = new RenderTargetBlendDescription()
        {
            AlphaBlendOperation = BlendOperation.Add,
            BlendOperation = BlendOperation.Add,
            SourceBlend = BlendOption.SourceAlpha,
            DestinationBlend = BlendOption.InverseSourceAlpha,

            SourceAlphaBlend = BlendOption.InverseSourceAlpha,
            DestinationAlphaBlend = BlendOption.Zero,
            IsBlendEnabled = true,
            RenderTargetWriteMask = ColorWriteMaskFlags.All
        };

        BSGlowBlending.RenderTarget[0] = new RenderTargetBlendDescription()
        {
            BlendOperation = BlendOperation.Add,
            SourceBlend = BlendOption.One,
            DestinationBlend = BlendOption.InverseSourceAlpha,

            SourceAlphaBlend = BlendOption.SourceAlpha,
            DestinationAlphaBlend = BlendOption.DestinationAlpha,
            AlphaBlendOperation = BlendOperation.Add,
            IsBlendEnabled = true,
            RenderTargetWriteMask = ColorWriteMaskFlags.All
        };

        BSOITDP.IndependentBlendEnable = true;
        BSOITDP.AlphaToCoverageEnable = false;
        // Max blending
        BSOITDP.RenderTarget[0] = new RenderTargetBlendDescription()
        {
            IsBlendEnabled = true,
            RenderTargetWriteMask = ColorWriteMaskFlags.All,
            SourceBlend = BlendOption.One,
            DestinationBlend = BlendOption.One,
            BlendOperation = BlendOperation.Maximum,
            SourceAlphaBlend = BlendOption.One,
            DestinationAlphaBlend = BlendOption.One,
            AlphaBlendOperation = BlendOperation.Maximum
        };
        // Front to back blending
        BSOITDP.RenderTarget[1] = new RenderTargetBlendDescription()
        {
            IsBlendEnabled = true,
            RenderTargetWriteMask = ColorWriteMaskFlags.All,
            SourceBlend = BlendOption.DestinationAlpha,
            DestinationBlend = BlendOption.One,
            BlendOperation = BlendOperation.Add,
            SourceAlphaBlend = BlendOption.Zero,
            DestinationAlphaBlend = BlendOption.InverseSourceAlpha,
            AlphaBlendOperation = BlendOperation.Add
        };
        // Back to front blending
        BSOITDP.RenderTarget[2] = new RenderTargetBlendDescription()
        {
            IsBlendEnabled = true,
            RenderTargetWriteMask = ColorWriteMaskFlags.All,
            SourceBlend = BlendOption.SourceAlpha,
            DestinationBlend = BlendOption.InverseSourceAlpha,
            BlendOperation = BlendOperation.Add,
            SourceAlphaBlend = BlendOption.Zero,
            DestinationAlphaBlend = BlendOption.One,
            AlphaBlendOperation = BlendOperation.Add
        };
        BSOITDPMaxBlending.AlphaToCoverageEnable = false;
        BSOITDPMaxBlending.IndependentBlendEnable = true;
        // Max blending
        for (int i = 0; i < 3; ++i)
        {
            BSOITDPMaxBlending.RenderTarget[i] = new RenderTargetBlendDescription()
            {
                IsBlendEnabled = true,
                RenderTargetWriteMask = ColorWriteMaskFlags.All,
                SourceBlend = BlendOption.One,
                DestinationBlend = BlendOption.One,
                BlendOperation = BlendOperation.Maximum,
                SourceAlphaBlend = BlendOption.One,
                DestinationAlphaBlend = BlendOption.One,
                AlphaBlendOperation = BlendOperation.Maximum
            };
        }
        BSOITDPFinal = BSSourceAlways;
        BSOITDPFinal.AlphaToCoverageEnable = false;
    }
}
