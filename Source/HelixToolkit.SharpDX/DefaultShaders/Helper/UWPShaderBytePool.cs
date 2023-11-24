namespace HelixToolkit.SharpDX.Helper;

/// <summary>
/// Used to read shader bytecode
/// </summary>
public static class UWPShaderBytePool
{
    public static readonly Dictionary<string, byte[]> Dict = new();

    internal static readonly IShaderByteCodeReader InternalByteCodeReader = new HelixToolkitByteCodeReader();

    public static byte[] Read(string name, IShaderByteCodeReader? reader = null)
    {
        lock (Dict)
        {
            if (!Dict.TryGetValue(name, out var byteCode))
            {
                lock (Dict)
                {
                    if (!Dict.TryGetValue(name, out byteCode))
                    {
                        if (reader == null)
                        {
                            byteCode = InternalByteCodeReader.Read(name);
                        }
                        else
                        {
                            byteCode = reader.Read(name);
                        }
                        Dict.Add(name, byteCode);
                    }
                }
            }
            return byteCode;
        }
    }
}
