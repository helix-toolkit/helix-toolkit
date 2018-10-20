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

        private Color4 emissiveColor = Color.Black;
        /// <summary>
        /// Gets or sets the color of the emissive.
        /// </summary>
        /// <value>
        /// The color of the emissive.
        /// </value>
        public Color4 EmissiveColor
        {
            set => Set(ref emissiveColor, value);
            get => emissiveColor;
        }

        private float metallicFactor = 0;
        /// <summary>
        /// Gets or sets the metallic factor. If RMA map is used, for each pixel, metallic factor = max(<see cref="MetallicFactor"/>, RMA map B Channel)
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
        /// Gets or sets the roughness factor. If RMA map is used, for each pixel, roughness factor = max(<see cref="RoughnessFactor"/>, RMA map G Channel)
        /// </summary>
        /// <value>
        /// The roughness factor.
        /// </value>
        public float RoughnessFactor
        {
            set => Set(ref roughnessFactor, value);
            get => roughnessFactor; 
        }

        private float ambientOcclusionFactor = 1;
        /// <summary>
        /// Gets or sets the ambient occlusion factor. If RMA map is used, for each pixel, ambient occlusion factor = min(<see cref="AmbientOcclusionFactor"/>, RMA map R Channel)
        /// </summary>
        /// <value>
        /// The ambient occlusion factor.
        /// </value>
        public float AmbientOcclusionFactor
        {
            set => Set(ref ambientOcclusionFactor, value);
            get => ambientOcclusionFactor;
        }

        private float reflectanceFactor = 0;
        /// <summary>
        /// Gets or sets the reflectance factor.
        /// </summary>
        /// <value>
        /// The reflectance factor.
        /// </value>
        public float ReflectanceFactor
        {
            set => Set(ref reflectanceFactor, value);
            get => reflectanceFactor;
        }

        private float clearCoatStrength = 0;
        /// <summary>
        /// Gets or sets the clear coat strength.
        /// </summary>
        /// <value>
        /// The clear coat strength.
        /// </value>
        public float ClearCoatStrength
        {
            set => Set(ref clearCoatStrength, value);
            get => clearCoatStrength;
        }

        private float clearCoatRoughness = 0;
        /// <summary>
        /// Gets or sets the clear coat roughness.
        /// </summary>
        /// <value>
        /// The clear coat roughness.
        /// </value>
        public float ClearCoatRoughness
        {
            set => Set(ref clearCoatRoughness, value);
            get => clearCoatRoughness;
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
            set => Set(ref renderDisplacementMap, value);
            get => renderDisplacementMap;
        }

        private bool renderShadowMap = false;
        /// <summary>
        /// 
        /// </summary>
        public bool RenderShadowMap
        {
            set => Set(ref renderShadowMap, value);
            get => renderShadowMap; 
        }

        private bool renderEnvironmentMap = false;
        /// <summary>
        /// 
        /// </summary>
        public bool RenderEnvironmentMap
        {
            set => Set(ref renderEnvironmentMap, value);
            get => renderEnvironmentMap;
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
            set => Set(ref renderIrradianceMap, value);
            get => renderIrradianceMap;
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
            set => Set(ref renderEmissiveMap, value); 
            get => renderEmissiveMap; 
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

        private Stream albedoMap;
        /// <summary>
        /// Gets or sets the albedo map.
        /// </summary>
        /// <value>
        /// The albedo map.
        /// </value>
        public Stream AlbedoMap
        {
            set => Set(ref albedoMap, value);
            get => albedoMap;
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
            set => Set(ref emissiveMap, value);
            get => emissiveMap; 
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
            set => Set(ref normalMap, value); 
            get => normalMap; 
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
            set => Set(ref displacementMap, value); 
            get => displacementMap; 
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
            set => Set(ref irradianceMap, value);
            get => irradianceMap; 
        }

        private Stream rmaMap;
        /// <summary>
        /// Gets or sets the Roughness, Metallic, Ambient Occlusion map. glTF2 defines occlusion as R channel, roughness as G channel, metalness as B channel
        /// </summary>
        /// <value>
        /// The rma map.
        /// </value>
        public Stream RMAMap
        {
            set => Set(ref rmaMap, value); 
            get => rmaMap; 
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
            set => Set(ref displacementMapScaleMask, value); 
            get => displacementMapScaleMask; 
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
            set => Set(ref uvTransform, value); 
            get => uvTransform; 
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
            set => Set(ref surfaceMapSampler, value); 
            get => surfaceMapSampler; 
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
            set => Set(ref displacementMapSampler, value); 
            get => displacementMapSampler; 
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
            set => Set(ref iblSampler, value); 
            get => iblSampler; 
        }

        private float minTessellationDistance = 10;
        public float MinTessellationDistance
        {
            set => Set(ref minTessellationDistance, value);
            get => minTessellationDistance; 
        }

        private float maxTessellationDistance = 100;
        public float MaxTessellationDistance
        {
            set =>  Set(ref maxTessellationDistance, value);
            get => maxTessellationDistance; 
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
            set =>  Set(ref minDistanceTessellationFactor, value);
            get => minDistanceTessellationFactor;
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
            set => Set(ref maxDistanceTessellationFactor, value);
            get => maxDistanceTessellationFactor;
        }

        private MeshTopologyEnum meshType = MeshTopologyEnum.PNTriangles;
        public MeshTopologyEnum MeshType
        {
            set => Set(ref meshType, value);
            get => meshType;
        }

        private bool enableTessellation = false;
        public bool EnableTessellation
        {
            set => Set(ref enableTessellation, value);
            get => enableTessellation;
        }



        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new PBRMaterialVariable(manager, technique, this);
        }
    }
}
