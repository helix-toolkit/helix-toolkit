using HelixToolkit.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene;

/// <summary>
///
/// </summary>
public class AxisPlaneGridNode : SceneNode
{
    /// <summary>
    /// Gets or sets a value indicating whether [automatic spacing].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [automatic spacing]; otherwise, <c>false</c>.
    /// </value>
    public bool AutoSpacing
    {
        set
        {
            if (RenderCore is AxisPlaneGridCore core)
            {
                core.AutoSpacing = value;
            }
        }
        get
        {
            if (RenderCore is AxisPlaneGridCore core)
            {
                return core.AutoSpacing;
            }

            return false;
        }
    }
    /// <summary>
    /// Gets or sets the automatic spacing rate.
    /// </summary>
    /// <value>
    /// The automatic spacing rate.
    /// </value>
    public float AutoSpacingRate
    {
        set
        {
            if (RenderCore is AxisPlaneGridCore core)
            {
                core.AutoSpacingRate = value;
            }
        }
        get
        {
            if (RenderCore is AxisPlaneGridCore core)
            {
                return core.AutoSpacingRate;
            }

            return 0.0f;
        }
    }

    /// <summary>
    /// Gets the acutal spacing.
    /// </summary>
    /// <value>
    /// The acutal spacing.
    /// </value>
    public float AcutalSpacing
    {
        get
        {
            if (RenderCore is AxisPlaneGridCore core)
            {
                return core.AcutalSpacing;
            }

            return 0.0f;
        }
    }

    /// <summary>
    /// Gets or sets the grid spacing.
    /// </summary>
    /// <value>
    /// The grid spacing.
    /// </value>
    public float GridSpacing
    {
        set
        {
            if (RenderCore is AxisPlaneGridCore core)
            {
                core.GridSpacing = value;
            }
        }
        get
        {
            if (RenderCore is AxisPlaneGridCore core)
            {
                return core.GridSpacing;
            }

            return 0.0f;
        }
    }

    /// <summary>
    /// Gets or sets the grid thickness.
    /// </summary>
    /// <value>
    /// The grid thickness.
    /// </value>
    public float GridThickness
    {
        set
        {
            if (RenderCore is AxisPlaneGridCore core)
            {
                core.GridThickness = value;
            }
        }
        get
        {
            if (RenderCore is AxisPlaneGridCore core)
            {
                return core.GridThickness;
            }

            return 0.0f;
        }
    }

    /// <summary>
    /// Gets or sets the fading factor.
    /// </summary>
    /// <value>
    /// The fading factor.
    /// </value>
    public float FadingFactor
    {
        set
        {
            if (RenderCore is AxisPlaneGridCore core)
            {
                core.FadingFactor = value;
            }
        }
        get
        {
            if (RenderCore is AxisPlaneGridCore core)
            {
                return core.FadingFactor;
            }

            return 0.0f;
        }
    }

    /// <summary>
    /// Gets or sets the color of the plane.
    /// </summary>
    /// <value>
    /// The color of the plane.
    /// </value>
    public Color4 PlaneColor
    {
        set
        {
            if (RenderCore is AxisPlaneGridCore core)
            {
                core.PlaneColor = value;
            }
        }
        get
        {
            if (RenderCore is AxisPlaneGridCore core)
            {
                return core.PlaneColor;
            }

            return Color.Zero;
        }
    }

    /// <summary>
    /// Gets or sets the color of the grid.
    /// </summary>
    /// <value>
    /// The color of the grid.
    /// </value>
    public Color4 GridColor
    {
        set
        {
            if (RenderCore is AxisPlaneGridCore core)
            {
                core.GridColor = value;
            }
        }
        get
        {
            if (RenderCore is AxisPlaneGridCore core)
            {
                return core.GridColor;
            }

            return Color.Zero;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [render shadow map].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [render shadow map]; otherwise, <c>false</c>.
    /// </value>
    public bool RenderShadowMap
    {
        set
        {
            if (RenderCore is AxisPlaneGridCore core)
            {
                core.RenderShadowMap = value;
            }
        }
        get
        {
            if (RenderCore is AxisPlaneGridCore core)
            {
                return core.RenderShadowMap;
            }

            return false;
        }
    }

    /// <summary>
    /// Gets or sets up axis.
    /// </summary>
    /// <value>
    /// Up axis.
    /// </value>
    public Axis UpAxis
    {
        set
        {
            if (RenderCore is AxisPlaneGridCore core)
            {
                core.UpAxis = value;
            }
        }
        get
        {
            if (RenderCore is AxisPlaneGridCore core)
            {
                return core.UpAxis;
            }

            return Axis.X;
        }
    }

    /// <summary>
    /// Gets or sets the axis plane offset.
    /// </summary>
    /// <value>
    /// The offset.
    /// </value>
    public float Offset
    {
        set
        {
            if (RenderCore is AxisPlaneGridCore core)
            {
                core.Offset = value;
            }
        }
        get
        {
            if (RenderCore is AxisPlaneGridCore core)
            {
                return core.Offset;
            }

            return 0.0f;
        }
    }
    /// <summary>
    /// Gets or sets the type of the grid.
    /// </summary>
    /// <value>
    /// The type of the grid.
    /// </value>
    public GridPattern GridPattern
    {
        set
        {
            if (RenderCore is AxisPlaneGridCore core)
            {
                core.GridPattern = value;
            }
        }
        get
        {
            if (RenderCore is AxisPlaneGridCore core)
            {
                return core.GridPattern;
            }

            return GridPattern.Tile;
        }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="AxisPlaneGridNode"/> class.
    /// </summary>
    public AxisPlaneGridNode()
    {
        RenderOrder = 1000;
    }

    protected override RenderCore OnCreateRenderCore()
    {
        return new AxisPlaneGridCore();
    }

    protected override IRenderTechnique? OnCreateRenderTechnique(IEffectsManager effectsManager)
    {
        return effectsManager[DefaultRenderTechniqueNames.PlaneGrid];
    }

    protected override bool OnHitTest(HitTestContext? context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
    {
        if (context is null)
        {
            return false;
        }

        var normal = Vector3.Zero;
        switch (UpAxis)
        {
            case Axis.X:
                normal = Vector3.UnitX;
                break;
            case Axis.Y:
                normal = Vector3.UnitY;
                break;
            case Axis.Z:
                normal = Vector3.UnitZ;
                break;
        }
        var plane = new Plane(normal, -Offset);
        var ray = context.RayWS;
        if (Collision.RayIntersectsPlane(ref ray, ref plane, out Vector3 point))
        {
            var hitTestResult = new HitTestResult
            {
                IsValid = true,
                NormalAtHit = normal,
                Distance = (context.RayWS.Position - point).Length(),
                PointHit = point,
                ModelHit = WrapperSource
            };
            hits.Add(hitTestResult);
            return true;
        }

        return false;
    }
}
