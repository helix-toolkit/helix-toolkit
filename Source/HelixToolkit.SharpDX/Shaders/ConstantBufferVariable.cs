namespace HelixToolkit.SharpDX.Shaders;

public struct ConstantBufferVariable
{
    //
    // Summary:
    //     The variable name.
    public string Name;
    //
    // Summary:
    //     Offset from the start of the parent structure to the beginning of the variable.
    public int StartOffset;
    //
    // Summary:
    //     Size of the variable (in bytes).
    public int Size;
}
