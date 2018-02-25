using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.Wpf.SharpDX
{
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
                    ((d as IRenderable).RenderCore as IScreenClone).CloneRectangle = new Rectangle((int)rect.Left, (int)rect.Top, (int)rect.Width, (int)rect.Height);
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
                    ((d as IRenderable).RenderCore as IScreenClone).Output = (int)e.NewValue;
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
                    ((d as IRenderable).RenderCore as IScreenClone).StretchToFill = (bool)e.NewValue;
                }));



        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenDuplicationModel"/> class.
        /// </summary>
        public ScreenDuplicationModel()
        {
            IsHitTestVisible = false;

        }
        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override IRenderCore OnCreateRenderCore()
        {
            return new ScreenCloneRenderCore();
        }
        /// <summary>
        /// Assigns the default values to core.
        /// </summary>
        /// <param name="core">The core.</param>
        protected override void AssignDefaultValuesToCore(IRenderCore core)
        {
            base.AssignDefaultValuesToCore(core);
            (core as IScreenClone).Output = this.DisplayIndex;
            (core as IScreenClone).CloneRectangle = new Rectangle((int)CaptureRectangle.Left, (int)CaptureRectangle.Top, (int)CaptureRectangle.Width, (int)CaptureRectangle.Height);
            (core as IScreenClone).StretchToFill = StretchToFill;
        }
        /// <summary>
        /// Override this function to set render technique during Attach Host.
        /// <para>If <see cref="Element3DCore.OnSetRenderTechnique" /> is set, then <see cref="Element3DCore.OnSetRenderTechnique" /> instead of <see cref="OnCreateRenderTechnique" /> function will be called.</para>
        /// </summary>
        /// <param name="host"></param>
        /// <returns>
        /// Return RenderTechnique
        /// </returns>
        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.ScreenDuplication];
        }
        /// <summary>
        /// Called when [hit test].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="totalModelMatrix">The total model matrix.</param>
        /// <param name="ray">The ray.</param>
        /// <param name="hits">The hits.</param>
        /// <returns></returns>
        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }
    }
}
