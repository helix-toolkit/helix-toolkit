/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using System;
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
        /// <summary>
        /// 
        /// </summary>
        public class BitmapProxy : DisposeObject, IGUID
        {
            /// <summary>
            /// Gets the unique identifier.
            /// </summary>
            /// <value>
            /// The unique identifier.
            /// </value>
            public Guid GUID { get; } = Guid.NewGuid();
            /// <summary>
            /// Gets the properties.
            /// </summary>
            /// <value>
            /// The properties.
            /// </value>
            public BitmapProperties1 Properties { private set; get; }
            /// <summary>
            /// Gets the context.
            /// </summary>
            /// <value>
            /// The context.
            /// </value>
            public DeviceContext Context { private set; get; }
            /// <summary>
            /// Gets the bitmap.
            /// </summary>
            /// <value>
            /// The bitmap.
            /// </value>
            public Bitmap1 Bitmap { private set; get; }
            /// <summary>
            /// Gets the name.
            /// </summary>
            /// <value>
            /// The name.
            /// </value>
            public string Name { private set; get; }
            /// <summary>
            /// Gets the bitmap size.
            /// </summary>
            /// <value>
            /// The size.
            /// </value>
            public Size2 Size { private set; get; }
            /// <summary>
            /// Initializes a new instance of the <see cref="BitmapProxy"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="context">The context.</param>
            /// <param name="size">The size.</param>
            /// <param name="properties">The properties.</param>
            public BitmapProxy(string name, DeviceContext context, Size2 size, BitmapProperties1 properties)
            {
                Properties = properties;
                Context = context;
                Bitmap = Collect(new Bitmap1(context, size, properties));
                Size = size;
                Name = name;
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="BitmapProxy"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="context">The context.</param>
            /// <param name="surface">The surface.</param>
            /// <param name="properties">The properties.</param>
            public BitmapProxy(string name, DeviceContext context, Surface surface, BitmapProperties1 properties)
            {
                Properties = properties;
                Context = context;
                Bitmap = Collect(new Bitmap1(context, surface, properties));
                Size = new Size2((int)Bitmap.Size.Width, (int)Bitmap.Size.Height);
                Name = name;
            }

            /// <summary>
            /// Creates the description.
            /// </summary>
            /// <param name="dpiX">The dpi x.</param>
            /// <param name="dpiY">The dpi y.</param>
            /// <param name="format">The format.</param>
            /// <param name="alphaMode">The alpha mode.</param>
            /// <param name="options">The options.</param>
            /// <param name="colorContext">The color context.</param>
            /// <returns></returns>
            public static BitmapProperties1 CreateDescription(float dpiX, float dpiY, Format format,
                global::SharpDX.Direct2D1.AlphaMode alphaMode = global::SharpDX.Direct2D1.AlphaMode.Premultiplied,
                BitmapOptions options = BitmapOptions.Target | BitmapOptions.CannotDraw, ColorContext colorContext = null)
            {
                // Make sure that the texture to create is a render target
                options |= BitmapOptions.Target;
                var description = NewDescription(dpiX, dpiY, new PixelFormat(format, alphaMode), options, colorContext);
                return description;
            }

            /// <summary>
            /// News the description.
            /// </summary>
            /// <param name="dpiX">The dpi x.</param>
            /// <param name="dpiY">The dpi y.</param>
            /// <param name="format">The format.</param>
            /// <param name="bitmapOptions">The bitmap options.</param>
            /// <param name="colorContext">The color context.</param>
            /// <returns></returns>
            protected static BitmapProperties1 NewDescription(float dpiX, float dpiY, PixelFormat format,
                BitmapOptions bitmapOptions, ColorContext colorContext)
            {
                return new BitmapProperties1(format, dpiX, dpiY, bitmapOptions, colorContext);
            }

            /// <summary>
            /// Creates by surface
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="context">The context.</param>
            /// <param name="surface">The surface.</param>
            /// <returns></returns>
            public static BitmapProxy Create(string name, DeviceContext context, Surface surface)
            {
                return new BitmapProxy(name, context, surface, CreateDescription(context.DotsPerInch.Width, context.DotsPerInch.Height, surface.Description.Format));
            }

            /// <summary>
            /// Creates by size and format
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="context">The context.</param>
            /// <param name="size">The size.</param>
            /// <param name="format">The format.</param>
            /// <returns></returns>
            public static BitmapProxy Create(string name, DeviceContext context, Size2 size, Format format)
            {
                return new BitmapProxy(name, context, size, CreateDescription(context.DotsPerInch.Width, context.DotsPerInch.Height, format, global::SharpDX.Direct2D1.AlphaMode.Premultiplied, BitmapOptions.Target));
            }

            /// <summary>
            /// Performs an implicit conversion from <see cref="BitmapProxy"/> to <see cref="Bitmap1"/>.
            /// </summary>
            /// <param name="proxy">The proxy.</param>
            /// <returns>
            /// The result of the conversion.
            /// </returns>
            public static implicit operator Bitmap1(BitmapProxy proxy)
            {
                return proxy.Bitmap;
            }
        }
    }

}
