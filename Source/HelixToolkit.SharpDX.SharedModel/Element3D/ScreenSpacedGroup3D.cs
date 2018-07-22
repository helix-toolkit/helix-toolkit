/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model.Scene;
    public sealed class ScreenSpacedGroup3D : ScreenSpacedElement3D
    {
        protected override SceneNode OnCreateSceneNode()
        {
            return new ScreenSpacedNode();
        }
    }
}
