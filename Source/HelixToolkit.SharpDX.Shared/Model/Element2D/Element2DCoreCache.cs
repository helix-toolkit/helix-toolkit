/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
using System.Windows;
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    using global::SharpDX.DXGI;
    using Utilities;
#if NETFX_CORE
    public abstract partial class Element2DCore
#else
    public abstract partial class Element2DCore
#endif
    {
        /// <summary>
        /// Gets or sets a value indicating whether [enable bitmap cache].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable bitmap cache]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableBitmapCache { set; get; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is bitmap cache valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is bitmap cache valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsBitmapCacheValid { set; get; } = false;

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
            if (size.Width == 0 || size.Height == 0 || !EnableBitmapCache)
            {
                Disposer.RemoveAndDispose(ref bitmapCache);
            }
            else if (size.Width > maxSize || size.Height > maxSize)
            {
                return;
            }
            else if (bitmapCache == null || size.Width > bitmapCache.Size.Width || size.Height > bitmapCache.Size.Height)
            {
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
