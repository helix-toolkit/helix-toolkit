using HelixToolkit.SharpDX.Model;
using SharpDX;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
///
/// </summary>
public class PointLightCore : LightCoreBase
{
    private Vector3 position;
    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    /// <value>
    /// The position.
    /// </value>
    public Vector3 Position
    {
        set
        {
            SetAffectsRender(ref position, value);
        }
        get
        {
            return position;
        }
    }

    private Vector3 attenuation = new(1, 0, 0);
    /// <summary>
    /// Gets or sets the attenuation.
    /// </summary>
    /// <value>
    /// The attenuation.
    /// </value>
    public Vector3 Attenuation
    {
        set
        {
            SetAffectsRender(ref attenuation, value);
        }
        get
        {
            return attenuation;
        }
    }

    private float range = 1000;
    /// <summary>
    /// Gets or sets the range.
    /// </summary>
    /// <value>
    /// The range.
    /// </value>
    public float Range
    {
        set
        {
            SetAffectsRender(ref range, value);
        }
        get
        {
            return range;
        }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="PointLightCore"/> class.
    /// </summary>
    public PointLightCore()
    {
        LightType = LightType.Point;
    }

    /// <summary>
    /// Called when [render].
    /// </summary>
    /// <param name="lightScene">The light scene.</param>
    /// <param name="index">The index.</param>
    protected override void OnRender(Light3DSceneShared? lightScene, int index)
    {
        if (lightScene is null)
        {
            return;
        }

        base.OnRender(lightScene, index);
        lightScene.LightModels.Lights[index].LightPos = new Vector4(position + ModelMatrix.Row4().ToHomogeneousVector3(), 1f);
        lightScene.LightModels.Lights[index].LightAtt = new Vector4(attenuation, range);
    }
}
