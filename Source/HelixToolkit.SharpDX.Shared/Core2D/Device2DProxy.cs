/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using DeviceContext2D = global::SharpDX.Direct2D1.DeviceContext;

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
    namespace Core2D
    {
        using Utilities;

        /// <summary>
        /// 
        /// </summary>
        public sealed class D2DTargetProxy : DisposeObject 
        {
            private BitmapProxy d2DTarget;
            /// <summary>
            /// Gets the d2d target. Which is bind to the 3D back buffer/texture
            /// </summary>
            /// <value>
            /// The d2d target.
            /// </value>
            public BitmapProxy D2DTarget { get { return d2DTarget; } }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="swapChain"></param>
            /// <param name="deviceContext"></param>
            public void Initialize(SwapChain1 swapChain, DeviceContext2D deviceContext)
            {
                RemoveAndDispose(ref d2DTarget);
                using (var surf = swapChain.GetBackBuffer<Surface>(0))
                {
                    d2DTarget = Collect(BitmapProxy.Create("SwapChainTarget", deviceContext, surf));
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="texture"></param>
            /// <param name="deviceContext"></param>
            public void Initialize(Texture2D texture, DeviceContext2D deviceContext)
            {
                RemoveAndDispose(ref d2DTarget);        
                using (var surface = texture.QueryInterface<global::SharpDX.DXGI.Surface>())
                {
                    d2DTarget = Collect(BitmapProxy.Create("TextureTarget", deviceContext, surface));
                }
            }

            /// <summary>
            /// Performs an implicit conversion from <see cref="D2DTargetProxy"/> to <see cref="Bitmap1"/>.
            /// </summary>
            /// <param name="proxy">The proxy.</param>
            /// <returns>
            /// The result of the conversion.
            /// </returns>
            public static implicit operator Bitmap1(D2DTargetProxy proxy)
            {
                return proxy.d2DTarget;
            }
        }
    }

}
