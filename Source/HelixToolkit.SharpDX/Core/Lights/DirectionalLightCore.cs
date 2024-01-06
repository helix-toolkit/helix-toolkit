using HelixToolkit.SharpDX.Model;
using SharpDX;

namespace HelixToolkit.SharpDX.Core;

public class DirectionalLightCore : LightCoreBase
{
    private Vector3 direction;
    public Vector3 Direction
    {
        set
        {
            SetAffectsRender(ref direction, value);
        }
        get
        {
            return direction;
        }
    }

    public DirectionalLightCore()
    {
        LightType = LightType.Directional;
    }

    protected override void OnRender(Light3DSceneShared? lightScene, int index)
    {
        if (lightScene is null)
        {
            return;
        }

        base.OnRender(lightScene, index);
        lightScene.LightModels.Lights[index].LightDir = -Vector3.Normalize(Vector3.TransformNormal(direction, ModelMatrix)).ToVector4(0);
    }
}
