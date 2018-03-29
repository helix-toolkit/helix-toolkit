/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;
    public class MeshOutlineNode : MeshNode
    {
        public bool EnableOutline
        {
            set
            {
                (RenderCore as IMeshOutlineParams).OutlineEnabled = value;
            }
            get
            {
                return (RenderCore as IMeshOutlineParams).OutlineEnabled;
            }
        }

        public Color4 OutlineColor
        {
            set
            {
                (RenderCore as IMeshOutlineParams).Color = value;
            }
            get
            {
                return (RenderCore as IMeshOutlineParams).Color;
            }
        }

        public bool IsDrawGeometry
        {
            set
            {
                (RenderCore as IMeshOutlineParams).DrawMesh = value;
            }
            get
            {
                return (RenderCore as IMeshOutlineParams).DrawMesh;
            }
        }

        public float OutlineFadingFactor
        {
            set
            {
                (RenderCore as IMeshOutlineParams).OutlineFadingFactor = value;
            }
            get
            {
                return (RenderCore as IMeshOutlineParams).OutlineFadingFactor;
            }
        }

        protected override RenderCore OnCreateRenderCore()
        {
            return new MeshOutlineRenderCore();
        }
    }
}
