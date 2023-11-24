using SharpDX;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public interface ICrossSectionRenderParams
{
    /// <summary>
    /// Cutting operation, intersects or substract
    /// </summary>
    CuttingOperation CuttingOperation
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the color of the section.
    /// </summary>
    /// <value>
    /// The color of the section.
    /// </value>
    Color4 SectionColor
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets a value indicating whether [plane1/plane2/plane3/plane4 enabled].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [plane1/plane2/plane3/plane4 enabled]; otherwise, <c>false</c>.
    /// </value>
    Bool4 PlaneEnabled
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the plane5 to 8 enabled.
    /// </summary>
    /// <value>
    /// The plane5 to8 enabled.
    /// </value>
    Bool4 Plane5To8Enabled
    {
        set; get;
    }
    /// <summary>
    /// Defines the plane (Normal + d)
    /// </summary>
    Vector4 Plane1Params
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the plane2 parameters.(Normal + d)
    /// </summary>
    /// <value>
    /// The plane2 parameters.
    /// </value>
    Vector4 Plane2Params
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the plane3 parameters.(Normal + d)
    /// </summary>
    /// <value>
    /// The plane3 parameters.
    /// </value>
    Vector4 Plane3Params
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the plane4 parameters.(Normal + d)
    /// </summary>
    /// <value>
    /// The plane4 parameters.
    /// </value>
    Vector4 Plane4Params
    {
        set; get;
    }

    /// <summary>
    /// Gets or sets the plane5 parameters.(Normal + d)
    /// </summary>
    /// <value>
    /// The plane5 parameters.
    /// </value>
    Vector4 Plane5Params
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the plane6 parameters.(Normal + d)
    /// </summary>
    /// <value>
    /// The plane6 parameters.
    /// </value>
    Vector4 Plane6Params
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the plane7 parameters.(Normal + d)
    /// </summary>
    /// <value>
    /// The plane7 parameters.
    /// </value>
    Vector4 Plane7Params
    {
        set; get;
    }

    /// <summary>
    /// Gets or sets the plane8 parameters.(Normal + d)
    /// </summary>
    /// <value>
    /// The plane8 parameters.
    /// </value>
    Vector4 Plane8Params
    {
        set; get;
    }
}
