// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientLight3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using Model.Scene;
    public sealed class AmbientLight3D : Light3D
    {
        protected override SceneNode OnCreateSceneNode()
        {
            return new AmbientLightNode();
        }
    }
}