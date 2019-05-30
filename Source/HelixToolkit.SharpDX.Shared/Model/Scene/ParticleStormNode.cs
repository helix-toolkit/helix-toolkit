/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;
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
    namespace Model.Scene
    {
        using Core;   
        using Utilities;
        /// <summary>
        /// 
        /// </summary>
        public class ParticleStormNode : SceneNode, IInstancing, IBoundable
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
            public TextureModel ParticleTexture
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

            private Vector3 domainBoundMax = ParticleRenderCore.DefaultBoundMaximum;
            /// <summary>
            /// Gets or sets the domain bound maximum.
            /// </summary>
            /// <value>
            /// The domain bound maximum.
            /// </value>
            public Vector3 DomainBoundMax
            {
                set {
                    if (Set(ref domainBoundMax, value))
                    {
                        particleCore.DomainBoundMax = value;
                        boundChanged = true;
                    }
                }
                get { return domainBoundMax; }
            }

            private Vector3 domainBoundMin = ParticleRenderCore.DefaultBoundMinimum;
            /// <summary>
            /// Gets or sets the domain bound minimum.
            /// </summary>
            /// <value>
            /// The domain bound minimum.
            /// </value>
            public Vector3 DomainBoundMin
            {
                set
                {
                    if(Set(ref domainBoundMin, value))
                    {
                        particleCore.DomainBoundMin = value;
                        boundChanged = true;
                    }              
                }
                get
                {
                    return domainBoundMin;
                }
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
            /// Gets or sets the blend factor for blending
            /// </summary>
            /// <value>
            /// The blend factor.
            /// </value>
            public Color4 BlendFactor
            {
                set
                {
                    particleCore.BlendFactor = value;
                }
                get { return particleCore.BlendFactor; }
            }
            /// <summary>
            /// Gets or sets the sample mask for blending
            /// </summary>
            /// <value>
            /// The sample mask.
            /// </value>
            public int SampleMask
            {
                set { particleCore.SampleMask = value; }
                get { return particleCore.SampleMask; }
            }

            private IList<Matrix> instances;
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
                    if(Set(ref instances, value))
                    {
                        InstanceBuffer.Elements = value;
                        boundChanged = true;
                    }               
                }
                get
                {
                    return instances;
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

            #region IBoundable
            private BoundingBox originalBound = MaxBound;
            public override BoundingBox OriginalBounds
            {
                get { return originalBound; }
            }

            private BoundingSphere originalBoundsSphere = MaxBoundSphere;
            public override BoundingSphere OriginalBoundsSphere
            {
                get { return originalBoundsSphere; }
            }

            private BoundingBox bounds = MaxBound;
            public override BoundingBox Bounds
            {
                get { return bounds; }
            }

            private BoundingBox boundsWithTransform = MaxBound;
            public override BoundingBox BoundsWithTransform
            {
                get { return boundsWithTransform; }
            }

            private BoundingSphere boundsSphere;
            public override BoundingSphere BoundsSphere
            {
                get { return boundsSphere; }
            }

            private BoundingSphere boundsSphereWithTransform;
            public override BoundingSphere BoundsSphereWithTransform
            {
                get { return boundsSphereWithTransform; }
            }

            protected volatile bool boundChanged = true;
            #endregion

            private bool enableViewFrustumCheck = true;
            /// <summary>
            /// Gets or sets a value indicating whether [enable view frustum check].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [enable view frustum check]; otherwise, <c>false</c>.
            /// </value>
            public bool EnableViewFrustumCheck
            {
                set { enableViewFrustumCheck = value; }
                get { return enableViewFrustumCheck && HasBound; }
            }

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

            public ParticleStormNode()
            {
                HasBound = true;
            }

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
            public override void Update(RenderContext context)
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
                if (boundChanged)
                {
                    UpdateBounds();
                    boundChanged = false;
                }
            }

            private void UpdateBounds(bool transformOnly = false)
            {
                if (!transformOnly)
                {
                    originalBound = new BoundingBox(DomainBoundMin, DomainBoundMax);
                    originalBoundsSphere = BoundingSphere.FromBox(originalBound);
                    BoundingBox newBound;
                    BoundingSphere newBoundSphere;
                    if (HasInstances)
                    {
                        newBound = OriginalBounds.Transform(Instances[0]);
                        newBoundSphere = OriginalBoundsSphere.TransformBoundingSphere(Instances[0]);
                        foreach (var instance in Instances)
                        {
                            var b = OriginalBounds.Transform(instance);
                            BoundingBox.Merge(ref newBound, ref b, out newBound);
                            var bs = OriginalBoundsSphere.TransformBoundingSphere(instance);
                            BoundingSphere.Merge(ref newBoundSphere, ref bs, out newBoundSphere);
                        }
                    }
                    else
                    {
                        newBound = OriginalBounds;
                        newBoundSphere = OriginalBoundsSphere;
                    }

                    var old = bounds;
                    if (Set(ref bounds, newBound))
                    {
                        RaiseOnBoundChanged(new BoundChangeArgs<BoundingBox>(ref bounds, ref old));
                    }
                    var oldS = boundsSphere;
                    if (Set(ref boundsSphere, newBoundSphere))
                    {
                        RaiseOnBoundSphereChanged(new BoundChangeArgs<BoundingSphere>(ref boundsSphere, ref oldS));
                    }
                }

                var oldT = boundsWithTransform;
                if (Set(ref boundsWithTransform, bounds.Transform(ModelMatrix)))
                {
                    RaiseOnTransformBoundChanged(new BoundChangeArgs<BoundingBox>(ref boundsWithTransform, ref oldT));
                }
                var oldTS = boundsSphereWithTransform;
                if (Set(ref boundsSphereWithTransform, boundsSphere.TransformBoundingSphere(ModelMatrix)))
                {
                    RaiseOnTransformBoundSphereChanged(new BoundChangeArgs<BoundingSphere>(ref boundsSphereWithTransform, ref oldTS));
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
            /// Tests the view frustum.
            /// </summary>
            /// <param name="viewFrustum">The view frustum.</param>
            /// <returns></returns>
            public override bool TestViewFrustum(ref BoundingFrustum viewFrustum)
            {
                if (!EnableViewFrustumCheck)
                {
                    return true;
                }
                return BoundingFrustumExtensions.Intersects(ref viewFrustum, ref boundsWithTransform);
            }

            public sealed override bool HitTest(RenderContext context, Ray ray, ref List<HitTestResult> hits)
            {
                return false;
            }

            protected sealed override bool OnHitTest(RenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
            {
                return false;
            }
        }
    }

}