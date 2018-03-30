/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;
    /// <summary>
    /// 
    /// </summary>
    public sealed class NodeDirectionalLight : NodeLight
    {
        public Vector3 Direction
        {
            set { (RenderCore as DirectionalLightCore).Direction = value; }
            get { return (RenderCore as DirectionalLightCore).Direction; }
        }

        protected override RenderCore OnCreateRenderCore()
        {
            return new DirectionalLightCore();
        }
    }
}
