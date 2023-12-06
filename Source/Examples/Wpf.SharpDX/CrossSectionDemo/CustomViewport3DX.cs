using CommunityToolkit.Diagnostics;
using DependencyPropertyGenerator;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using System.Collections.Generic;
using System.Windows.Input;

namespace CrossSectionDemo;

/// <summary>
/// Viewport which does hit test on mouse over
/// </summary>
[DependencyProperty<object>("ModelAtCursor", Description = "The model that the mouse hovers over.")]
public partial class CustomViewport3DX : Viewport3DX
{
    /// <inheritdoc />
    protected override void OnPreviewMouseMove(MouseEventArgs? e)
    {
        Guard.IsNotNull(e);

        base.OnPreviewMouseMove(e);

        // During startup the camera in the render context might be null while the camera in this class isn't.
        // In that case don't do the hit test. The program is just starting anyway and the model at cursor won't be interesting yet.
        if (RenderContext?.Camera is not null)
        {
            IList<HitTestResult> hits = this.FindHits(e.GetPosition(this));
            ModelAtCursor = hits.Count > 0 ? hits[0].ModelHit : null;
        }
        else
        {
            ModelAtCursor = null;
        }
    }
}
