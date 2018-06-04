using System;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using System.IO;
    using Utilities;

    public interface ITextureResourceManager
    {
        ShaderResourceViewProxy Register(Stream textureStream);
    }
}
