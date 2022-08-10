using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using HelixToolkit.Wpf.SharpDX;

namespace CrossSectionDemo
{
    /// <summary>
    /// Viewport which does hit test on mouse over
    /// </summary>
    public class CustomViewport3DX : Viewport3DX
    {
        /// <inheritdoc />
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            base.OnPreviewMouseMove(e);

            // During startup the camera in the render context might be null while the camera in this class isn't.
            // In that case don't do the hit test. The program is just starting anyway and the model at cursor won't be interesting yet.
            if (RenderContext?.Camera != null)
            {
                IList<HitTestResult> hits = this.FindHits(e.GetPosition(this));
                ModelAtCursor = hits.Count > 0 ? hits[0].ModelHit : null;
            }
            else
            {
                ModelAtCursor = null;
            }
        }

        public static readonly DependencyProperty ModelAtCursorProperty = DependencyProperty.Register(
            nameof(ModelAtCursor), typeof(object), typeof(CustomViewport3DX), new PropertyMetadata(default(object)));

        /// <summary>
        /// The model that the mouse hovers over
        /// </summary>
        public object ModelAtCursor
        {
            get => GetValue(ModelAtCursorProperty);
            set => SetValue(ModelAtCursorProperty, value);
        }
    }
}
