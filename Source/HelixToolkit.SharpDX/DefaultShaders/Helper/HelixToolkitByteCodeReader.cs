using System.Reflection;

namespace HelixToolkit.SharpDX.Helper;

/// <summary>
/// Used to read HelixToolkit internal default shader byte codes
/// </summary>
public sealed class HelixToolkitByteCodeReader : IShaderByteCodeReader
{
    public byte[] Read(string name)
    {
        // todo

        var assembly = typeof(UWPShaderBytePool).GetTypeInfo().Assembly;
        Stream shaderStream = assembly.GetManifestResourceStream($"HelixToolkit.SharpDX.Resources.{name}.cso")
            ?? throw new System.Exception($"Shader byte code is not read. Shader Name: {name}");
        using var memory = new MemoryStream();
        shaderStream.CopyTo(memory);
        return memory.ToArray();

        /*
        #if CORE
                        var assembly = typeof(UWPShaderBytePool).GetTypeInfo().Assembly;
                        Stream shaderStream = assembly.GetManifestResourceStream($"HelixToolkit.SharpDX.Core.Resources.{name}.cso");
                        if (shaderStream == null)
                        {
                            throw new System.Exception($"Shader byte code is not read. Shader Name: {name}");
                        }
                        using (var memory = new MemoryStream())
                        {
                            shaderStream.CopyTo(memory);
                            return memory.ToArray();
                        }
        #elif NETFX_CORE
                        var filePath = Windows.ApplicationModel.Package.Current.InstalledLocation.Path + $"\\HelixToolkit.UWP\\Resources\\{name}.cso";
                        if (!File.Exists(filePath))
                        {
                            throw new System.Exception($"Shader byte code is not read. Shader Name: {name}");
                        }
                        var byteCode = global::SharpDX.IO.NativeFile.ReadAllBytes(filePath);
                        if(byteCode == null)
                        {
                            throw new System.Exception($"Shader byte code is not read. Shader Name: {name}");
                        }
                        return byteCode;
        #else
                var byteCode = Properties.Resources.ResourceManager.GetObject(name) as byte[];
                if (byteCode == null)
                {
                    throw new System.Exception($"Shader byte code is not read. Shader Name: {name}");
                }
                return byteCode;
        #endif
        */
    }
}
