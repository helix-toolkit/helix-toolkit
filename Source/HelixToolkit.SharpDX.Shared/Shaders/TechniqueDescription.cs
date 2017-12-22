/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Text;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    public class TechniqueDescription
    {
        /// <summary>
        /// Technique Name
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// Input Layout
        /// </summary>
        public InputLayoutDescription InputLayoutDescription { set; get; }

        public IList<ShaderPassDescription> PassDescriptions { set; get; }

        public TechniqueDescription() { }

        public TechniqueDescription(string name)
        {
            Name = name;
        }
    }

    public class ShaderPassDescription
    {        
        /// <summary>
        /// Pass Name
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// Shaders for this technique
        /// </summary>
        public IList<ShaderDescription> ShaderList { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public BlendStateDescription? BlendStateDescription { set; get; } = null;

        /// <summary>
        /// 
        /// </summary>
        public DepthStencilStateDescription? DepthStencilStateDescription { set; get; } = null;
        /// <summary>
        /// 
        /// </summary>
        public RasterizerStateDescription? RasterStateDescription { set; get; } = null;

        public ShaderPassDescription() { }

        public ShaderPassDescription(string name)
        {
            Name = name;
        }
    }
}
