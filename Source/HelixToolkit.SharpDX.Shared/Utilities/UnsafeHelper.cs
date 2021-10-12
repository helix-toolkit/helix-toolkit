using System;
using System.Collections.Generic;
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
    namespace Utilities
    {
        public static class UnsafeHelper
        {
            public static int SizeOf<T>() where T : unmanaged
            {
                unsafe
                {
                    return sizeof(T);
                }
            }
        }    
    }
}
