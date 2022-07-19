/*
The MIT License (MIT)
Copyright (c) 2021 Helix Toolkit contributors
*/
using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Logging;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Utilities
    {

        public class TextureFileLoader : ITextureInfoLoader
        {
            static readonly ILogger logger = Logger.LogManager.Create<TextureFileLoader>();

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
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("Disposing file stream {}.", FilePath);
                }
                fileStream.Dispose();
            }

            public TextureInfo Load(Guid id)
            {
                logger.LogInformation("Loading texture file: {}", FilePath);

#if WINDOWS_UWP
                try
                {
                    Windows.Storage.StorageFile.GetFileFromPathAsync(FilePath).AsTask().GetAwaiter().GetResult();
                    var folder = Windows.Storage.StorageFile.GetFileFromPathAsync(FilePath).AsTask().GetAwaiter().GetResult();
                    fileStream = folder.OpenStreamForReadAsync().GetAwaiter().GetResult();
                    return new TextureInfo(fileStream);
                }
                catch (Exception) { }
                return TextureInfo.Null;
#else
                if (!File.Exists(FilePath))
                {
                    return TextureInfo.Null;
                }
                fileStream = File.OpenRead(FilePath);
                return new TextureInfo(fileStream);
#endif   
            }
        }
    }
}
