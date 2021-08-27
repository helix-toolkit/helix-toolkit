/*
The MIT License (MIT)
Copyright (c) 2021 Helix Toolkit contributors
*/
using System;
using System.Diagnostics;
using System.IO;

#if !NETFX_CORE && !NET5_0
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI
namespace HelixToolkit.WinUI
#else
namespace TT.HelixToolkit.UWP
#endif
#endif
{
    namespace Utilities
    {
#if WINDOWS_UWP
        public class TextureFileLoader : ITextureInfoLoader
        {
            public string FilePath
            {
                get;
            }

            private Stream fileStream = Stream.Null;

            public TextureFileLoader(string filePath)
            {
                FilePath = filePath;
            }

            public void Complete(Guid id, TextureInfo info, bool succeeded)
            {
                Debug.WriteLine("Disposing file stream.");
                fileStream.Dispose();
            }

            public TextureInfo Load(Guid id)
            {
                try
                {
                    Windows.Storage.StorageFile.GetFileFromPathAsync(FilePath).AsTask().GetAwaiter().GetResult();
                    var folder = Windows.Storage.StorageFile.GetFileFromPathAsync(FilePath).AsTask().GetAwaiter().GetResult();
                    fileStream = folder.OpenStreamForReadAsync().GetAwaiter().GetResult();
                    return new TextureInfo(fileStream);
                }
                catch (Exception) { }
                return TextureInfo.Null;
            }
        }
#else
        public class TextureFileLoader : ITextureInfoLoader
        {
            public string FilePath
            {
                get;
            }

            private Stream fileStream = FileStream.Null;

            public TextureFileLoader(string filePath)
            {
                FilePath = filePath;
            }
            public void Complete(Guid id, TextureInfo info, bool succeeded)
            {
                Debug.WriteLine("Disposing file stream.");
                fileStream.Dispose();
            }

            public TextureInfo Load(Guid id)
            {
                if (!File.Exists(FilePath))
                {
                    return TextureInfo.Null;
                }
                fileStream = File.OpenRead(FilePath);
                return new TextureInfo(fileStream);
            }
        }
#endif    
    }
}
