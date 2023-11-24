namespace HelixToolkit.SharpDX;

/// <summary>
/// Used for render ordering. Order is the same as render type defined.
/// </summary>
public enum RenderType
{
    None, Light, PreProc, Opaque, Particle, Transparent, PostEffect, GlobalEffect, ScreenSpaced
}
