using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HelixToolkit.Wpf.SharpDX.Core2D;
using SharpDX;
using D2D = SharpDX.Direct2D1;
using SharpDX.WIC;
using SharpDX.DXGI;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="HelixToolkit.Wpf.SharpDX.Elements2D.Element2D" />
    public class ImageModel2D : Element2D
    {
        /// <summary>
        /// Gets or sets the image stream.
        /// </summary>
        /// <value>
        /// The image stream.
        /// </value>
        public Stream ImageStream
        {
            get { return (Stream)GetValue(ImageStreamProperty); }
            set { SetValue(ImageStreamProperty, value); }
        }

        /// <summary>
        /// The image stream property
        /// </summary>
        public static readonly DependencyProperty ImageStreamProperty =
            DependencyProperty.Register("ImageStream", typeof(Stream), typeof(ImageModel2D), new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsRender,
                (d,e)=> 
                {
                    (d as ImageModel2D).bitmapChanged = true;
                }));


        /// <summary>
        /// Gets or sets the opacity.
        /// </summary>
        /// <value>
        /// The opacity.
        /// </value>
        public double Opacity
        {
            get { return (double)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }

        /// <summary>
        /// The opacity property
        /// </summary>
        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register("Opacity", typeof(double), typeof(ImageModel2D), new PropertyMetadata(1.0, (d,e)=> 
            {
                ((d as ImageModel2D).RenderCore as ImageRenderCore2D).Opacity = (float)(double)e.NewValue;
            }));



        protected bool bitmapChanged { private set; get; } = true;

        protected override IRenderCore2D CreateRenderCore()
        {
            return new ImageRenderCore2D();
        }

        protected override bool OnAttach(IRenderHost host)
        {         
            if (base.OnAttach(host))
            {
                (RenderCore as ImageRenderCore2D).Opacity = (float)Opacity;
                bitmapChanged = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void LoadBitmap(IRenderContext2D context, Stream stream)
        {
            (RenderCore as ImageRenderCore2D).Bitmap = stream == null ? null : OnLoadImage(context, stream);
        }

        protected virtual D2D.Bitmap OnLoadImage(IRenderContext2D context, Stream stream)
        {
            stream.Position = 0;
            using (var decoder = new BitmapDecoder(context.DeviceResources.WICImgFactory, stream, DecodeOptions.CacheOnLoad))
            {
                using (var frame = decoder.GetFrame(0))
                {
                    using (var converter = new FormatConverter(context.DeviceResources.WICImgFactory))
                    {
                        converter.Initialize(frame, PixelFormat.Format32bppPBGRA);
                        return D2D.Bitmap1.FromWicBitmap(context.DeviceContext, converter);
                    }
                }
            }
        }

        public override void Update(IRenderContext2D context)
        {
            base.Update(context);
            if (bitmapChanged)
            {
                LoadBitmap(context, ImageStream);
                bitmapChanged = false;
            }
        }

        protected override Size2F MeasureOverride(Size2F availableSize)
        {
            if(ImageStream != null)
            {
                var imageSize = (RenderCore as ImageRenderCore2D).ImageSize;
                if (Width == 0 && Height == 0)
                {
                    return new Size2F(Math.Min(availableSize.Width, imageSize.Width), Math.Min(availableSize.Height, imageSize.Height));
                }
                else
                {
                    float aspectRatio = (RenderCore as ImageRenderCore2D).ImageSize.Width / (RenderCore as ImageRenderCore2D).ImageSize.Height;
                    if(Width == 0)
                    {
                        var height = Math.Min(availableSize.Height, HeightInternal);
                        return new Size2F(height / aspectRatio, height);
                    }
                    else
                    {
                        var width = Math.Min(availableSize.Width, WidthInternal);
                        return new Size2F(width, width * aspectRatio);
                    }
                }
            }
            return new Size2F(Math.Max(0, WidthInternal), Math.Max(0, HeightInternal));
        }

        //protected override RectangleF ArrangeOverride(RectangleF finalSize)
        //{
        //    textRenderable.MaxWidth = finalSize.Width;
        //    textRenderable.MaxHeight = finalSize.Height;
        //    var metrices = textRenderable.Metrices;
        //    return finalSize;
        //}

        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            hitResult = null;
            return false;
        }
    }
}
