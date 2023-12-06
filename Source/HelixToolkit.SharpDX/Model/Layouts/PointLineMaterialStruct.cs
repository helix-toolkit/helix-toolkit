using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// Used combine with <see cref="PointLineModelStruct"/>
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct PointLineMaterialStruct
{
    //public Vector4 Params;
    //public Vector4 Color;
    //public Bool4 BoolParams;

    public const int SizeInBytes = 4 * (4 * 5) + PointLineModelStruct.SizeInBytes;
    public const string FadeNearDistance = "fadeNearDistance";//float
    public const string FadeFarDistance = "fadeFarDistance";//float
    public const string EnableDistanceFading = "enableDistanceFading";//bool
    public const string ParamsStr = "pfParams";//vector4
    public const string ColorStr = "pColor";//vector4
    public const string FixedSize = "fixedSize";//bool
    public const string BoolParamsStr = "pbParams";//bool3
    public const string HasTextureStr = "bHasTexture"; //bool
    public const string TextureScaleStr = "pTextureScale";//float;
    public const string AlphaThresholdStr = "pAlphaThreshold"; // float; 
    public const string EnableBlendingStr = "pEnableBlending"; //bool
    public const string BlendingFactorStr = "pBlendingFactor"; //float
}
