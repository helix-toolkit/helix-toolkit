using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene2D;

#if WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#else
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#endif

/// <summary>
/// 
/// </summary>
/// <seealso cref="Panel2D" />
public abstract class ScreenSpacePositionMoverBase : Panel2D
{
    /// <summary>
    /// Gets or sets a value indicating whether [enable mover].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable mover]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableMover
    {
        get
        {
            return (bool)GetValue(EnableMoverProperty);
        }
        set
        {
            SetValue(EnableMoverProperty, value);
        }
    }

    /// <summary>
    /// The enable mover property
    /// </summary>
    public static readonly DependencyProperty EnableMoverProperty =
        DependencyProperty.Register("EnableMover", typeof(bool), typeof(ScreenSpacePositionMover), new PropertyMetadata(true, (d, e) =>
        {
            if (d is Element2D { SceneNode: Node2DMoverBase node })
            {
                node.EnableMover = (bool)e.NewValue;
            }
        }));

    /// <summary>
    /// Occurs when [on move clicked].
    /// </summary>
    public event EventHandler<ScreenSpaceMoveDirArgs>? OnMoveClicked;

    /// <summary>
    /// Raises the on move click.
    /// </summary>
    /// <param name="direction">The direction.</param>
    protected void RaiseOnMoveClick(ScreenSpaceMoveDirection direction)
    {
        OnMoveClicked?.Invoke(this, new ScreenSpaceMoveDirArgs(direction));
    }

    public abstract class Node2DMoverBase : PanelNode2D
    {
        public bool EnableMover { set; get; } = true;
        protected override bool CanRender(RenderContext2D context)
        {
            return base.CanRender(context) && EnableMover;
        }
    }
}
