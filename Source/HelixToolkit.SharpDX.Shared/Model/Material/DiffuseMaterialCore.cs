/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
using System.IO;
using System.Runtime.Serialization;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Model
    {
        using Shaders;
        
        [DataContract]
        public class DiffuseMaterialCore : MaterialCore
        {
            private Color4 diffuseColor = Color.White;
            /// <summary>
            /// Gets or sets the color of the diffuse.
            /// </summary>
            /// <value>
            /// The color of the diffuse.
            /// </value>
            public Color4 DiffuseColor
            {
                set { Set(ref diffuseColor, value); }
                get { return diffuseColor; }
            }
            private TextureModel diffuseMap;
            /// <summary>
            /// Gets or sets the diffuse map.
            /// </summary>
            /// <value>
            /// The diffuse map.
            /// </value>
            public TextureModel DiffuseMap
            {
                set { Set(ref diffuseMap, value); }
                get { return diffuseMap; }
            }
            /// <summary>
            /// Gets or sets the diffuse map file path. Only for export
            /// </summary>
            /// <value>
            /// The diffuse map file path.
            /// </value>
            public string DiffuseMapFilePath { set; get; }

            private UVTransform uvTransform = UVTransform.Identity;
            /// <summary>
            /// Gets or sets the uv transform.
            /// </summary>
            /// <value>
            /// The uv transform.
            /// </value>
            public UVTransform UVTransform
            {
                set { Set(ref uvTransform, value); }
                get { return uvTransform; }
            }
            private SamplerStateDescription diffuseMapSampler = DefaultSamplers.LinearSamplerWrapAni4;
            /// <summary>
            /// Gets or sets the DiffuseMapSampler.
            /// </summary>
            /// <value>
            /// DiffuseMapSampler
            /// </value>
            public SamplerStateDescription DiffuseMapSampler
            {
                set { Set(ref diffuseMapSampler, value); }
                get { return diffuseMapSampler; }
            }

            private bool renderDiffuseMap = true;
            /// <summary>
            /// 
            /// </summary>
            public bool RenderDiffuseMap
            {
                set
                {
                    Set(ref renderDiffuseMap, value);
                }
                get { return renderDiffuseMap; }
            }

            private bool enableUnLit = false;
            /// <summary>
            /// Gets or sets a value indicating whether disable lighting. Directly render diffuse color and diffuse map
            /// </summary>
            /// <value>
            ///   <c>true</c> if [enable un lit]; otherwise, <c>false</c>.
            /// </value>
            public bool EnableUnLit
            {
                set { Set(ref enableUnLit, value); }
                get { return enableUnLit; }
            }

            private bool enableFlatShading = false;
            /// <summary>
            /// Gets or sets a value indicating whether [enable flat shading].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [enable flat shading]; otherwise, <c>false</c>.
            /// </value>
            public bool EnableFlatShading
            {
                set { Set(ref enableFlatShading, value); }
                get { return enableFlatShading; }
            }

            private float vertexColorBlendingFactor = 0f;
            /// <summary>
            /// Gets or sets the vert color blending factor.
            /// Diffuse = (1- <see cref="VertexColorBlendingFactor"/>) * Diffuse + <see cref="VertexColorBlendingFactor"/> * Vertex Color
            /// </summary>
            /// <value>
            /// The vert color blending factor.
            /// </value>
            public float VertexColorBlendingFactor
            {
                set { Set(ref vertexColorBlendingFactor, value); }
                get { return vertexColorBlendingFactor; }
            }

            public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
            {
                return new DiffuseMaterialVariables(DefaultPassNames.Diffuse, manager, technique, this);
            }
        }

        public sealed class ViewCubeMaterialCore : DiffuseMaterialCore
        {
            public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
            {
                return new DiffuseMaterialVariables(DefaultPassNames.ViewCube, manager, technique, this);
            }
        }
    }

}
