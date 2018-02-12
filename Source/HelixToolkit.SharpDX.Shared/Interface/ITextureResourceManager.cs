using System;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using System.IO;
    using Model;

    public interface ITextureResourceManager
    {
        SharedTextureResourceProxy Register(Guid modelGuid, Stream textureStream);
        void Unregister(Guid modelGuid, Stream textureStream);
    }
}
