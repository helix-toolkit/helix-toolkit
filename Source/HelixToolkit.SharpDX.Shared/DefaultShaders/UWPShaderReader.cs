using System.Collections.Generic;
using System.IO;
#if NETFX_CORE
namespace HelixToolkit.UWP.Helper
{
    public static class UWPShaderBytePool
    {
        public static Dictionary<string, byte[]> Dict = new Dictionary<string, byte[]>();
        public static byte[] Read(string name)
        {
            lock (Dict)
            {
                if (Dict.ContainsKey(name))
                {
                    return Dict[name];
                }
                else
                {
                    var packageFolder = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "HelixToolkit.UWP");
                    var bytecode = global::SharpDX.IO.NativeFile.ReadAllBytes(packageFolder + @"\Resources\" + name + @".cso");
                    Dict.Add(name, bytecode);
                    return bytecode;
                }
            }
        }
    }
}
#endif