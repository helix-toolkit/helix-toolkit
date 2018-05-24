/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
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
    /// <summary>
    /// 
    /// </summary>
    public sealed class TechniqueDescription
    {
        /// <summary>
        /// Technique Name
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// Input Layout
        /// </summary>
        public InputLayoutDescription InputLayoutDescription { set; get; }
        /// <summary>
        /// Gets or sets the pass descriptions.
        /// </summary>
        /// <value>
        /// The pass descriptions.
        /// </value>
        public IList<ShaderPassDescription> PassDescriptions { set; get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="TechniqueDescription"/> class.
        /// </summary>
        public TechniqueDescription() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TechniqueDescription"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public TechniqueDescription(string name)
        {
            Name = name;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TechniqueDescription"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="inputLayout">The input layout.</param>
        public TechniqueDescription(string name, InputLayoutDescription inputLayout)
            : this(name)
        {
            InputLayoutDescription = inputLayout;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TechniqueDescription"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="inputLayout">The input layout.</param>
        /// <param name="shaderPasses">The shader passes.</param>
        public TechniqueDescription(string name, InputLayoutDescription inputLayout, IList<ShaderPassDescription> shaderPasses)
            :this(name, inputLayout)
        {
            PassDescriptions = shaderPasses;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class ShaderPassDescription
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
        /// Gets or sets the color of the blend.
        /// </summary>
        /// <value>
        /// The color of the blend.
        /// </value>
        public Color4 BlendFactor { set; get; } = Color4.White;
        /// <summary>
        /// Gets or sets the sample factor.
        /// </summary>
        /// <value>
        /// The sample factor.
        /// </value>
        public int SampleFactor { set; get; } = -1;
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
