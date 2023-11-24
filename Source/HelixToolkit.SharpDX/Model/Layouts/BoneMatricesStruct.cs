using SharpDX;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
//[StructLayout(LayoutKind.Sequential, Pack = 4)]
public static class BoneMatricesStruct
{
    //public const int NumberOfBones = 128;
    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = NumberOfBones)]
    //public Matrix[] Bones;
    //public const int SizeInBytes = 4 * (4 * 4 * NumberOfBones);
    public static readonly Matrix[] DefaultBones = Enumerable.Repeat(Matrix.Identity, 1).ToArray();
}
