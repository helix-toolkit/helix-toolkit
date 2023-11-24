using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// Used combine with <see cref="ModelStruct"/>
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct PhongPBRMaterialStruct
{
    public const int SizeInBytes = 4 * (4 + 4 * 5 + 4 + 4 + 4 + 4 * 3 + 4) + ModelStruct.SizeInBytes;

    public const string MinTessDistanceStr = "minTessDistance"; //float
    public const string MaxTessDistanceStr = "maxTessDistance";//float
    public const string MinDistTessFactorStr = "minTessFactor";//float
    public const string MaxDistTessFactorStr = "maxTessFactor";//float
    public const string DiffuseStr = "vMaterialDiffuse";//float4
    public const string AmbientStr = "vMaterialAmbient";//float4
    public const string EmissiveStr = "vMaterialEmissive";//float4
    public const string SpecularStr = "vMaterialSpecular";//float4
    public const string ReflectStr = "vMaterialReflect";//float4

    public const string HasDiffuseMapStr = "bHasDiffuseMap";//bool
    public const string HasNormalMapStr = "bHasNormalMap";//bool
    public const string HasCubeMapStr = "bHasCubeMap";//bool
    public const string RenderShadowMapStr = "bRenderShadowMap";//bool
    public const string HasSpecularColorMap = "bHasSpecularMap";
    public const string HasDiffuseAlphaMapStr = "bHasAlphaMap";//bool

    public const string AmbientOcclusionStr = "ConstantAO";
    public const string RoughnessStr = "ConstantRoughness";
    public const string ConstantMetallic = "ConstantMetallic";
    public const string ReflectanceStr = "ConstantReflectance";
    public const string ClearCoatStr = "ClearCoat";
    public const string ClearCoatRoughnessStr = "ClearCoatRoughness";

    public const string HasRMMapStr = "bHasRMMap";//bool
    public const string HasAOMapStr = "bHasAOMap";//bool
    public const string HasEmissiveMapStr = "bHasEmissiveMap";//bool
    public const string HasIrradianceMapStr = "bHasIrradianceMap";//bool
    public const string EnableAutoTangent = "bAutoTengent";//bool

    public const string HasDisplacementMapStr = "bHasDisplacementMap";//bool
    public const string RenderPBR = "bRenderPBR";//bool
    public const string RenderFlat = "bRenderFlat";//bool
    public const string ShininessStr = "sMaterialShininess";//float

    public const string DisplacementMapScaleMaskStr = "displacementMapScaleMask";//float4

    public const string UVTransformR1Str = "uvTransformR1";//float4
    public const string UVTransformR2Str = "uvTransformR2";//float4

    public const string VertColorBlending = "vertColorBlending"; //float
}
