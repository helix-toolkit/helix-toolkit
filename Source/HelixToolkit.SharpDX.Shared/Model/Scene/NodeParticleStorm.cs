/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;
    using System.IO;
    using Utilities;
    /// <summary>
    /// 
    /// </summary>
    public class NodeParticleStorm : SceneNode, IInstancing
    {
        #region Properties
        /// <summary>
        /// Gets or sets the particle count.
        /// </summary>
        /// <value>
        /// The particle count.
        /// </value>
        public int ParticleCount
        {
            set
            {
                particleCore.ParticleCount = value;
            }
            get
            {
                return particleCore.ParticleCount;
            }
        }
        /// <summary>
        /// Gets or sets the emitter location.
        /// </summary>
        /// <value>
        /// The emitter location.
        /// </value>
        public Vector3 EmitterLocation
        {
            set
            {
                particleCore.EmitterLocation = value;
            }
            get
            {
                return particleCore.EmitterLocation;
            }
        }
        /// <summary>
        /// Gets or sets the emitter radius.
        /// </summary>
        /// <value>
        /// The emitter radius.
        /// </value>
        public float EmitterRadius
        {
            set
            {
                particleCore.EmitterRadius = value;
            }
            get
            {
                return particleCore.EmitterRadius;
            }
        }
        /// <summary>
        /// Gets or sets the consumer location.
        /// </summary>
        /// <value>
        /// The consumer location.
        /// </value>
        public Vector3 ConsumerLocation
        {
            set
            {
                particleCore.ConsumerLocation = value;
            }
            get
            {
                return particleCore.ConsumerLocation;
            }
        }
        /// <summary>
        /// Gets or sets the consumer radius.
        /// </summary>
        /// <value>
        /// The consumer radius.
        /// </value>
        public float ConsumerRadius
        {
            set { particleCore.ConsumerRadius = value; }
            get { return particleCore.ConsumerRadius; }
        }
        /// <summary>
        /// Gets or sets the consumer gravity.
        /// </summary>
        /// <value>
        /// The consumer gravity.
        /// </value>
        public float ConsumerGravity
        {
            set { particleCore.ConsumerGravity = value; }
            get { return particleCore.ConsumerGravity; }
        }
        /// <summary>
        /// Gets or sets the initial energy.
        /// </summary>
        /// <value>
        /// The initial energy.
        /// </value>
        public float InitialEnergy
        {
            set
            {
                particleCore.InitialEnergy = value;
                particleCore.UpdateInsertThrottle();
            }
            get { return particleCore.InitialEnergy; }
        }
        /// <summary>
        /// Gets or sets the energy dissipation rate.
        /// </summary>
        /// <value>
        /// The energy dissipation rate.
        /// </value>
        public float EnergyDissipationRate
        {
            set { particleCore.EnergyDissipationRate = value; }
            get { return particleCore.EnergyDissipationRate; }
        }
        /// <summary>
        /// Gets or sets the random vector generator.
        /// </summary>
        /// <value>
        /// The random vector generator.
        /// </value>
        public IRandomVector RandomVectorGenerator
        {
            set
            {
                particleCore.VectorGenerator = value;
            }
            get
            {
                return particleCore.VectorGenerator;
            }
        }
        /// <summary>
        /// Gets or sets the particle texture.
        /// </summary>
        /// <value>
        /// The particle texture.
        /// </value>
        public Stream ParticleTexture
        {
            set { particleCore.ParticleTexture = value; }
            get { return particleCore.ParticleTexture; }
        }
        /// <summary>
        /// Gets or sets the number texture column.
        /// </summary>
        /// <value>
        /// The number texture column.
        /// </value>
        public uint NumTextureColumn
        {
            set { particleCore.NumTextureColumn = value; }
            get { return particleCore.NumTextureColumn; }
        }
        /// <summary>
        /// Gets or sets the number texture row.
        /// </summary>
        /// <value>
        /// The number texture row.
        /// </value>
        public uint NumTextureRow
        {
            set { particleCore.NumTextureRow = value; }
            get { return particleCore.NumTextureRow; }
        }
        /// <summary>
        /// Gets or sets the size of the particle.
        /// </summary>
        /// <value>
        /// The size of the particle.
        /// </value>
        public Vector2 ParticleSize
        {
            set { particleCore.ParticleSize = value; }
            get { return particleCore.ParticleSize; }
        }
        /// <summary>
        /// Gets or sets the initial velocity.
        /// </summary>
        /// <value>
        /// The initial velocity.
        /// </value>
        public float InitialVelocity
        {
            set { particleCore.InitialVelocity = value; }
            get { return particleCore.InitialVelocity; }
        }
        /// <summary>
        /// Gets or sets the initialize acceleration.
        /// </summary>
        /// <value>
        /// The initialize acceleration.
        /// </value>
        public Vector3 InitAcceleration
        {
            set { particleCore.InitialAcceleration = value; }
            get { return particleCore.InitialAcceleration; }
        }
        /// <summary>
        /// Gets or sets the domain bound maximum.
        /// </summary>
        /// <value>
        /// The domain bound maximum.
        /// </value>
        public Vector3 DomainBoundMax
        {
            set { particleCore.DomainBoundMax = value; }
            get { return particleCore.DomainBoundMax; }
        }
        /// <summary>
        /// Gets or sets the domain bound minimum.
        /// </summary>
        /// <value>
        /// The domain bound minimum.
        /// </value>
        public Vector3 DomainBoundMin
        {
            set { particleCore.DomainBoundMin = value; }
            get { return particleCore.DomainBoundMin; }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [cumulate at bound].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [cumulate at bound]; otherwise, <c>false</c>.
        /// </value>
        public bool CumulateAtBound
        {
            set { particleCore.CumulateAtBound = value; }
            get { return particleCore.CumulateAtBound; }
        }
        /// <summary>
        /// Gets or sets the color of the blend.
        /// </summary>
        /// <value>
        /// The color of the blend.
        /// </value>
        public Color4 BlendColor
        {
            set { particleCore.ParticleBlendColor = value; }
            get { return particleCore.ParticleBlendColor; }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [animate sprite by energy].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [animate sprite by energy]; otherwise, <c>false</c>.
        /// </value>
        public bool AnimateSpriteByEnergy
        {
            set { particleCore.AnimateSpriteByEnergy = value; }
            get { return particleCore.AnimateSpriteByEnergy; }
        }
        /// <summary>
        /// Gets or sets the turbulance.
        /// </summary>
        /// <value>
        /// The turbulance.
        /// </value>
        public float Turbulance
        {
            set { particleCore.Turbulance = value; }
            get { return particleCore.Turbulance; }
        }

        private BlendOperation blend = BlendOperation.Add;
        /// <summary>
        /// Gets or sets the blend.
        /// </summary>
        /// <value>
        /// The blend.
        /// </value>
        public BlendOperation Blend
        {
            set
            {
                if (Set(ref blend, value))
                {
                    OnBlendStateChanged();
                }
            }
            get { return blend; }
        }

        private BlendOperation alphaBlend = BlendOperation.Add;
        /// <summary>
        /// Gets or sets the alpha blend.
        /// </summary>
        /// <value>
        /// The alpha blend.
        /// </value>
        public BlendOperation AlphaBlend
        {
            set
            {
                if (Set(ref alphaBlend, value))
                {
                    OnBlendStateChanged();
                }
            }
            get { return alphaBlend; }
        }

        private BlendOption sourceBlend = BlendOption.One;
        /// <summary>
        /// Gets or sets the source blend.
        /// </summary>
        /// <value>
        /// The source blend.
        /// </value>
        public BlendOption SourceBlend
        {
            set
            {
                if (Set(ref sourceBlend, value))
                {
                    OnBlendStateChanged();
                }
            }
            get { return sourceBlend; }
        }

        private BlendOption destBlend = BlendOption.One;
        /// <summary>
        /// Gets or sets the dest blend.
        /// </summary>
        /// <value>
        /// The dest blend.
        /// </value>
        public BlendOption DestBlend
        {
            set
            {
                if (Set(ref destBlend, value))
                {
                    OnBlendStateChanged();
                }
            }
            get { return destBlend; }
        }

        private BlendOption sourceAlphaBlend = BlendOption.One;
        /// <summary>
        /// Gets or sets the source alpha blend.
        /// </summary>
        /// <value>
        /// The source alpha blend.
        /// </value>
        public BlendOption SourceAlphaBlend
        {
            set
            {
                if (Set(ref sourceAlphaBlend, value))
                {
                    OnBlendStateChanged();
                }
            }
            get { return sourceAlphaBlend; }
        }

        private BlendOption destAlphaBlend = BlendOption.Zero;
        /// <summary>
        /// Gets or sets the dest alpha blend.
        /// </summary>
        /// <value>
        /// The dest alpha blend.
        /// </value>
        public BlendOption DestAlphaBlend
        {
            set
            {
                if (Set(ref destAlphaBlend, value))
                {
                    OnBlendStateChanged();
                }
            }
            get { return destAlphaBlend; }
        }
        /// <summary>
        /// Gets or sets the instances.
        /// </summary>
        /// <value>
        /// The instances.
        /// </value>
        public IList<Matrix> Instances
        {
            set
            {
                InstanceBuffer.Elements = value;
            }
            get
            {
                return InstanceBuffer.Elements;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance has instances.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has instances; otherwise, <c>false</c>.
        /// </value>
        public bool HasInstances { get { return InstanceBuffer.HasElements; } } 
        #endregion

        /// <summary>
        /// Gets the instance buffer.
        /// </summary>
        /// <value>
        /// The instance buffer.
        /// </value>
        public IElementsBufferModel<Matrix> InstanceBuffer { get; } = new MatrixInstanceBufferModel();
        
        private ParticleRenderCore particleCore
        {
            get
            {
                return RenderCore as ParticleRenderCore;
            }
        }

        private volatile bool blendChanged = true;

        private void OnBlendStateChanged()
        {
            blendChanged = true;
        }
        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override RenderCore OnCreateRenderCore()
        {
            return new ParticleRenderCore();
        }
        /// <summary>
        /// Called when [create render technique].
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.ParticleStorm];
        }
        /// <summary>
        /// Called when [attach].
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        protected override bool OnAttach(IRenderHost host)
        {
            base.OnAttach(host);
            InstanceBuffer.Initialize();
            InstanceBuffer.Elements = Instances;
            particleCore.InstanceBuffer = InstanceBuffer;
            return true;
        }
        /// <summary>
        /// Updates the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Update(IRenderContext context)
        {
            base.Update(context);
            if (blendChanged)
            {
                var desc = new BlendStateDescription();
                desc.RenderTarget[0] = new RenderTargetBlendDescription
                {
                    IsBlendEnabled = true,
                    BlendOperation = this.Blend,
                    AlphaBlendOperation = this.AlphaBlend,
                    SourceBlend = this.SourceBlend,
                    DestinationBlend = this.DestBlend,
                    SourceAlphaBlend = this.SourceAlphaBlend,
                    DestinationAlphaBlend = this.DestAlphaBlend,
                    RenderTargetWriteMask = ColorWriteMaskFlags.All
                };
                particleCore.BlendDescription = desc;
                blendChanged = false;
            }
        }
        /// <summary>
        /// Called when [detach].
        /// </summary>
        protected override void OnDetach()
        {
            InstanceBuffer.Dispose();
            base.OnDetach();
        }
        /// <summary>
        /// Determines whether this instance [can hit test] the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can hit test] the specified context; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CanHitTest(IRenderContext context)
        {
            return false;
        }
        /// <summary>
        /// Called when [hit test].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="totalModelMatrix">The total model matrix.</param>
        /// <param name="ray">The ray.</param>
        /// <param name="hits">The hits.</param>
        /// <returns></returns>
        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }
    }
}