using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// Used combine with <see cref="PhongPBRMaterialStruct"/>
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct ModelStruct
{
    public Matrix World;
    public int InvertNormal;
    public int HasInstances;
    public int HasInstanceParams;
    public int HasBones;
    public Vector4 Params;
    public Vector4 Color;
    public Color4 WireframeColor;

    public Int3 BoolParams;
    public int Batched;
    public const int SizeInBytes = 4 * (4 * 4 + 4 + 4 * 2 + 4 + 4);

    public const string WorldStr = "mWorld";
    public const string InvertNormalStr = "bInvertNormal";
    public const string HasInstancesStr = "bHasInstances";
    public const string HasInstanceParamsStr = "bHasInstanceParams";
    public const string HasBonesStr = "bHasBones";
    public const string ParamsStr = "vParams";
    public const string ColorStr = "vColor";
    public const string BoolParamsStr = "bParams";
    public const string BatchedStr = "bBatched";
    public const string WireframeColorStr = "wireframeColor";
}
