using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.Wpf.SharpDX
{
    public class ScreenDuplicationModel : Element3D
    {
        public Rect ScreenRectangle
        {
            get { return (Rect)GetValue(ScreenRectangleProperty); }
            set { SetValue(ScreenRectangleProperty, value); }
        }

        public static readonly DependencyProperty ScreenRectangleProperty =
            DependencyProperty.Register("ScreenRectangle", typeof(Rect), typeof(ScreenDuplicationModel), new PropertyMetadata(new Rect()));

        public int DisplayIndex
        {
            get { return (int)GetValue(DisplayIndexProperty); }
            set { SetValue(DisplayIndexProperty, value); }
        }

        public static readonly DependencyProperty DisplayIndexProperty =
            DependencyProperty.Register("DisplayIndex", typeof(int), typeof(ScreenDuplicationModel), new PropertyMetadata(0));


        public ScreenDuplicationModel()
        {
            IsHitTestVisible = false;

        }

        protected override IRenderCore OnCreateRenderCore()
        {
            return new ScreenCloneRenderCore();
        }

        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.ScreenDuplication];
        }

        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }
    }
}
