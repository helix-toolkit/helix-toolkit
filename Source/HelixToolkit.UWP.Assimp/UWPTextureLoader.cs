using HelixToolkit.Logger;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Windows.Storage;

namespace HelixToolkit.UWP.Assimp
{
    public class UWPTextureLoader : DefaultTextureLoader
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

        protected override Stream LoadFileToStream(string path)
        {
            var folder = StorageFile.GetFileFromPathAsync(path).AsTask().GetAwaiter().GetResult();
            if(folder != null)
            {
                var m = new MemoryStream();
                var result = folder.OpenStreamForReadAsync().GetAwaiter().GetResult();
                result.CopyTo(m);
                return m;
            }
            else
            {
                return null;
            }
        }
    }
}
