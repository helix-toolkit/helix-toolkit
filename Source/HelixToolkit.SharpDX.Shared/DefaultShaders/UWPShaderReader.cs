using System.Collections.Generic;
using System.IO;
using System.Reflection;
#if CORE
namespace HelixToolkit.UWP.Helper
#else
#if NETFX_CORE
namespace HelixToolkit.UWP.Helper
#else
namespace HelixToolkit.Wpf.SharpDX.Helper
#endif
#endif
{
    public static class UWPShaderBytePool
    {
        public static Dictionary<string, byte[]> Dict = new Dictionary<string, byte[]>();
        public static byte[] Read(string name)
        {
            lock (Dict)
            {
                byte[] byteCode;
                if (!Dict.TryGetValue(name, out byteCode))
                {
                    lock (Dict)
                    {
                        if (!Dict.TryGetValue(name, out byteCode))
                        {
#if CORE
                            var assembly = typeof(UWPShaderBytePool).GetTypeInfo().Assembly;
                            Stream shaderStream = assembly.GetManifestResourceStream($"HelixToolkit.SharpDX.Core.Resources.{name}.cso");
                            if(shaderStream == null)
                            {
                                throw new System.Exception($"Shader byte code is not read. Shader Name: {name}");
                            }
                            using (var memory = new MemoryStream())
                            {
                                shaderStream.CopyTo(memory);
                                byteCode = memory.ToArray();
                                Dict.Add(name, byteCode);
                            }

#else
#if NETFX_CORE
                            var packageFolder = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "HelixToolkit.UWP");
                            byteCode = global::SharpDX.IO.NativeFile.ReadAllBytes(packageFolder + @"\Resources\" + name + @".cso");
                            if(byteCode == null)
                            {
                                throw new System.Exception($"Shader byte code is not read. Shader Name: {name}");
                            }
                            Dict.Add(name, byteCode);
#else
                            byteCode = Properties.Resources.ResourceManager.GetObject(name) as byte[];
                            if(byteCode == null)
                            {
                                throw new System.Exception($"Shader byte code is not read. Shader Name: {name}");
                            }
                            Dict.Add(name, byteCode);
#endif
#endif
                        }

                    } 
                }
                return byteCode;
            }
        }
    }
}