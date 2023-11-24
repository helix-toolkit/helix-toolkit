using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct VolumeParamsStruct
{
    public const string World = "mWorld"; //Separated from the struct in material
    public const string WorldInv = "mWorldInv"; // Inverse of world matrix
    public const string Color = "pColor"; // Vector4
    public const string StepSize = "stepSize"; // Vector3
    public const string MaxIterations = "maxIterations"; // int or uint
    public const string HasGradientMapX = "bHasGradientMapX";// bool
    public const string IsoValue = "isoValue"; // float
    public const string BaseSampleDistance = "baseSampleDist"; //float
    public const string ActualSampleDistance = "actualSampleDist"; //float
    public const string IterationOffset = "iterationOffset"; // int or uint
    public const string EnablePlaneAlignment = "enablePlaneAlignment"; // bool
    public const int SizeInBytes = 4 * (4 * 4 + 4 * 4 + 4 + 4 + 4);
}
