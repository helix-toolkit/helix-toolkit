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
/// <seealso cref="ScreenSpacePositionMoverBase" />
public class ScreenSpacePositionMover : ScreenSpacePositionMoverBase
{
    private readonly Button2D MoveLeftTop;
    private readonly Button2D MoveLeftBottom;
    private readonly Button2D MoveRightTop;
    private readonly Button2D MoveRightBottom;
    private readonly Button2D[] buttons = new Button2D[4];

    /// <summary>
    /// Initializes a new instance of the <see cref="ScreenSpacePositionMover"/> class.
    /// </summary>
    public ScreenSpacePositionMover()
    {
        MoveLeftTop = new MoverButton2D()
        {
            HorizontalAlignment = UIHorizontalAlignment.Left,
            VerticalAlignment = UIVerticalAlignment.Top,
            BorderThickness = new UIThickness(2, 0, 0, 2)
        };

        MoveRightTop = new MoverButton2D()
        {
            HorizontalAlignment = UIHorizontalAlignment.Right,
            VerticalAlignment = UIVerticalAlignment.Top,
            BorderThickness = new UIThickness(2, 2, 0, 0)
        };

        MoveLeftBottom = new MoverButton2D()
        {
            HorizontalAlignment = UIHorizontalAlignment.Left,
            VerticalAlignment = UIVerticalAlignment.Bottom,
            BorderThickness = new UIThickness(0, 0, 2, 2)
        };

        MoveRightBottom = new MoverButton2D()
        {
            HorizontalAlignment = UIHorizontalAlignment.Right,
            VerticalAlignment = UIVerticalAlignment.Bottom,
            BorderThickness = new UIThickness(0, 2, 2, 0)
        };

        buttons[0] = MoveLeftTop;
        buttons[1] = MoveLeftBottom;
        buttons[2] = MoveRightTop;
        buttons[3] = MoveRightBottom;

        Width = 100;
        Height = 100;

        foreach (var b in buttons)
        {
#if WINUI
            b.Visibility = UIVisibility.Collapsed;
#else
            b.Visibility = UIVisibility.Hidden;
#endif
            Children.Add(b);
        }

        MoveLeftTop.Clicked2D += (s, e) => { RaiseOnMoveClick(ScreenSpaceMoveDirection.LeftTop); };
        MoveLeftBottom.Clicked2D += (s, e) => { RaiseOnMoveClick(ScreenSpaceMoveDirection.LeftBottom); };
        MoveRightTop.Clicked2D += (s, e) => { RaiseOnMoveClick(ScreenSpaceMoveDirection.RightTop); };
        MoveRightBottom.Clicked2D += (s, e) => { RaiseOnMoveClick(ScreenSpaceMoveDirection.RightBottom); };
    }

    protected override SceneNode2D OnCreateSceneNode()
    {
        return new Node2DMover() { Buttons = buttons };
    }

    public sealed class Node2DMover : Node2DMoverBase
    {
        public Button2D[]? Buttons
        {
            set;
            get;
        }

        /// <summary>
        /// Called when [hit test].
        /// </summary>
        /// <param name="mousePoint">The mouse point.</param>
        /// <param name="hitResult">The hit result.</param>
        /// <returns></returns>
        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult? hitResult)
        {
            hitResult = null;
            if (!EnableMover)
            {
                return false;
            }
            if (LayoutBoundWithTransform.Contains(mousePoint))
            {
                if (Buttons is not null)
                {
                    foreach (Button2D b in Buttons)
                    {
                        b.Visibility = UIVisibility.Visible;
                    }
                }
                return base.OnHitTest(ref mousePoint, out hitResult);
            }
            else
            {
                if (Buttons is not null)
                {
                    foreach (Button2D b in Buttons)
                    {
#if WINUI
                        b.Visibility = UIVisibility.Collapsed;
#else
                        b.Visibility = UIVisibility.Hidden;
#endif
                    }
                }
                return false;
            }
        }
    }
}
