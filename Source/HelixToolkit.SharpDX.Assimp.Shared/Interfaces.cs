using HelixToolkit.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
    public interface ITextureIO
    {
        TextureModel Load(string modelPath, string texturePath, ILogger logger);
    }
}
