namespace HelixToolkit.SharpDX.Assimp;

/// <summary>
///
/// </summary>
[Flags]
public enum ErrorCode
{
    None = 0,
    Failed = 1,
    Succeed = 1 << 1,
    DuplicateNodeName = 1 << 2,
    FileTypeNotSupported = 1 << 3,
    NonUniformAnimationKeyDoesNotSupported = 1 << 4
}
