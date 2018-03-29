/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;
using System.Linq;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;
    using System.IO;
    using Utilities;

    public class ParticleStormNode : SceneNode, IInstancing
    {
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

        public float ConsumerRadius
        {
            set { particleCore.ConsumerRadius = value; }
            get { return particleCore.ConsumerRadius; }
        }

        public float ConsumerGravity
        {
            set { particleCore.ConsumerGravity = value; }
            get { return particleCore.ConsumerGravity; }
        }

        public float InitialEnergy
        {
            set
            {
                particleCore.InitialEnergy = value;
                particleCore.UpdateInsertThrottle();
            }
            get { return particleCore.InitialEnergy; }
        }

        public float EnergyDissipationRate
        {
            set
            {
                particleCore.EnergyDissipationRate = value;
            }
            get { return particleCore.EnergyDissipationRate; }
        }

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

        public Stream ParticleTexture
        {
            set
            {
                particleCore.ParticleTexture = value;
            }
            get { return particleCore.ParticleTexture; }
        }

        public uint NumTextureColumn
        {
            set { particleCore.NumTextureColumn = value; }
            get { return particleCore.NumTextureColumn; }
        }

        public uint NumTextureRow
        {
            set { particleCore.NumTextureRow = value; }
            get { return particleCore.NumTextureRow; }
        }

        public Vector2 ParticleSize
        {
            set { particleCore.ParticleSize = value; }
            get { return particleCore.ParticleSize; }
        }

        public float InitialVelocity
        {
            set { particleCore.InitialVelocity = value; }
            get { return particleCore.InitialVelocity; }
        }

        public Vector3 InitAcceleration
        {
            set { particleCore.InitialAcceleration = value; }
            get { return particleCore.InitialAcceleration; }
        }

        public Vector3 DomainBoundMax
        {
            set { particleCore.DomainBoundMax = value; }
            get { return particleCore.DomainBoundMax; }
        }

        public Vector3 DomainBoundMin
        {
            set { particleCore.DomainBoundMin = value; }
            get { return particleCore.DomainBoundMin; }
        }

        public bool CumulateAtBound
        {
            set { particleCore.CumulateAtBound = value; }
            get { return particleCore.CumulateAtBound; }
        }

        public Color4 BlendColor
        {
            set { particleCore.ParticleBlendColor = value; }
            get { return particleCore.ParticleBlendColor; }
        }

        public bool AnimateSpriteByEnergy
        {
            set { particleCore.AnimateSpriteByEnergy = value; }
            get { return particleCore.AnimateSpriteByEnergy; }
        }

        public float Turbulance
        {
            set { particleCore.Turbulance = value; }
            get { return particleCore.Turbulance; }
        }

        private BlendOperation blend = BlendOperation.Add;
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

        public bool HasInstances { get { return InstanceBuffer.HasElements; } }
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

        protected override RenderCore OnCreateRenderCore()
        {
            return new ParticleRenderCore();
        }

        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.ParticleStorm];
        }

        protected override bool OnAttach(IRenderHost host)
        {
            base.OnAttach(host);
            InstanceBuffer.Initialize();
            InstanceBuffer.Elements = Instances;
            particleCore.InstanceBuffer = InstanceBuffer;
            return true;
        }

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

        protected override void OnDetach()
        {
            InstanceBuffer.Dispose();
            base.OnDetach();
        }

        protected override bool CanHitTest(IRenderContext context)
        {
            return false;
        }

        protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }
    }   
}
