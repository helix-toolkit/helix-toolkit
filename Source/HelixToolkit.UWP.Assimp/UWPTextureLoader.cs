using HelixToolkit.Logger;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Windows.Storage;

namespace HelixToolkit.UWP.Assimp
{
    public class UWPTextureLoader : DefaultTexturePathResolver
    {
        protected override bool FileExists(string path)
        {
            try
            {
                StorageFile.GetFileFromPathAsync(path).AsTask().GetAwaiter().GetResult();
                return true;
            }
            catch (Exception) { }
            return false;
        }
    }
}
