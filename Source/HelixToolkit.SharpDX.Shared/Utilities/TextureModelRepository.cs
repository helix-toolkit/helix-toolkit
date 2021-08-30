/*
The MIT License (MIT)
Copyright (c) 2021 Helix Toolkit contributors
*/
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

#if !NETFX_CORE && !WINUI_NET5_0
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI_NET5_0
namespace HelixToolkit.WinUI
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

            private readonly ConditionalWeakTable<string, WeakReference<TextureModel>> fileDict
                = new ConditionalWeakTable<string, WeakReference<TextureModel>>();

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

            public TextureModel Create(string texturePath)
            {
                if (string.IsNullOrEmpty(texturePath))
                {
                    return null;
                }
                lock (fileDict)
                {
                    if (fileDict.TryGetValue(texturePath, out var tex))
                    {
                        if (tex.TryGetTarget(out var target))
                        {
                            Debug.WriteLine($"Reuse existing TextureModel. Guid: {target.Guid}");
                            return target;
                        }
                        fileDict.Remove(texturePath);
                    }
                    var newTexModel = new TextureModel(texturePath);
                    fileDict.Add(texturePath, new WeakReference<TextureModel>(newTexModel));
                    Debug.WriteLine($"Created new TextureModel. Guid: {newTexModel.Guid}");
                    return newTexModel;
                }
            }
        }
    }
}
