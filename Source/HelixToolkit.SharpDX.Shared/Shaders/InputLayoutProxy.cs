using System;
using System.Collections.Generic;
using System.Text;
using global::SharpDX.Direct3D11;
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
    namespace Shaders
    {
        public sealed class InputLayoutProxy : DisposeObject
        {
            private InputLayout layout;
            public InputLayout Layout => layout;

            public InputLayoutProxy(Device device, byte[] vertexShaderByteCode, InputElement[] elements)
            {
                layout = new InputLayout(device, vertexShaderByteCode, elements);
            }


            protected override void OnDispose(bool disposeManagedResources)
            {
                RemoveAndDispose(ref layout);
                base.OnDispose(disposeManagedResources);
            }

            public static explicit operator InputLayout(InputLayoutProxy proxy)
            {
                return proxy.layout;
            }
        }
    }
}
