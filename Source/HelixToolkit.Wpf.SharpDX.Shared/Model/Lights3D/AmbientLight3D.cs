// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientLight3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if COREWPF
using HelixToolkit.SharpDX.Core.Model.Scene;
namespace HelixToolkit.SharpDX.Core.Wpf
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
#if !COREWPF
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