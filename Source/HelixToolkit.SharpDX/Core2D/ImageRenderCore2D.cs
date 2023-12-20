using SharpDX;
using SharpDX.Mathematics.Interop;
using D2D = global::SharpDX.Direct2D1;

namespace HelixToolkit.SharpDX.Core2D;

public class ImageRenderCore2D : RenderCore2DBase
{
    private D2D.Bitmap? bitmap;
    /// <summary>
    /// Gets or sets the bitmap.
    /// </summary>
    /// <value>
    /// The bitmap.
    /// </value>
    public D2D.Bitmap? Bitmap
    {
        set
        {
            var old = bitmap;
            if (SetAffectsRender(ref bitmap, value))
            {
                RemoveAndDispose(ref old);
                if (value != null)
                {
                    ImageSize = bitmap?.Size.ToVector2() ?? new();
                }
                else
                {
                    ImageSize = new();
                }
            }
        }
        get
        {
            return bitmap;
        }
    }
    /// <summary>
    /// Gets or sets the size of the image.
    /// </summary>
    /// <value>
    /// The size of the image.
    /// </value>
    public Vector2 ImageSize
    {
        private set; get;
    }

    private float opacity = 1;
    /// <summary>
    /// Gets or sets the opacity.
    /// </summary>
    /// <value>
    /// The opacity.
    /// </value>
    public float Opacity
    {
        set
        {
            SetAffectsRender(ref opacity, value);
        }
        get
        {
            return opacity;
        }
    }

    private D2D.BitmapInterpolationMode interpolationMode = D2D.BitmapInterpolationMode.Linear;
    /// <summary>
    /// Gets or sets the interpolation mode.
    /// </summary>
    /// <value>
    /// The interpolation mode.
    /// </value>
    public D2D.BitmapInterpolationMode InterpolationMode
    {
        set
        {
            Set(ref interpolationMode, value);
        }
        get
        {
            return interpolationMode;
        }
    }

    protected override bool CanRender(RenderContext2D context)
    {
        return base.CanRender(context) && Bitmap != null;
    }

    protected override void OnRender(RenderContext2D context)
    {
        context.DeviceContext.DrawBitmap(Bitmap, LayoutBound.ToStruct<RectangleF, RawRectangleF>(), Opacity, InterpolationMode);
    }

    protected override void OnDetach()
    {
        RemoveAndDispose(ref bitmap);
        base.OnDetach();
    }
}
