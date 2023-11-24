using SharpDX;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public interface IScreenSpacedRenderParams
{
    event EventHandler<BoolArgs> OnCoordinateSystemChanged;
    /// <summary>
    /// Relative position X of the center of viewport
    /// </summary>
    float RelativeScreenLocationX
    {
        set; get;
    }
    /// <summary>
    /// Relative position Y of the center of viewport
    /// </summary>
    float RelativeScreenLocationY
    {
        set; get;
    }
    /// <summary>
    /// 
    /// </summary>
    float SizeScale
    {
        set; get;
    }
    /// <summary>
    /// Only being used when <see cref="Mode"/> is RelativeScreenSpaced
    /// </summary>
    ScreenSpacedCameraType CameraType
    {
        set; get;
    }

    bool IsPerspective
    {
        get;
    }
    /// <summary>
    /// 
    /// </summary>
    float Width
    {
        get;
    }
    /// <summary>
    /// 
    /// </summary>
    float Height
    {
        get;
    }
    /// <summary>
    /// 
    /// </summary>
    float Size
    {
        get;
    }
    /// <summary>
    /// 
    /// </summary>
    float ScreenRatio
    {
        get;
    }
    /// <summary>
    /// 
    /// </summary>
    float Fov
    {
        get;
    }
    /// <summary>
    /// 
    /// </summary>
    float CameraDistance
    {
        get;
    }
    /// <summary>
    /// 
    /// </summary>
    GlobalTransformStruct GlobalTransform
    {
        get;
    }
    /// <summary>
    /// Gets or sets the mode.
    /// </summary>
    /// <value>
    /// The mode.
    /// </value>
    ScreenSpacedMode Mode
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the absolute position. Used in <see cref="ScreenSpacedMode.AbsolutePosition3D"/>
    /// </summary>
    /// <value>
    /// The absolute position.
    /// </value>
    Vector3 AbsolutePosition3D
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the far plane for screen spaced camera rendering.
    /// </summary>
    /// <value>
    /// The far plane.
    /// </value>
    float FarPlane
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the near plane for screen spaced camera rendering.
    /// </summary>
    /// <value>
    /// The near plane.
    /// </value>
    float NearPlane
    {
        set; get;
    }
}
