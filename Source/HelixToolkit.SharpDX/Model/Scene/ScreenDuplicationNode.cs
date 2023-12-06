using HelixToolkit.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene;

/// <summary>
/// 
/// </summary>
public class ScreenDuplicationNode : SceneNode
{
    #region Properties
    /// <summary>
    /// Gets or sets the capture rectangle.
    /// </summary>
    /// <value>
    /// The capture rectangle.
    /// </value>
    public Rectangle CaptureRectangle
    {
        set
        {
            if (RenderCore is IScreenClone core)
            {
                core.CloneRectangle = value;
            }
        }
        get
        {
            if (RenderCore is IScreenClone core)
            {
                return core.CloneRectangle;
            }

            return Rectangle.Empty;
        }
    }
    /// <summary>
    /// Gets or sets the display index.
    /// </summary>
    /// <value>
    /// The display index.
    /// </value>
    public int DisplayIndex
    {
        set
        {
            if (RenderCore is IScreenClone core)
            {
                core.Output = value;
            }
        }
        get
        {
            if (RenderCore is IScreenClone core)
            {
                return core.Output;
            }

            return 0;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [stretch to fill].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [stretch to fill]; otherwise, <c>false</c>.
    /// </value>
    public bool StretchToFill
    {
        set
        {
            if (RenderCore is IScreenClone core)
            {
                core.StretchToFill = value;
            }
        }
        get
        {
            if (RenderCore is IScreenClone core)
            {
                return core.StretchToFill;
            }

            return false;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [show mouse cursor].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [show mouse cursor]; otherwise, <c>false</c>.
    /// </value>
    public bool ShowMouseCursor
    {
        set
        {
            if (RenderCore is IScreenClone core)
            {
                core.ShowMouseCursor = value;
            }
        }
        get
        {
            if (RenderCore is IScreenClone core)
            {
                return core.ShowMouseCursor;
            }

            return false;
        }
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="ScreenDuplicationNode"/> class.
    /// </summary>
    public ScreenDuplicationNode()
    {
        IsHitTestVisible = false;
    }

    /// <summary>
    /// Called when [create render core].
    /// </summary>
    /// <returns></returns>
    protected override RenderCore OnCreateRenderCore()
    {
        return new ScreenCloneRenderCore();
    }

    protected override IRenderTechnique? OnCreateRenderTechnique(IEffectsManager effectsManager)
    {
        return effectsManager[DefaultRenderTechniqueNames.ScreenDuplication];
    }

    protected override bool OnHitTest(HitTestContext? context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
    {
        return false;
    }
}
