/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;
    public sealed class AmbientLightNode : LightNode
    {
        protected override RenderCore OnCreateRenderCore()
        {
            return new AmbientLightCore();
        }
    }
}
