/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
using System.IO;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    using Utilities;
    using Shaders;
    public partial class PBRMaterialCore : MaterialCore
    {
        private Color4 albedoColor = Color.White;

        /// <summary>
        /// Gets or sets the color of the albedo.
        /// </summary>
        /// <value>
        /// The color of the albedo.
        /// </value>
        public Color4 AlbedoColor
        {
            set => Set(ref albedoColor, value); 
            get => albedoColor; 
        }

        private float metallicFactor = 0;
        /// <summary>
        /// Gets or sets the metallic factor.
        /// </summary>
        /// <value>
        /// The metallic factor.
        /// </value>
        public float MetallicFactor
        {
            set => Set(ref metallicFactor, value); 
            get => metallicFactor; 
        }

        private float roughnessFactor = 0;
        /// <summary>
        /// Gets or sets the roughness factor.
        /// </summary>
        /// <value>
        /// The roughness factor.
        /// </value>
        public float RoughnessFactor
        {
            set => Set(ref roughnessFactor, value);
            get => roughnessFactor; 
        }

        private bool renderAlbodoMap = true;
        public bool RenderAlbedoMap
        {
            set => Set(ref renderAlbodoMap, value);
            get => renderAlbodoMap;
        }

        private bool renderNormalMap = true;
        public bool RenderNormalMap
        {
            set => Set(ref renderNormalMap, value);
            get => renderNormalMap;
        }

        private bool renderRMAMap = true;
        public bool RenderRMAMap
        {
            set => Set(ref renderRMAMap, value);
            get => renderRMAMap;
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

        private bool renderIrradianceMap = true;
        /// <summary>
        /// Gets or sets a value indicating whether [render irrandiance map].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render irrandiance map]; otherwise, <c>false</c>.
        /// </value>
        public bool RenderIrradianceMap
        {
            set { Set(ref renderIrradianceMap, value); }
            get { return renderIrradianceMap; }
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

        private Stream albedoMap;
        /// <summary>
        /// Gets or sets the albedo map.
        /// </summary>
        /// <value>
        /// The albedo map.
        /// </value>
        public Stream AlbedoMap
        {
            set { Set(ref albedoMap, value); }
            get { return albedoMap; }
        }

        private Stream emissiveMap;
        /// <summary>
        /// Gets or sets the emissive map.
        /// </summary>
        /// <value>
        /// The emissive map.
        /// </value>
        public Stream EmissiveMap
        {
            set { Set(ref emissiveMap, value); }
            get { return emissiveMap; }
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

        private Stream irradianceMap;
        /// <summary>
        /// Gets or sets the irradiance map.
        /// </summary>
        /// <value>
        /// The irradiance map.
        /// </value>
        public Stream IrradianceMap
        {
            set { Set(ref irradianceMap, value); }
            get { return irradianceMap; }
        }

        private Stream rmaMap;
        /// <summary>
        /// Gets or sets the roughness, metallic, ambient map.
        /// </summary>
        /// <value>
        /// The rma map.
        /// </value>
        public Stream RMAMap
        {
            set { Set(ref rmaMap, value); }
            get { return rmaMap; }
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

        private SamplerStateDescription surfaceMapSampler = DefaultSamplers.LinearSamplerWrapAni4;
        /// <summary>
        /// Gets or sets the surface map sampler.
        /// </summary>
        /// <value>
        /// The surface map sampler.
        /// </value>
        public SamplerStateDescription SurfaceMapSampler
        {
            set { Set(ref surfaceMapSampler, value); }
            get { return surfaceMapSampler; }
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

        private SamplerStateDescription iblSampler = DefaultSamplers.IBLSampler;
        /// <summary>
        /// Gets or sets the IBL sampler.
        /// </summary>
        /// <value>
        /// The IBL sampler.
        /// </value>
        public SamplerStateDescription IBLSampler
        {
            set { Set(ref iblSampler, value); }
            get { return iblSampler; }
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



        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new PBRMaterialVariable(manager, technique, this);
        }
    }
}
