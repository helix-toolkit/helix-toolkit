using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.Wpf.SharpDX
{
    /// <summary>
    /// 
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
        public Rect ScreenRectangle
        {
            get { return (Rect)GetValue(ScreenRectangleProperty); }
            set { SetValue(ScreenRectangleProperty, value); }
        }
        /// <summary>
        /// The screen rectangle property
        /// </summary>
        public static readonly DependencyProperty ScreenRectangleProperty =
            DependencyProperty.Register("ScreenRectangle", typeof(Rect), typeof(ScreenDuplicationModel), new PropertyMetadata(new Rect(),
                (d,e)=> 
                {
                    var rect = (Rect)e.NewValue;
                    ((d as ScreenDuplicationModel).RenderCore as IScreenClone).CloneRectangle = new Rectangle((int)rect.Left, (int)rect.Top, (int)rect.Width, (int)rect.Height);
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
                    ((d as ScreenDuplicationModel).RenderCore as IScreenClone).Output = (int)e.NewValue;
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

        protected override void AssignDefaultValuesToCore(IRenderCore core)
        {
            base.AssignDefaultValuesToCore(core);
            (core as IScreenClone).Output = this.DisplayIndex;
            (core as IScreenClone).CloneRectangle = new Rectangle((int)ScreenRectangle.Left, (int)ScreenRectangle.Top, (int)ScreenRectangle.Width, (int)ScreenRectangle.Height);
        }
        /// <summary>
        /// Override this function to set render technique during Attach Host.
        /// <para>If <see cref="OnSetRenderTechnique" /> is set, then <see cref="OnSetRenderTechnique" /> instead of <see cref="OnCreateRenderTechnique" /> function will be called.</para>
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
