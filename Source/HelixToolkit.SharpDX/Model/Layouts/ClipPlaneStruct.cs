using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct ClipPlaneStruct
{
    //public Matrix CrossPlaneParams;
    public const int SizeInBytes = 4 * (4 * 4 + 4 * 8);

    public const string EnableCrossPlaneStr = "EnableCrossPlane";
    public const string EnableCrossPlane5To8Str = "EnableCrossPlane5To8";
    public const string CrossSectionColorStr = "CrossSectionColors";
    public const string CuttingOperationStr = "CuttingOperation";
    public const string CrossPlane1ParamsStr = "CrossPlane1Params";
    public const string CrossPlane2ParamsStr = "CrossPlane2Params";
    public const string CrossPlane3ParamsStr = "CrossPlane3Params";
    public const string CrossPlane4ParamsStr = "CrossPlane4Params";
    public const string CrossPlane5ParamsStr = "CrossPlane5Params";
    public const string CrossPlane6ParamsStr = "CrossPlane6Params";
    public const string CrossPlane7ParamsStr = "CrossPlane7Params";
    public const string CrossPlane8ParamsStr = "CrossPlane8Params";
}
