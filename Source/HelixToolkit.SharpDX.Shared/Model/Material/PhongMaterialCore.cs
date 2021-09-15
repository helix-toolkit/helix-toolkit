/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
using System.IO;
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
        using Utilities;
        /// <summary>
        /// 
        /// </summary>
        public partial class PhongMaterialCore : MaterialCore
        {
            private Color4 ambientColor = Color.DarkGray;
            /// <summary>
            /// Gets or sets the color of the ambient.
            /// </summary>
            /// <value>
            /// The color of the ambient.
            /// </value>
            public Color4 AmbientColor
            {
                set
                {
                    Set(ref ambientColor, value);
                }
                get { return ambientColor; }
            }

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

            private Color4 emissiveColor = Color.Black;
            /// <summary>
            /// Gets or sets the color of the emissive.
            /// </summary>
            /// <value>
            /// The color of the emissive.
            /// </value>
            public Color4 EmissiveColor
            {
                set
                {
                    Set(ref emissiveColor, value);
                }
                get { return emissiveColor; }
            }

            private Color4 reflectiveColor = Color.Black;
            /// <summary>
            /// Gets or sets the color of the reflective.
            /// </summary>
            /// <value>
            /// The color of the reflective.
            /// </value>
            public Color4 ReflectiveColor
            {
                set { Set(ref reflectiveColor, value); }
                get { return reflectiveColor; }
            }

            private Color4 specularColor = Color.Gray;
            /// <summary>
            /// Gets or sets the color of the specular.
            /// </summary>
            /// <value>
            /// The color of the specular.
            /// </value>
            public Color4 SpecularColor
            {
                set { Set(ref specularColor, value); }
                get { return specularColor; }
            }

            private float specularShininess = 1;
            /// <summary>
            /// Gets or sets the specular shininess.
            /// </summary>
            /// <value>
            /// The specular shininess.
            /// </value>
            /// <exception cref="System.NotImplementedException">
            /// </exception>
            public float SpecularShininess
            {
                set { Set(ref specularShininess, value); }
                get { return specularShininess; }
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
            /// Gets or sets the diffuse map file path. For export only
            /// </summary>
            /// <value>
            /// The diffuse map file path.
            /// </value>
            public string DiffuseMapFilePath { set; get; }

            private TextureModel diffuseAlphaMap;
            /// <summary>
            /// Gets or sets the DiffuseAlphaMap.
            /// </summary>
            /// <value>
            /// DiffuseAlphaMap
            /// </value>
            public TextureModel DiffuseAlphaMap
            {
                set { Set(ref diffuseAlphaMap, value); }
                get { return diffuseAlphaMap; }
            }
            /// <summary>
            /// Gets or sets the diffuse alpha map file path. For export only
            /// </summary>
            /// <value>
            /// The diffuse alpha map file path.
            /// </value>
            public string DiffuseAlphaMapFilePath { set; get; }

            private TextureModel normalMap;
            /// <summary>
            /// Gets or sets the NormalMap.
            /// </summary>
            /// <value>
            /// NormalMap
            /// </value>
            public TextureModel NormalMap
            {
                set { Set(ref normalMap, value); }
                get { return normalMap; }
            }
            /// <summary>
            /// Gets or sets the normal map file path. For export only
            /// </summary>
            /// <value>
            /// The normal map file path.
            /// </value>
            public string NormalMapFilePath { set; get; }

            private TextureModel specularColorMap;
            /// <summary>
            /// Gets or sets the specular color map.
            /// </summary>
            /// <value>
            /// The specular color map.
            /// </value>
            public TextureModel SpecularColorMap
            {
                set { Set(ref specularColorMap, value); }
                get { return specularColorMap; }
            }
            /// <summary>
            /// Gets or sets the specular color map file path. For export only
            /// </summary>
            /// <value>
            /// The specular color map file path.
            /// </value>
            public string SpecularColorMapFilePath { set; get; }

            private TextureModel displacementMap;
            /// <summary>
            /// Gets or sets the DisplacementMap.
            /// </summary>
            /// <value>
            /// DisplacementMap
            /// </value>
            public TextureModel DisplacementMap
            {
                set { Set(ref displacementMap, value); }
                get { return displacementMap; }
            }
            /// <summary>
            /// Gets or sets the displacement file path. For export only
            /// </summary>
            /// <value>
            /// The displacement file path.
            /// </value>
            public string DisplacementMapFilePath { set; get; }

            private TextureModel emissiveMap;
            /// <summary>
            /// Gets or sets the emissive map.
            /// </summary>
            /// <value>
            /// The emissive map.
            /// </value>
            public TextureModel EmissiveMap
            {
                set { Set(ref emissiveMap, value); }
                get { return emissiveMap; }
            }
            /// <summary>
            /// Gets or sets the emissive map file path. For export only
            /// </summary>
            /// <value>
            /// The emissive map file path.
            /// </value>
            public string EmissiveMapFilePath { set; get; }

            private Vector4 displacementMapScaleMask;
            /// <summary>
            /// Gets or sets the DisplacementMapScaleMask.
            /// </summary>
            /// <value>
            /// DisplacementMapScaleMask
            /// </value>
            public Vector4 DisplacementMapScaleMask
            {
                set { Set(ref displacementMapScaleMask, value); }
                get { return displacementMapScaleMask; }
            }

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


            private SamplerStateDescription displacementMapSampler = DefaultSamplers.LinearSamplerWrapAni1;
            /// <summary>
            /// Gets or sets the DisplacementMapSampler.
            /// </summary>
            /// <value>
            /// DisplacementMapSampler
            /// </value>
            public SamplerStateDescription DisplacementMapSampler
            {
                set { Set(ref displacementMapSampler, value); }
                get { return displacementMapSampler; }
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

            private bool renderDiffuseAlphaMap = true;
            /// <summary>
            /// 
            /// </summary>
            public bool RenderDiffuseAlphaMap
            {
                set
                {
                    Set(ref renderDiffuseAlphaMap, value);
                }
                get
                {
                    return renderDiffuseAlphaMap;
                }
            }
            private bool renderNormalMap = true;
            /// <summary>
            /// 
            /// </summary>
            public bool RenderNormalMap
            {
                set
                {
                    Set(ref renderNormalMap, value);
                }
                get
                {
                    return renderNormalMap;
                }
            }

            private bool renderSpecularColorMap = true;
            /// <summary>
            /// Gets or sets a value indicating whether [render specular color map].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [render specular color map]; otherwise, <c>false</c>.
            /// </value>
            public bool RenderSpecularColorMap
            {
                set { Set(ref renderSpecularColorMap, value); }
                get { return renderSpecularColorMap; }
            }

            private bool renderDisplacementMap = true;
            /// <summary>
            /// 
            /// </summary>
            public bool RenderDisplacementMap
            {
                set
                {
                    Set(ref renderDisplacementMap, value);
                }
                get { return renderDisplacementMap; }
            }

            private bool renderEmissiveMap = true;
            /// <summary>
            /// Gets or sets a value indicating whether [render emissive map].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [render emissive map]; otherwise, <c>false</c>.
            /// </value>
            public bool RenderEmissiveMap
            {
                set { Set(ref renderEmissiveMap, value); }
                get { return renderEmissiveMap; }
            }

            private bool enableAutoTangent = false;
            /// <summary>
            /// Gets or sets a value indicating whether [enable automatic tangent].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [enable automatic tangent]; otherwise, <c>false</c>.
            /// </value>
            public bool EnableAutoTangent
            {
                set => Set(ref enableAutoTangent, value);
                get => enableAutoTangent;
            }

            private float minTessellationDistance = 10;
            public float MinTessellationDistance
            {
                set
                {
                    Set(ref minTessellationDistance, value);
                }
                get { return minTessellationDistance; }
            }

            private float maxTessellationDistance = 100;
            public float MaxTessellationDistance
            {
                set
                {
                    Set(ref maxTessellationDistance, value);
                }
                get { return maxTessellationDistance; }
            }

            private float minDistanceTessellationFactor = 2;
            /// <summary>
            /// Gets or sets the tessellation factor at <see cref="MinTessellationDistance"/>.
            /// </summary>
            /// <value>
            /// The minimum distance tessellation factor.
            /// </value>
            public float MinDistanceTessellationFactor
            {
                set
                {
                    Set(ref minDistanceTessellationFactor, value);
                }
                get
                {
                    return minDistanceTessellationFactor;
                }
            }

            private float maxDistanceTessellationFactor = 1;
            /// <summary>
            /// Gets or sets the tessellation factor at <see cref="MaxDistanceTessellationFactor"/>
            /// </summary>
            /// <value>
            /// The maximum distance tessellation factor.
            /// </value>
            public float MaxDistanceTessellationFactor
            {
                set
                {
                    Set(ref maxDistanceTessellationFactor, value);
                }
                get
                {
                    return maxDistanceTessellationFactor;
                }
            }

            private MeshTopologyEnum meshType = MeshTopologyEnum.PNTriangles;
            public MeshTopologyEnum MeshType
            {
                set
                {
                    Set(ref meshType, value);
                }
                get
                {
                    return meshType;
                }
            }

            private bool enableTessellation = false;
            public bool EnableTessellation
            {
                set
                {
                    Set(ref enableTessellation, value);
                }
                get
                {
                    return enableTessellation;
                }
            }

            private bool renderShadowMap = false;
            /// <summary>
            /// 
            /// </summary>
            public bool RenderShadowMap
            {
                set
                {
                    Set(ref renderShadowMap, value);
                }
                get { return renderShadowMap; }
            }

            private bool renderEnvironmentMap = false;
            /// <summary>
            /// 
            /// </summary>
            public bool RenderEnvironmentMap
            {
                set
                {
                    Set(ref renderEnvironmentMap, value);
                }
                get { return renderEnvironmentMap; }
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
                return new PhongMaterialVariables(manager, technique, this);
            }
        }
    }

}
