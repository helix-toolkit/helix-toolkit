using HelixToolkit.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene;

public class MeshOutlineNode : MeshNode
{
    #region Properties
    /// <summary>
    /// Gets or sets a value indicating whether [enable outline].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable outline]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableOutline
    {
        set
        {
            if (RenderCore is IMeshOutlineParams core)
            {
                core.OutlineEnabled = value;
            }
        }
        get
        {
            if (RenderCore is IMeshOutlineParams core)
            {
                return core.OutlineEnabled;
            }

            return false;
        }
    }
    /// <summary>
    /// Gets or sets the color of the outline.
    /// </summary>
    /// <value>
    /// The color of the outline.
    /// </value>
    public Color4 OutlineColor
    {
        set
        {
            if (RenderCore is IMeshOutlineParams core)
            {
                core.Color = value;
            }
        }
        get
        {
            if (RenderCore is IMeshOutlineParams core)
            {
                return core.Color;
            }

            return Color.Zero;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is draw geometry.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is draw geometry; otherwise, <c>false</c>.
    /// </value>
    public bool IsDrawGeometry
    {
        set
        {
            if (RenderCore is IMeshOutlineParams core)
            {
                core.DrawMesh = value;
            }
        }
        get
        {
            if (RenderCore is IMeshOutlineParams core)
            {
                return core.DrawMesh;
            }

            return false;
        }
    }
    /// <summary>
    /// Gets or sets the outline fading factor.
    /// </summary>
    /// <value>
    /// The outline fading factor.
    /// </value>
    public float OutlineFadingFactor
    {
        set
        {
            if (RenderCore is IMeshOutlineParams core)
            {
                core.OutlineFadingFactor = value;
            }
        }
        get
        {
            if (RenderCore is IMeshOutlineParams core)
            {
                return core.OutlineFadingFactor;
            }

            return 0.0f;
        }
    }
    #endregion
    /// <summary>
    /// Called when [create render core].
    /// </summary>
    /// <returns></returns>
    protected override RenderCore OnCreateRenderCore()
    {
        return new MeshOutlineRenderCore();
    }
}
