/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if NETFX_CORE
namespace HelixToolkit.UWP
#elif WINUI
using HelixToolkit.SharpDX.Core.Model.Scene;
namespace HelixToolkit.WinUI
#else
#if COREWPF
using HelixToolkit.SharpDX.Core.Model.Scene;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
#if !COREWPF && !WINUI
    using Model.Scene;
#endif
    /// <summary>
    /// ScreenSpacedGroup3D uses a fixed camera to render model (Mainly used for view box and coordinate system rendering) onto screen which is separated from viewport camera.
    /// <para>
    /// Default fix camera is perspective camera with FOV 45 degree and camera distance = 20. Look direction is always looking at (0,0,0).
    /// </para>
    /// <para>
    /// User must properly scale the model to fit into the camera frustum. The usual maximum size is from (5,5,5) to (-5,-5,-5) bounding box.
    /// </para>
    /// <para>
    /// User can use <see cref="ScreenSpacedElement3D.SizeScale"/> to scale the size of the rendering.
    /// </para>
    /// </summary>
    public sealed class ScreenSpacedGroup3D : ScreenSpacedElement3D
    {
        protected override SceneNode OnCreateSceneNode()
        {
            return new ScreenSpacedNode();
        }
    }
}
