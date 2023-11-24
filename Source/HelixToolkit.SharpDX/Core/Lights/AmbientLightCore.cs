using HelixToolkit.SharpDX.Model;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public class AmbientLightCore : LightCoreBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AmbientLightCore"/> class.
    /// </summary>
    public AmbientLightCore()
    {
        LightType = LightType.Ambient;
    }
    /// <summary>
    /// Called when [render].
    /// </summary>
    /// <param name="lightScene">The light scene.</param>
    /// <param name="idx">The index.</param>
    protected override void OnRender(Light3DSceneShared? lightScene, int idx)
    {
        if (lightScene is null)
        {
            return;
        }

        lightScene.LightModels.AmbientLight = Color;
    }
}
