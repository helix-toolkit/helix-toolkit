/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if DEBUG
//#define DEBUGCACHECREATE
#endif
using SharpDX;
using SharpDX.DXGI;
#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    using System.Diagnostics;
    using Utilities;
#if NETFX_CORE
    public abstract partial class Element2DCore
#else
    public abstract partial class Element2DCore
#endif
    {
        /// <summary>
        /// The minimum bitmap size by Bytes. Default 2048 * B8G8R8A8 format = 64kb. https://msdn.microsoft.com/en-us/library/windows/desktop/dd372260%28v=vs.85%29.aspx?f=255&MSPPError=-2147217396
        /// </summary>
        private const int MinimumBitmapSize = 2048;
        /// <summary>
        /// Gets or sets a value indicating whether [enable bitmap cache].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable bitmap cache]; otherwise, <c>false</c>.
        /// </value>
        internal bool EnableBitmapCacheInternal { set; get; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is bitmap cache valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is bitmap cache valid; otherwise, <c>false</c>.
        /// </value>
        internal bool IsBitmapCacheValid { set; get; } = false;

        private BitmapProxy bitmapCache;

        /// <summary>
        /// Ensures the bitmap cache.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="size">The size.</param>
        /// <param name="maxSize">The maximum size.</param>
        private void EnsureBitmapCache(IRenderContext2D context, Size2 size, int maxSize)
        {
            IsBitmapCacheValid = false;
            if (size.Width == 0 || size.Height == 0 || !EnableBitmapCacheInternal || size.Width * size.Height < MinimumBitmapSize)
            {
                Disposer.RemoveAndDispose(ref bitmapCache);
            }
            else if (size.Width > maxSize || size.Height > maxSize)
            {
                return;
            }
            else if (bitmapCache == null || size.Width > bitmapCache.Size.Width || size.Height > bitmapCache.Size.Height)
            {
#if DEBUGCACHECREATE
                Debug.WriteLine("Create new bitmap cache.");
#endif
                Disposer.RemoveAndDispose(ref bitmapCache);
                bitmapCache = BitmapProxy.Create("Cache", context.DeviceContext, size, Format.B8G8R8A8_UNorm);
                IsBitmapCacheValid = true;
                IsVisualDirty = true;
            }
            else
            {
                IsBitmapCacheValid = true;
            }
        }
    }
}
