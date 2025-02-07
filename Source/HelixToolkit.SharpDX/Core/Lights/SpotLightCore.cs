﻿using HelixToolkit.SharpDX.Model;
using SharpDX;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public class SpotLightCore : PointLightCore
{
    private Vector3 direction;
    /// <summary>
    /// Gets or sets the direction.
    /// </summary>
    /// <value>
    /// The direction.
    /// </value>
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

    private float fallOff = 1;
    /// <summary>
    /// Gets or sets the fall off.
    /// </summary>
    /// <value>
    /// The fall off.
    /// </value>
    public float FallOff
    {
        set
        {
            SetAffectsRender(ref fallOff, value);
        }
        get
        {
            return fallOff;
        }
    }

    private float innerAngle = 5;
    /// <summary>
    /// Gets or sets the inner angle.
    /// </summary>
    /// <value>
    /// The inner angle.
    /// </value>
    public float InnerAngle
    {
        set
        {
            SetAffectsRender(ref innerAngle, value);
        }
        get
        {
            return innerAngle;
        }
    }

    private float outerAngle = 45;
    /// <summary>
    /// Gets or sets the outer angle.
    /// </summary>
    /// <value>
    /// The outer angle.
    /// </value>
    public float OuterAngle
    {
        set
        {
            SetAffectsRender(ref outerAngle, value);
        }
        get
        {
            return outerAngle;
        }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="SpotLightCore"/> class.
    /// </summary>
    public SpotLightCore()
    {
        LightType = LightType.Spot;
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
        lightScene.LightModels.Lights[index].LightDir = new Vector4(Vector3.Normalize(Vector3.TransformNormal(direction, ModelMatrix)), 0f);
        lightScene.LightModels.Lights[index].LightSpot = new Vector4((float)Math.Cos(outerAngle / 360.0f * Math.PI), (float)Math.Cos(innerAngle / 360.0f * Math.PI), fallOff, 0);
    }
}
