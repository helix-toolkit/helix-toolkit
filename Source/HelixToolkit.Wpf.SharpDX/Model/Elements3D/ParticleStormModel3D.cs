// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParticleStormModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2017 Helix Toolkit contributors
// </copyright>
// <author>Lunci Hua</author>
// <summary>
//  Particle system.
//  References: https://github.com/spazzarama/Direct3D-Rendering-Cookbook
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using SharpDX;
using System.Windows;
using SharpDX.Direct3D11;
using System.IO;
using Media3D = System.Windows.Media.Media3D;
using Media = System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX
{
    using Core;
    using System;
    using Utility;
    using static Core.ParticleRenderCore;

    public class ParticleStormModel3D : Element3D
    {
        #region Dependency Properties
        public static DependencyProperty ParticleCountProperty = DependencyProperty.Register("ParticleCount", typeof(int), typeof(ParticleStormModel3D),
            new PropertyMetadata(DefaultParticleCount,
            (d, e) =>
            {
                (d as ParticleStormModel3D).particleCore.ParticleCount = Math.Max(8, (int)e.NewValue);
            }
            ));

        public int ParticleCount
        {
            set
            {
                SetValue(ParticleCountProperty, value);
            }
            get
            {
                return (int)GetValue(ParticleCountProperty);
            }
        }

        public static DependencyProperty EmitterLocationProperty = DependencyProperty.Register("EmitterLocation", typeof(Media3D.Point3D), typeof(ParticleStormModel3D),
            new PropertyMetadata(DefaultEmitterLocation.ToPoint3D(),
            (d, e) =>
            {
                (d as ParticleStormModel3D).particleCore.InsertVariables.EmitterLocation = (((Media3D.Point3D)e.NewValue).ToVector3());
            }
            ));

        public Media3D.Point3D EmitterLocation
        {
            set
            {
                SetValue(EmitterLocationProperty, value);
            }
            get
            {
                return (Media3D.Point3D)GetValue(EmitterLocationProperty);
            }
        }

        public static DependencyProperty EmitterRadiusProperty = DependencyProperty.Register("EmitterRadius", typeof(double), typeof(ParticleStormModel3D),
            new PropertyMetadata(0.0,
            (d, e) =>
            {
                (d as ParticleStormModel3D).particleCore.InsertVariables.EmitterRadius = (float)(double)e.NewValue;
            }
            ));

        public double EmitterRadius
        {
            set
            {
                SetValue(EmitterRadiusProperty, value);
            }
            get
            {
                return (double)GetValue(EmitterRadiusProperty);
            }
        }

        public static DependencyProperty ConsumerLocationProperty = DependencyProperty.Register("ConsumerLocation", typeof(Media3D.Point3D), typeof(ParticleStormModel3D),
            new PropertyMetadata(DefaultConsumerLocation.ToPoint3D(),
            (d, e) =>
            {
                (d as ParticleStormModel3D).particleCore.FrameVariables.ConsumerLocation = (((Media3D.Point3D)e.NewValue).ToVector3());
            }
            ));

        public Media3D.Point3D ConsumerLocation
        {
            set
            {
                SetValue(ConsumerLocationProperty, value);
            }
            get
            {
                return (Media3D.Point3D)GetValue(ConsumerLocationProperty);
            }
        }

        public static DependencyProperty ConsumerGravityProperty = DependencyProperty.Register("ConsumerGravity", typeof(double), typeof(ParticleStormModel3D),
            new PropertyMetadata(0.0,
            (d, e) =>
            {
                (d as ParticleStormModel3D).particleCore.FrameVariables.ConsumerGravity = ((float)(double)e.NewValue);
            }
            ));

        public double ConsumerGravity
        {
            set
            {
                SetValue(ConsumerGravityProperty, value);
            }
            get
            {
                return (double)GetValue(ConsumerGravityProperty);
            }
        }

        public static DependencyProperty ConsumerRadiusProperty = DependencyProperty.Register("ConsumerRadius", typeof(double), typeof(ParticleStormModel3D),
            new PropertyMetadata(0.0,
            (d, e) =>
            {
                (d as ParticleStormModel3D).particleCore.FrameVariables.ConsumerRadius = (float)(double)e.NewValue;
            }
            ));

        public double ConsumerRadius
        {
            set
            {
                SetValue(ConsumerRadiusProperty, value);
            }
            get
            {
                return (double)GetValue(ConsumerRadiusProperty);
            }
        }

        public static DependencyProperty InitialEnergyProperty = DependencyProperty.Register("InitialEnergy", typeof(double), typeof(ParticleStormModel3D),
            new PropertyMetadata(5.0,
            (d, e) =>
            {
                (d as ParticleStormModel3D).particleCore.InsertVariables.InitialEnergy = System.Math.Max(1f, (float)(double)e.NewValue);
                (d as ParticleStormModel3D).particleCore.UpdateInsertThrottle();
            }
            ));

        public double InitialEnergy
        {
            set
            {
                SetValue(InitialEnergyProperty, value);
            }
            get
            {
                return (double)GetValue(InitialEnergyProperty);
            }
        }

        public static DependencyProperty EnergyDissipationRateProperty = DependencyProperty.Register("EnergyDissipationRate", typeof(double), typeof(ParticleStormModel3D),
            new PropertyMetadata(1.0,
            (d, e) =>
            {
                (d as ParticleStormModel3D).particleCore.InsertVariables.EnergyDissipationRate = System.Math.Max(1f, (float)(double)e.NewValue);
            }
            ));

        public double EnergyDissipationRate
        {
            set
            {
                SetValue(EnergyDissipationRateProperty, value);
            }
            get
            {
                return (double)GetValue(EnergyDissipationRateProperty);
            }
        }

        public static DependencyProperty RandomVectorGeneratorProperty = DependencyProperty.Register("RandomVectorGenerator", typeof(IRandomVector), typeof(ParticleStormModel3D),
            new PropertyMetadata(new UniformRandomVectorGenerator(),
            (d, e) =>
            {
                (d as ParticleStormModel3D).particleCore.VectorGenerator = (IRandomVector)e.NewValue;
            }
            ));

        public IRandomVector RandomVectorGenerator
        {
            set
            {
                SetValue(RandomVectorGeneratorProperty, value);
            }
            get
            {
                return (IRandomVector)GetValue(RandomVectorGeneratorProperty);
            }
        }

        public static DependencyProperty ParticleTextureProperty = DependencyProperty.Register("ParticleTexture", typeof(Stream), typeof(ParticleStormModel3D),
            new AffectsRenderPropertyMetadata(null,
            (d, e) =>
            {
                (d as ParticleStormModel3D).particleCore.ParticleTexture = (Stream)e.NewValue;
            }
            ));

        public Stream ParticleTexture
        {
            set
            {
                SetValue(ParticleTextureProperty, value);
            }
            get
            {
                return (Stream)GetValue(ParticleTextureProperty);
            }
        }

        public static DependencyProperty NumTextureColumnProperty = DependencyProperty.Register("NumTextureColumn", typeof(int), typeof(ParticleStormModel3D),
            new PropertyMetadata(1,
            (d, e) =>
            {
                (d as ParticleStormModel3D).particleCore.NumTextureColumn = (uint)System.Math.Max(1, (int)e.NewValue);
            }
            ));

        public int NumTextureColumn
        {
            set
            {
                SetValue(NumTextureColumnProperty, value);
            }
            get
            {
                return (int)GetValue(NumTextureColumnProperty);
            }
        }

        public static DependencyProperty NumTextureRowProperty = DependencyProperty.Register("NumTextureRow", typeof(int), typeof(ParticleStormModel3D),
            new PropertyMetadata(1,
            (d, e) =>
            {
                (d as ParticleStormModel3D).particleCore.NumTextureRow = (uint)System.Math.Max(1, (int)e.NewValue);
            }
            ));

        public int NumTextureRow
        {
            set
            {
                SetValue(NumTextureRowProperty, value);
            }
            get
            {
                return (int)GetValue(NumTextureRowProperty);
            }
        }

        public static DependencyProperty ParticleSizeProperty = DependencyProperty.Register("ParticleSize", typeof(Size), typeof(ParticleStormModel3D),
            new AffectsRenderPropertyMetadata(new Size(ParticleRenderCore.DefaultParticleSize.X, ParticleRenderCore.DefaultParticleSize.Y),
                (d, e) =>
                {
                    var size = (Size)e.NewValue;
                    (d as ParticleStormModel3D).particleCore.ParticleSize = new Vector2((float)size.Width, (float)size.Height);
                }));

        public Size ParticleSize
        {
            set
            {
                SetValue(ParticleSizeProperty, value);
            }
            get
            {
                return (Size)GetValue(ParticleSizeProperty);
            }
        }


        public static DependencyProperty InitialVelocityProperty = DependencyProperty.Register("InitialVelocity", typeof(double), typeof(ParticleStormModel3D),
            new PropertyMetadata(1.0,
            (d, e) =>
            {
                (d as ParticleStormModel3D).particleCore.InsertVariables.InitialVelocity = (float)(double)e.NewValue;
            }
            ));

        public double InitialVelocity
        {
            set
            {
                SetValue(InitialVelocityProperty, value);
            }
            get
            {
                return (double)GetValue(InitialVelocityProperty);
            }
        }

        public static DependencyProperty AccelerationProperty = DependencyProperty.Register("Acceleration", typeof(Media3D.Vector3D), typeof(ParticleStormModel3D),
            new PropertyMetadata(DefaultAcceleration.ToVector3D(),
            (d, e) =>
            {
                (d as ParticleStormModel3D).particleCore.InsertVariables.InitialAcceleration = ((Media3D.Vector3D)e.NewValue).ToVector3();
            }
            ));

        public Media3D.Vector3D Acceleration
        {
            set
            {
                SetValue(AccelerationProperty, value);
            }
            get
            {
                return (Media3D.Vector3D)GetValue(AccelerationProperty);
            }
        }

        public static DependencyProperty ParticleBoundsProperty = DependencyProperty.Register("ParticleBounds", typeof(Media3D.Rect3D), typeof(ParticleStormModel3D),
            new PropertyMetadata(new Media3D.Rect3D(0, 0, 0, 100, 100, 100),
            (d, e) =>
            {
                var bound = (Media3D.Rect3D)e.NewValue;
                (d as ParticleStormModel3D).particleCore.FrameVariables.DomainBoundsMax = new Vector3((float)(bound.SizeX / 2 + bound.Location.X), (float)(bound.SizeY / 2 + bound.Location.Y), (float)(bound.SizeZ / 2 + bound.Location.Z));
                (d as ParticleStormModel3D).particleCore.FrameVariables.DomainBoundsMin = new Vector3((float)(bound.Location.X - bound.SizeX / 2), (float)(bound.Location.Y - bound.SizeY / 2), (float)(bound.Location.Z - bound.SizeZ / 2));
            }));

        public Media3D.Rect3D ParticleBounds
        {
            set
            {
                SetValue(ParticleBoundsProperty, value);
            }
            get
            {
                return (Media3D.Rect3D)GetValue(ParticleBoundsProperty);
            }
        }

        public static DependencyProperty CumulateAtBoundProperty = DependencyProperty.Register("CumulateAtBound", typeof(bool), typeof(ParticleStormModel3D),
            new PropertyMetadata(false,
                (d, e) =>
                {
                    (d as ParticleStormModel3D).particleCore.FrameVariables.CumulateAtBound = (bool)e.NewValue ? 1u : 0;
                }));

        public bool CumulateAtBound
        {
            set
            {
                SetValue(CumulateAtBoundProperty, value);
            }
            get
            {
                return (bool)GetValue(CumulateAtBoundProperty);
            }
        }

        public static DependencyProperty BlendColorProperty = DependencyProperty.Register("BlendColor", typeof(Media.Color), typeof(ParticleStormModel3D),
            new PropertyMetadata(Media.Colors.White,
                (d, e) =>
                {
                    (d as ParticleStormModel3D).particleCore.InsertVariables.ParticleBlendColor = ((Media.Color)e.NewValue).ToColor4();
                }));

        public Media.Color BlendColor
        {
            set
            {
                SetValue(BlendColorProperty, value);
            }
            get
            {
                return (Media.Color)GetValue(BlendColorProperty);
            }
        }

        public static DependencyProperty AnimateSpriteByEnergyBoundProperty = DependencyProperty.Register("AnimateSpriteByEnergy", typeof(bool), typeof(ParticleStormModel3D),
            new PropertyMetadata(false,
                (d, e) =>
                {
                    (d as ParticleStormModel3D).particleCore.AnimateSpriteByEnergy = (bool)e.NewValue;
                }));

        public bool AnimateSpriteByEnergy
        {
            set
            {
                SetValue(AnimateSpriteByEnergyBoundProperty, value);
            }
            get
            {
                return (bool)GetValue(AnimateSpriteByEnergyBoundProperty);
            }
        }

        public static DependencyProperty BlendProperty = DependencyProperty.Register("Blend", typeof(BlendOperation), typeof(ParticleStormModel3D),
            new PropertyMetadata(BlendOperation.Add, (d, e) =>
            {
                (d as ParticleStormModel3D).OnBlendStateChanged();
            }));

        public BlendOperation Blend
        {
            set
            {
                SetValue(BlendProperty, value);
            }
            get
            {
                return (BlendOperation)GetValue(BlendProperty);
            }
        }

        public static DependencyProperty AlphaBlendProperty = DependencyProperty.Register("AlphaBlend", typeof(BlendOperation), typeof(ParticleStormModel3D),
            new PropertyMetadata(BlendOperation.Add, (d, e) =>
            {
                (d as ParticleStormModel3D).OnBlendStateChanged();
            }));

        public BlendOperation AlphaBlend
        {
            set
            {
                SetValue(AlphaBlendProperty, value);
            }
            get
            {
                return (BlendOperation)GetValue(AlphaBlendProperty);
            }
        }

        public static DependencyProperty SourceBlendProperty = DependencyProperty.Register("SourceBlend", typeof(BlendOption), typeof(ParticleStormModel3D),
            new PropertyMetadata(BlendOption.One, (d, e) =>
            {
                (d as ParticleStormModel3D).OnBlendStateChanged();
            }));

        public BlendOption SourceBlend
        {
            set
            {
                SetValue(SourceBlendProperty, value);
            }
            get
            {
                return (BlendOption)GetValue(SourceBlendProperty);
            }
        }

        public static DependencyProperty DestBlendProperty = DependencyProperty.Register("DestBlend", typeof(BlendOption), typeof(ParticleStormModel3D),
            new PropertyMetadata(BlendOption.One, (d, e) =>
            {
                (d as ParticleStormModel3D).OnBlendStateChanged();
            }));

        public BlendOption DestBlend
        {
            set
            {
                SetValue(DestBlendProperty, value);
            }
            get
            {
                return (BlendOption)GetValue(DestBlendProperty);
            }
        }

        public static DependencyProperty SourceAlphaBlendProperty = DependencyProperty.Register("SourceAlphaBlend", typeof(BlendOption), typeof(ParticleStormModel3D),
            new PropertyMetadata(BlendOption.One, (d, e) =>
            {
                (d as ParticleStormModel3D).OnBlendStateChanged();
            }));

        public BlendOption SourceAlphaBlend
        {
            set
            {
                SetValue(SourceAlphaBlendProperty, value);
            }
            get
            {
                return (BlendOption)GetValue(SourceAlphaBlendProperty);
            }
        }

        public static DependencyProperty DestAlphaBlendProperty = DependencyProperty.Register("DestAlphaBlend", typeof(BlendOption), typeof(ParticleStormModel3D),
            new PropertyMetadata(BlendOption.Zero, (d, e) =>
            {
                (d as ParticleStormModel3D).OnBlendStateChanged();
            }));

        public BlendOption DestAlphaBlend
        {
            set
            {
                SetValue(DestAlphaBlendProperty, value);
            }
            get
            {
                return (BlendOption)GetValue(DestAlphaBlendProperty);
            }
        }

        /// <summary>
        /// List of instance matrix. 
        /// </summary>
        public IList<Matrix> Instances
        {
            get { return (IList<Matrix>)this.GetValue(InstancesProperty); }
            set { this.SetValue(InstancesProperty, value); }
        }

        /// <summary>
        /// List of instance matrix.
        /// </summary>
        public static readonly DependencyProperty InstancesProperty =
            DependencyProperty.Register("Instances", typeof(IList<Matrix>), typeof(ParticleStormModel3D), new AffectsRenderPropertyMetadata(null, InstancesChanged));

        /// <summary>
        /// 
        /// </summary>
        private static void InstancesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var model = (ParticleStormModel3D)d;
            model.InstanceBuffer.Elements = e.NewValue == null ? null : e.NewValue as IList<Matrix>;
        }
        #endregion

        public bool HasInstances { get { return InstanceBuffer.HasElements; } }
        protected readonly IElementsBufferModel<Matrix> InstanceBuffer = new MatrixInstanceBufferModel();

        private ParticleRenderCore particleCore
        {
            get
            {
                return (ParticleRenderCore)RenderCore;
            }
        }
        private bool blendChanged = true;

        private void OnBlendStateChanged()
        {
            blendChanged = true;
        }

        protected override IRenderCore OnCreateRenderCore()
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
            particleCore.InstanceBuffer = InstanceBuffer;
            Media.CompositionTarget.Rendering += CompositionTarget_Rendering;
            return true;
        }

        protected override void OnRender(IRenderContext context)
        {
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
            base.OnRender(context);
        }

        private void CompositionTarget_Rendering(object sender, System.EventArgs e)
        {
            InvalidateRender();
        }

        protected override void OnDetach()
        {
            Media.CompositionTarget.Rendering -= CompositionTarget_Rendering;
            InstanceBuffer.Dispose();
            base.OnDetach();
        }
    }
}
