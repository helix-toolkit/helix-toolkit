/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
namespace HelixToolkit.UWP
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