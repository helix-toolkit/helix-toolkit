/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Runtime.Serialization;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public sealed class TechniqueDescription
    {
        /// <summary>
        /// Technique Name
        /// </summary>
        [DataMember]
        public string Name { set; get; }

        /// <summary>
        /// Input Layout
        /// </summary>
        [DataMember]
        public InputLayoutDescription InputLayoutDescription { set; get; }
        /// <summary>
        /// Gets or sets the pass descriptions.
        /// </summary>
        /// <value>
        /// The pass descriptions.
        /// </value>
        [DataMember]
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
    [DataContract]
    public sealed class ShaderPassDescription
    {
        /// <summary>
        /// Pass Name
        /// </summary>
        [DataMember]
        public string Name { set; get; }
        /// <summary>
        /// Shaders for this technique
        /// </summary>
        [DataMember]
        public IList<ShaderDescription> ShaderList { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public BlendStateDescription? BlendStateDescription { set; get; } = null;
        /// <summary>
        /// Gets or sets the color of the blend.
        /// </summary>
        /// <value>
        /// The color of the blend.
        /// </value>
        [DataMember]
        public Color4 BlendFactor { set; get; } = Color4.White;
        /// <summary>
        /// Gets or sets the sample factor.
        /// </summary>
        /// <value>
        /// The sample factor.
        /// </value>
        [DataMember]
        public int SampleFactor { set; get; } = -1;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DepthStencilStateDescription? DepthStencilStateDescription { set; get; } = null;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public RasterizerStateDescription? RasterStateDescription { set; get; } = null;

        public ShaderPassDescription() { }

        public ShaderPassDescription(string name)
        {
            Name = name;
        }
    }
}
