/*
The MIT License (MIT)
Copyright (c) 2021 Helix Toolkit contributors
*/
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

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
        public sealed class TextureModelRepository : ITextureModelRepository
        {
            private readonly ConditionalWeakTable<Stream, WeakReference<TextureModel>> streamDict
                = new ConditionalWeakTable<Stream, WeakReference<TextureModel>>();

            public ITextureFileLoader TextureFileLoader
            {
                set; get;
            }

            public TextureModelRepository()
            {
#if WINDOWS_UWP
                TextureFileLoader = new UWPTextureFileLoader();
#else
                TextureFileLoader = new DefaultTextureFileLoader();
#endif
            }

            public TextureModelRepository(ITextureFileLoader loader)
            {
                TextureFileLoader = loader;
            }

            public Stream Load(string texturePath)
            {
                Debug.WriteLine($"Loading texture from {texturePath}.");
                return TextureFileLoader?.Load(texturePath);
            }

            public TextureModel Create(Stream stream)
            {
                if (stream == null)
                {
                    return null;
                }
                lock (streamDict)
                {
                    if (streamDict.TryGetValue(stream, out var tex))
                    {
                        if (tex.TryGetTarget(out var target))
                        {
                            Debug.WriteLine($"Reuse existing TextureModel. Guid: {target.Guid}");
                            return target;
                        }
                        streamDict.Remove(stream);
                    }
                    var newTexModel = new TextureModel(stream);
                    streamDict.Add(stream, new WeakReference<TextureModel>(newTexModel));
                    Debug.WriteLine($"Created new TextureModel. Guid: {newTexModel.Guid}");
                    return newTexModel;
                }
            }
        }

        public class DefaultTextureFileLoader : ITextureFileLoader
        {
            public Stream Load(string texturePath)
            {
                try
                {
                    if (!FileExists(texturePath))
                    {
                        Debug.WriteLine($"Load Texture Failed. Texture Path = {texturePath}.");
                        return null;
                    }
                    return LoadFileToStream(texturePath);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Load Texture Exception. Texture Path = {texturePath}. Exception: {ex.Message}");
                }
                return null;
            }

            protected virtual bool FileExists(string path)
            {
                return File.Exists(path);
            }

            protected virtual Stream LoadFileToStream(string path)
            {
                return File.OpenRead(path);
            }
        }
#if WINDOWS_UWP
        public class UWPTextureFileLoader : DefaultTextureFileLoader
        {
            protected override bool FileExists(string path)
            {
                try
                {
                    Windows.Storage.StorageFile.GetFileFromPathAsync(path).AsTask().GetAwaiter().GetResult();
                    return true;
                }
                catch (Exception) { }
                return false;
            }

            protected override Stream LoadFileToStream(string path)
            {
                var folder = Windows.Storage.StorageFile.GetFileFromPathAsync(path).AsTask().GetAwaiter().GetResult();
                if (folder != null)
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
#endif
    }
}
