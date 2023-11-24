using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace HelixToolkit.SharpDX.Shaders;

/// <summary>
/// 
/// </summary>
public static class DefaultInputLayout
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly InputElement[] VSInput = new InputElement[]
    {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                new InputElement("NORMAL",   0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                new InputElement("TANGENT",  0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                new InputElement("BINORMAL", 0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float,       InputElement.AppendAligned, 1),
                new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 2),             
                //INSTANCING: die 4 texcoords sind die matrix, die mit jedem buffer reinwandern
                new InputElement("TEXCOORD", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 3, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 3, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 3, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 4, Format.R32G32B32A32_Float, InputElement.AppendAligned, 3, InputClassification.PerInstanceData, 1)
    };

    public static InputElement[] VSMeshBatchedInput = new InputElement[]
    {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                new InputElement("NORMAL",   0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                new InputElement("TANGENT",  0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                new InputElement("BINORMAL", 0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float,       InputElement.AppendAligned, 0),
                new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                new InputElement("COLOR",    1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
    };
    /// <summary>
    /// 
    /// </summary>
    public static readonly InputElement[] VSInputInstancing = new InputElement[]
    {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                new InputElement("NORMAL",   0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                new InputElement("TANGENT",  0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                new InputElement("BINORMAL", 0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float,       InputElement.AppendAligned, 1),
                new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 2),             
                //INSTANCING: die 4 texcoords sind die matrix, die mit jedem buffer reinwandern
                new InputElement("TEXCOORD", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 3, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 3, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 3, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 4, Format.R32G32B32A32_Float, InputElement.AppendAligned, 3, InputClassification.PerInstanceData, 1),
                new InputElement("COLOR", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 4, InputClassification.PerInstanceData, 1),
                new InputElement("COLOR", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 4, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 5, Format.R32G32_Float, InputElement.AppendAligned, 4, InputClassification.PerInstanceData, 1),
    };

    /// <summary>
    /// Gets the vs input bone skinned basic.
    /// </summary>
    /// <value>
    /// The vs input bone skinned basic.
    /// </value>
    public static readonly InputElement[] VSInputBoneSkinnedBasic = new InputElement[]
    {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                new InputElement("NORMAL",   0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                new InputElement("TANGENT",  0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                new InputElement("BINORMAL", 0, Format.R32G32B32_Float,    InputElement.AppendAligned, 0),
                new InputElement("BONEIDS", 0, Format.R32G32B32A32_SInt, InputElement.AppendAligned, 1),
                new InputElement("BONEWEIGHTS", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1),
    };

    /// <summary>
    /// 
    /// </summary>
    public static readonly InputElement[] VSInputPoint = new InputElement[]
    {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                new InputElement("COLOR",    0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0),
                //INSTANCING: die 4 texcoords sind die matrix, die mit jedem buffer reinwandern
                new InputElement("TEXCOORD", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
    };

    /// <summary>
    /// 
    /// </summary>
    public static readonly InputElement[] VSInputBillboard = new InputElement[]
    {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float,  InputElement.AppendAligned, 0),
                new InputElement("COLOR",    0, Format.R32G32B32A32_Float,  InputElement.AppendAligned, 0),
                new InputElement("COLOR",    1, Format.R32G32B32A32_Float,  InputElement.AppendAligned, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float,  InputElement.AppendAligned, 0),
                new InputElement("TEXCOORD", 1, Format.R32G32_Float,  InputElement.AppendAligned, 0),
                new InputElement("TEXCOORD", 2, Format.R32G32_Float,  InputElement.AppendAligned, 0),
                new InputElement("TEXCOORD", 3, Format.R32G32_Float,  InputElement.AppendAligned, 0),
                new InputElement("TEXCOORD", 4, Format.R32G32_Float,  InputElement.AppendAligned, 0),
                new InputElement("TEXCOORD", 5, Format.R32G32_Float,  InputElement.AppendAligned, 0),
                //INSTANCING: die 4 texcoords sind die matrix, die mit jedem buffer reinwandern
                new InputElement("TEXCOORD", 6, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 7, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 8, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 9, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
    };
    /// <summary>
    /// Gets the vs input billboard instancing.
    /// </summary>
    /// <value>
    /// The vs input billboard instancing.
    /// </value>
    public static readonly InputElement[] VSInputBillboardInstancing = new InputElement[]
    {
                new InputElement("POSITION", 0, Format.R32G32B32A32_Float,  InputElement.AppendAligned, 0),
                new InputElement("COLOR",    0, Format.R32G32B32A32_Float,  InputElement.AppendAligned, 0),
                new InputElement("COLOR",    1, Format.R32G32B32A32_Float,  InputElement.AppendAligned, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float,  InputElement.AppendAligned, 0),
                new InputElement("TEXCOORD", 1, Format.R32G32_Float,  InputElement.AppendAligned, 0),
                new InputElement("TEXCOORD", 2, Format.R32G32_Float,  InputElement.AppendAligned, 0),
                new InputElement("TEXCOORD", 3, Format.R32G32_Float,  InputElement.AppendAligned, 0),
                new InputElement("TEXCOORD", 4, Format.R32G32_Float,  InputElement.AppendAligned, 0),
                new InputElement("TEXCOORD", 5, Format.R32G32_Float,  InputElement.AppendAligned, 0),
                //INSTANCING: die 4 texcoords sind die matrix, die mit jedem buffer reinwandern
                new InputElement("TEXCOORD", 6, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 7, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 8, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 9, Format.R32G32B32A32_Float, InputElement.AppendAligned, 1, InputClassification.PerInstanceData, 1),
                new InputElement("COLOR", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 2, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 10, Format.R32G32_Float, InputElement.AppendAligned, 2, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 11, Format.R32G32_Float, InputElement.AppendAligned, 2, InputClassification.PerInstanceData, 1),
    };
    /// <summary>
    /// Gets the vs input particle.
    /// </summary>
    /// <value>
    /// The vs input particle.
    /// </value>
    public static readonly InputElement[] VSInputParticle = new InputElement[]
    {
                new InputElement("TEXCOORD", 1, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 2, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 3, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0, InputClassification.PerInstanceData, 1),
                new InputElement("TEXCOORD", 4, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0, InputClassification.PerInstanceData, 1),
    };
    /// <summary>
    /// Gets the vs input skybox.
    /// </summary>
    /// <value>
    /// The vs input skybox.
    /// </value>
    public static readonly InputElement[] VSInputSkybox = new InputElement[]
    {
                new InputElement("SV_POSITION", 0, Format.R32G32B32_Float,  InputElement.AppendAligned, 0),
    };
    /// <summary>
    /// Gets the vs input sprite 2d.
    /// </summary>
    /// <value>
    /// The vs input sprite 2d.
    /// </value>
    public static readonly InputElement[] VSInputSprite2D = new InputElement[]
    {
                new InputElement("POSITION", 0, Format.R32G32_Float,  InputElement.AppendAligned, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float,  InputElement.AppendAligned, 0),
                new InputElement("COLOR", 0, Format.R32G32B32A32_Float,  InputElement.AppendAligned, 0),
    };

    /// <summary>
    /// Gets the vs input volume3d.
    /// </summary>
    /// <value>
    /// The vs input volume3d.
    /// </value>
    public static readonly InputElement[] VSInputVolume3D = new InputElement[]
    {
                new InputElement("SV_POSITION", 0, Format.R32G32B32_Float, InputElement.AppendAligned, 0),
    };
}
