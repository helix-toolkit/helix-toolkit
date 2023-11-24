using SharpDX;

namespace HelixToolkit.SharpDX;

public class TextInfo
{
    public string Text
    {
        get; set;
    } = string.Empty;

    public Vector3 Origin
    {
        get; set;
    }

    public Color4 Foreground { set; get; } = Color.Black;

    public Color4 Background { set; get; } = Color.Transparent;

    public float ActualWidth
    {
        protected set; get;
    }

    public float ActualHeight
    {
        protected set; get;
    }

    public float Scale { set; get; } = 1;

    /// <summary>
    /// Gets or sets the rotation angle in radians.
    /// </summary>
    /// <value>
    /// The angle in radians.
    /// </value>
    public float Angle { set; get; } = 0;

    /// <summary>
    /// Sets or gets the horizontal alignment. Default = <see cref="BillboardHorizontalAlignment.Center"/>
    /// <para>
    /// For example, when sets horizontal and vertical alignment to top/left,
    /// billboard's bottom/right point will be anchored at the billboard origin.
    /// </para>
    /// </summary>
    /// <value>
    /// The horizontal alignment.
    /// </value>
    public BillboardHorizontalAlignment HorizontalAlignment
    {
        set; get;
    } = BillboardHorizontalAlignment.Center;

    /// <summary>
    /// Sets or gets the vertical alignment. Default = <see cref="BillboardVerticalAlignment.Center"/>
    /// <para>
    /// For example, when sets horizontal and vertical alignment to top/left,
    /// billboard's bottom/right point will be anchored at the billboard origin.
    /// </para>
    /// </summary>
    /// <value>
    /// The vertical alignment.
    /// </value>
    public BillboardVerticalAlignment VerticalAlignment
    {
        set; get;
    } = BillboardVerticalAlignment.Center;

    /// <summary>
    /// Additional offset for billboard display location.
    /// Behavior depends on whether billboard is fixed sized or not.
    /// When billboard is fixed sized, the offset is screen spaced.
    /// </summary>
    public Vector2 Offset
    {
        set; get;
    } = Vector2.Zero;

    public TextInfo()
    {
    }

    public TextInfo(string text, Vector3 origin)
    {
        Text = text;
        Origin = origin;
    }

    public virtual void UpdateTextInfo(float actualWidth, float actualHeight)
    {
        ActualWidth = actualWidth;
        ActualHeight = actualHeight;
        BoundSphere = new BoundingSphere(Origin, Math.Max(actualWidth, actualHeight) / 2);
    }

    public BoundingSphere BoundSphere
    {
        get; private set;
    }
}
