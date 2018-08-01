/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using SharpDX.Direct3D11;
using System.IO;
using System.Numerics;
using Matrix = System.Numerics.Matrix4x4;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
using HelixToolkit.UWP.Utilities;
namespace HelixToolkit.UWP.Model
#endif
{
    using Shaders;

    /// <summary>
    /// 
    /// </summary>
    public partial class PhongMaterialCore : MaterialCore, IPhongMaterial
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

        private Stream diffuseMap;
        /// <summary>
        /// Gets or sets the diffuse map.
        /// </summary>
        /// <value>
        /// The diffuse map.
        /// </value>
        public Stream DiffuseMap
        {
            set { Set(ref diffuseMap, value); }
            get { return diffuseMap; }
        }

        private Stream diffuseAlphaMap;
        /// <summary>
        /// Gets or sets the DiffuseAlphaMap.
        /// </summary>
        /// <value>
        /// DiffuseAlphaMap
        /// </value>
        public Stream DiffuseAlphaMap
        {
            set { Set(ref diffuseAlphaMap, value); }
            get { return diffuseAlphaMap; }
        }


        private Stream normalMap;
        /// <summary>
        /// Gets or sets the NormalMap.
        /// </summary>
        /// <value>
        /// NormalMap
        /// </value>
        public Stream NormalMap
        {
            set { Set(ref normalMap, value); }
            get { return normalMap; }
        }


        private Stream displacementMap;
        /// <summary>
        /// Gets or sets the DisplacementMap.
        /// </summary>
        /// <value>
        /// DisplacementMap
        /// </value>
        public Stream DisplacementMap
        {
            set { Set(ref displacementMap, value); }
            get { return displacementMap; }
        }


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

        private Matrix uvTransform = Matrix.Identity;
        /// <summary>
        /// Gets or sets the uv transform.
        /// </summary>
        /// <value>
        /// The uv transform.
        /// </value>
        public Matrix UVTransform
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


        private SamplerStateDescription normalMapSampler = DefaultSamplers.LinearSamplerWrapAni4;
        /// <summary>
        /// Gets or sets the NormalMapSampler.
        /// </summary>
        /// <value>
        /// NormalMapSampler
        /// </value>
        public SamplerStateDescription NormalMapSampler
        {
            set { Set(ref normalMapSampler, value); }
            get { return normalMapSampler; }
        }


        private SamplerStateDescription diffuseAlphaMapSampler = DefaultSamplers.LinearSamplerWrapAni4;
        /// <summary>
        /// Gets or sets the DiffuseAlphaMapSampler.
        /// </summary>
        /// <value>
        /// DiffuseAlphaMapSampler
        /// </value>
        public SamplerStateDescription DiffuseAlphaMapSampler
        {
            set { Set(ref diffuseAlphaMapSampler, value); }
            get { return diffuseAlphaMapSampler; }
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

        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new TextureSharedPhongMaterialVariables(manager, technique, this);
        }
    }
}
