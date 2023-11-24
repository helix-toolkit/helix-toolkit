using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

public static class BinaryReaderExtensions
{
    /// <summary>
    /// Loads a string from the CMO file (WCHAR prefixed with uint length)
    /// </summary>
    /// <param name="br"></param>
    /// <returns></returns>
    public static string ReadCMO_wchar(this BinaryReader br)
    {
        // uint - Length of string (in WCHAR's i.e. 2-bytes)
        // wchar[] - string (if length > 0)
        var length = (int)br.ReadUInt32();
        if (length > 0)
        {
            var result = System.Text.Encoding.Unicode.GetString(br.ReadBytes(length * 2), 0, length * 2);
            // Remove the trailing \0
            return result[..^1];
        }

        return string.Empty;
    }

    /// <summary>
    /// Read a structure from binary reader
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="br"></param>
    /// <returns></returns>
    public static T ReadStructure<T>(this BinaryReader br) where T : unmanaged
    {
        return ByteArrayToStructure<T>(br.ReadBytes(global::SharpDX.Utilities.SizeOf<T>()));
    }

    /// <summary>
    /// Read <paramref name="count"/> instances of the structure from the binary reader.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="br"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static T[] ReadStructure<T>(this BinaryReader br, int count) where T : unmanaged
    {
        var result = new T[count];

        for (var i = 0; i < count; i++)
            result[i] = ByteArrayToStructure<T>(br.ReadBytes(global::SharpDX.Utilities.SizeOf<T>()));

        return result;
    }

    /// <summary>
    /// Read <paramref name="count"/> UInt16s.
    /// </summary>
    /// <param name="br"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static ushort[] ReadUInt16(this BinaryReader br, int count)
    {
        var result = new ushort[count];
        for (var i = 0; i < count; i++)
            result[i] = br.ReadUInt16();
        return result;
    }

    static T ByteArrayToStructure<T>(byte[] bytes) where T : unmanaged
    {
        var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        T stuff = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
        handle.Free();
        return stuff;
    }
}
