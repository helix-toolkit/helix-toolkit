/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if WINUI
using HelixToolkit.SharpDX.Core.Model.Scene;
namespace HelixToolkit.WinUI
#else
namespace HelixToolkit.UWP
#endif
{
#if !WINUI
    using Model.Scene;
#endif
    public sealed class AmbientLight3D : Light3D
    {
        protected override SceneNode OnCreateSceneNode()
        {
            return new AmbientLightNode();
        }
    }
}