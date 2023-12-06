using System.Runtime.Serialization;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
[Flags]
[DataContract]
public enum ShaderStage
{
    [EnumMember]
    None = 0,
    [EnumMember]
    Vertex = 1,
    [EnumMember]
    Hull = 1 << 2,
    [EnumMember]
    Domain = 1 << 3,
    [EnumMember]
    Geometry = 1 << 4,
    [EnumMember]
    Pixel = 1 << 5,
    [EnumMember]
    Compute = 1 << 6
}
