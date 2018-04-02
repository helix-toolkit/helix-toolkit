using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SharpDX;

namespace HelixToolkit.Wpf.SharpDX
{
    using Model;
    using Model.Scene;

    /// <summary>
    /// Limitation: Under switchable graphics card setup(Laptop with integrated graphics card and external graphics card), 
    /// only monitor outputs using integrated graphics card is fully supported.
    /// Trying to clone monitor outputs by external graphics card, 
    /// the clone window must reside in those monitors which is rendered by external graphics card, or error will be occurred.
    /// Ref: https://support.microsoft.com/en-us/help/3019314/error-generated-when-desktop-duplication-api-capable-application-is-ru
    /// </summary>
    /// <seealso cref="HelixToolkit.Wpf.SharpDX.Element3D" />
    public class ScreenDuplicationModel : Element3D
    {
        /// <summary>
        /// Gets or sets the screen rectangle on current display
        /// </summary>
        /// <value>
        /// The screen rectangle.
        /// </value>
        public Rect CaptureRectangle
        {
            get { return (Rect)GetValue(CaptureRectangleProperty); }
            set { SetValue(CaptureRectangleProperty, value); }
        }
        /// <summary>
        /// The screen rectangle property
        /// </summary>
        public static readonly DependencyProperty CaptureRectangleProperty =
            DependencyProperty.Register("CaptureRectangle", typeof(Rect), typeof(ScreenDuplicationModel), new PropertyMetadata(new Rect(),
                (d,e)=> 
                {
                    var rect = (Rect)e.NewValue;
                    ((d as Element3DCore).SceneNode as ScreenDuplicationNode).CaptureRectangle = new Rectangle((int)rect.Left, (int)rect.Top, (int)rect.Width, (int)rect.Height);
                }));

        /// <summary>
        /// Gets or sets the display index.
        /// </summary>
        /// <value>
        /// The display index.
        /// </value>
        public int DisplayIndex
        {
            get { return (int)GetValue(DisplayIndexProperty); }
            set { SetValue(DisplayIndexProperty, value); }
        }
        /// <summary>
        /// The display index property
        /// </summary>
        public static readonly DependencyProperty DisplayIndexProperty =
            DependencyProperty.Register("DisplayIndex", typeof(int), typeof(ScreenDuplicationModel), new PropertyMetadata(0, 
                (d,e)=>
                {
                    ((d as Element3DCore).SceneNode as ScreenDuplicationNode).DisplayIndex = (int)e.NewValue;
                }));


        /// <summary>
        /// Gets or sets a value indicating whether [stretch to fill].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [stretch to fill]; otherwise, <c>false</c>.
        /// </value>
        public bool StretchToFill
        {
            get { return (bool)GetValue(StretchToFillProperty); }
            set { SetValue(StretchToFillProperty, value); }
        }

        /// <summary>
        /// The stretch to fill property
        /// </summary>
        public static readonly DependencyProperty StretchToFillProperty =
            DependencyProperty.Register("StretchToFill", typeof(bool), typeof(ScreenDuplicationModel), new PropertyMetadata(false,
                (d,e)=> 
                {
                    ((d as Element3DCore).SceneNode as ScreenDuplicationNode).StretchToFill = (bool)e.NewValue;
                }));


        /// <summary>
        /// Gets or sets a value indicating whether [show mouse cursor].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show mouse cursor]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowMouseCursor
        {
            get { return (bool)GetValue(ShowMouseCursorProperty); }
            set { SetValue(ShowMouseCursorProperty, value); }
        }

        /// <summary>
        /// The show mouse cursor property
        /// </summary>
        public static readonly DependencyProperty ShowMouseCursorProperty =
            DependencyProperty.Register("ShowMouseCursor", typeof(bool), typeof(ScreenDuplicationModel), new PropertyMetadata(true,
                (d,e)=> {
                    ((d as Element3DCore).SceneNode as ScreenDuplicationNode).ShowMouseCursor = (bool)e.NewValue;
                }));

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenDuplicationModel"/> class.
        /// </summary>
        public ScreenDuplicationModel()
        {
            IsHitTestVisible = false;
        }

        protected override SceneNode OnCreateSceneNode()
        {
            return new ScreenDuplicationNode();
        }
        /// <summary>
        /// Assigns the default values to core.
        /// </summary>
        /// <param name="core">The core.</param>
        protected override void AssignDefaultValuesToSceneNode(SceneNode core)
        {
            base.AssignDefaultValuesToSceneNode(core);
            if(core is ScreenDuplicationNode c)
            {
                c.DisplayIndex = this.DisplayIndex;
                c.CaptureRectangle = new Rectangle((int)CaptureRectangle.Left, (int)CaptureRectangle.Top, (int)CaptureRectangle.Width, (int)CaptureRectangle.Height);
                c.StretchToFill = StretchToFill;
                c.ShowMouseCursor = ShowMouseCursor;
            }
        }

    }
}
