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
using System;
using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;

#if NETFX_CORE
using  Windows.UI.Xaml;
using Media = Windows.UI;
using Windows.Foundation;
using Vector3D = SharpDX.Vector3;

namespace HelixToolkit.UWP
#elif WINUI 
using Microsoft.UI.Xaml;
using Media = Windows.UI;
using Windows.Foundation;
using Vector3D = SharpDX.Vector3;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.SharpDX.Core.Utilities;
using static HelixToolkit.SharpDX.Core.Core.ParticleRenderCore;
namespace HelixToolkit.WinUI
#else
using System.Windows;
using Media = System.Windows.Media;
using Media3D = System.Windows.Media.Media3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
#if COREWPF
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.SharpDX.Core.Utilities;
using static HelixToolkit.SharpDX.Core.Core.ParticleRenderCore;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
#if !COREWPF && !WINUI
    using Model.Scene;
    using Utilities;
    using static Core.ParticleRenderCore;
#endif

    public class ParticleStormModel3D : Element3D
    {
        #region Dependency Properties
        public static DependencyProperty ParticleCountProperty = DependencyProperty.Register("ParticleCount", typeof(int), typeof(ParticleStormModel3D),
            new PropertyMetadata(DefaultParticleCount,
            (d, e) =>
            {
                ((d as Element3DCore).SceneNode as ParticleStormNode).ParticleCount = Math.Max(8, (int)e.NewValue);
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
#if NETFX_CORE || WINUI
        public static DependencyProperty EmitterLocationProperty = DependencyProperty.Register("EmitterLocation", typeof(Vector3), typeof(ParticleStormModel3D),
            new PropertyMetadata(DefaultEmitterLocation,
            (d, e) =>
            {
                ((d as Element3DCore).SceneNode as ParticleStormNode).EmitterLocation = (Vector3)e.NewValue;
            }
            ));

        public Vector3 EmitterLocation
        {
            set
            {
                SetValue(EmitterLocationProperty, value);
            }
            get
            {
                return (Vector3)GetValue(EmitterLocationProperty);
            }
        }

        public static DependencyProperty ConsumerLocationProperty = DependencyProperty.Register("ConsumerLocation", typeof(Vector3), typeof(ParticleStormModel3D),
            new PropertyMetadata(DefaultConsumerLocation,
            (d, e) =>
            {
                ((d as Element3DCore).SceneNode as ParticleStormNode).ConsumerLocation = (Vector3)e.NewValue;
            }
            ));

        public Vector3 ConsumerLocation
        {
            set
            {
                SetValue(ConsumerLocationProperty, value);
            }
            get
            {
                return (Vector3)GetValue(ConsumerLocationProperty);
            }
        }

        public static DependencyProperty ParticleBoundsProperty = DependencyProperty.Register("ParticleBounds", typeof(BoundingBox), typeof(ParticleStormModel3D),
            new PropertyMetadata(new BoundingBox(new Vector3D(-50, -50, -50), new Vector3D(50, 50, 50)),
            (d, e) =>
            {
                var bound = (BoundingBox)e.NewValue;
                ((d as Element3DCore).SceneNode as ParticleStormNode).DomainBoundMax = bound.Maximum;
                ((d as Element3DCore).SceneNode as ParticleStormNode).DomainBoundMin = bound.Minimum;
            }));

        public BoundingBox ParticleBounds
        {
            set
            {
                SetValue(ParticleBoundsProperty, value);
            }
            get
            {
                return (BoundingBox)GetValue(ParticleBoundsProperty);
            }
        }
#else
        public static DependencyProperty EmitterLocationProperty = DependencyProperty.Register("EmitterLocation", typeof(Media3D.Point3D), typeof(ParticleStormModel3D),
            new PropertyMetadata(DefaultEmitterLocation.ToPoint3D(),
            (d, e) =>
            {
                ((d as Element3DCore).SceneNode as ParticleStormNode).EmitterLocation = (((Media3D.Point3D)e.NewValue).ToVector3());
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

        public static DependencyProperty ConsumerLocationProperty = DependencyProperty.Register("ConsumerLocation", typeof(Media3D.Point3D), typeof(ParticleStormModel3D),
            new PropertyMetadata(DefaultConsumerLocation.ToPoint3D(),
            (d, e) =>
            {
                ((d as Element3DCore).SceneNode as ParticleStormNode).ConsumerLocation = (((Media3D.Point3D)e.NewValue).ToVector3());
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

        public static DependencyProperty ParticleBoundsProperty = DependencyProperty.Register("ParticleBounds", typeof(Media3D.Rect3D), typeof(ParticleStormModel3D),
            new PropertyMetadata(new Media3D.Rect3D(0, 0, 0, 100, 100, 100),
            (d, e) =>
            {
                var bound = (Media3D.Rect3D)e.NewValue;
                ((d as Element3DCore).SceneNode as ParticleStormNode).DomainBoundMax = new Vector3((float)(bound.SizeX / 2 + bound.Location.X), (float)(bound.SizeY / 2 + bound.Location.Y), (float)(bound.SizeZ / 2 + bound.Location.Z));
                ((d as Element3DCore).SceneNode as ParticleStormNode).DomainBoundMin = new Vector3((float)(bound.Location.X - bound.SizeX / 2), (float)(bound.Location.Y - bound.SizeY / 2), (float)(bound.Location.Z - bound.SizeZ / 2));
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
#endif

        public static DependencyProperty EmitterRadiusProperty = DependencyProperty.Register("EmitterRadius", typeof(double), typeof(ParticleStormModel3D),
            new PropertyMetadata(0.0,
            (d, e) =>
            {
                ((d as Element3DCore).SceneNode as ParticleStormNode).EmitterRadius = (float)(double)e.NewValue;
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

        public static DependencyProperty ConsumerGravityProperty = DependencyProperty.Register("ConsumerGravity", typeof(double), typeof(ParticleStormModel3D),
            new PropertyMetadata(0.0,
            (d, e) =>
            {
                ((d as Element3DCore).SceneNode as ParticleStormNode).ConsumerGravity = ((float)(double)e.NewValue);
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
                ((d as Element3DCore).SceneNode as ParticleStormNode).ConsumerRadius = (float)(double)e.NewValue;
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
                ((d as Element3DCore).SceneNode as ParticleStormNode).InitialEnergy = System.Math.Max(1f, (float)(double)e.NewValue);
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
                ((d as Element3DCore).SceneNode as ParticleStormNode).EnergyDissipationRate = System.Math.Max(1f, (float)(double)e.NewValue);
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
                ((d as Element3DCore).SceneNode as ParticleStormNode).RandomVectorGenerator = (IRandomVector)e.NewValue;
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

        public static DependencyProperty ParticleTextureProperty = DependencyProperty.Register("ParticleTexture", typeof(TextureModel), typeof(ParticleStormModel3D),
            new PropertyMetadata(null,
            (d, e) =>
            {
                ((d as Element3DCore).SceneNode as ParticleStormNode).ParticleTexture = (TextureModel)e.NewValue;
            }
            ));

        public TextureModel ParticleTexture
        {
            set
            {
                SetValue(ParticleTextureProperty, value);
            }
            get
            {
                return (TextureModel)GetValue(ParticleTextureProperty);
            }
        }

        public static DependencyProperty NumTextureColumnProperty = DependencyProperty.Register("NumTextureColumn", typeof(int), typeof(ParticleStormModel3D),
            new PropertyMetadata(1,
            (d, e) =>
            {
                ((d as Element3DCore).SceneNode as ParticleStormNode).NumTextureColumn = (uint)System.Math.Max(1, (int)e.NewValue);
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
                ((d as Element3DCore).SceneNode as ParticleStormNode).NumTextureRow = (uint)System.Math.Max(1, (int)e.NewValue);
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
            new PropertyMetadata(new Size(1, 1),
                (d, e) =>
                {
                    var size = (Size)e.NewValue;
                    ((d as Element3DCore).SceneNode as ParticleStormNode).ParticleSize = new Vector2((float)size.Width, (float)size.Height);
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
                ((d as Element3DCore).SceneNode as ParticleStormNode).InitialVelocity = (float)(double)e.NewValue;
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

#if NETFX_CORE || WINUI
        public static DependencyProperty AccelerationProperty = DependencyProperty.Register("Acceleration", typeof(Vector3D), typeof(ParticleStormModel3D),
            new PropertyMetadata(DefaultAcceleration,
            (d, e) =>
            {
                ((d as Element3DCore).SceneNode as ParticleStormNode).InitAcceleration = (Vector3D)e.NewValue;
            }
            ));
#else
        public static DependencyProperty AccelerationProperty = DependencyProperty.Register("Acceleration", typeof(Vector3D), typeof(ParticleStormModel3D),
            new PropertyMetadata(DefaultAcceleration.ToVector3D(),
            (d, e) =>
            {
                ((d as Element3DCore).SceneNode as ParticleStormNode).InitAcceleration = ((Vector3D)e.NewValue).ToVector3();

            }
            ));
#endif

        public Vector3D Acceleration
        {
            set
            {
                SetValue(AccelerationProperty, value);
            }
            get
            {
                return (Vector3D)GetValue(AccelerationProperty);
            }
        }

        public static DependencyProperty CumulateAtBoundProperty = DependencyProperty.Register("CumulateAtBound", typeof(bool), typeof(ParticleStormModel3D),
            new PropertyMetadata(false,
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as ParticleStormNode).CumulateAtBound = (bool)e.NewValue;
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
#if WINUI
                new PropertyMetadata(Microsoft.UI.Colors.White,
#else
                new PropertyMetadata(Media.Colors.White,
#endif
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as ParticleStormNode).BlendColor = ((Media.Color)e.NewValue).ToColor4();
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
                    ((d as Element3DCore).SceneNode as ParticleStormNode).AnimateSpriteByEnergy = (bool)e.NewValue;
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



        public double Turbulance
        {
            get
            {
                return (double)GetValue(TurbulanceProperty);
            }
            set
            {
                SetValue(TurbulanceProperty, value);
            }
        }


        public static readonly DependencyProperty TurbulanceProperty =
            DependencyProperty.Register("Turbulance", typeof(double), typeof(ParticleStormModel3D), new PropertyMetadata(0.0, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as ParticleStormNode).Turbulance = (float)(double)e.NewValue;
            }));



        public static DependencyProperty BlendProperty = DependencyProperty.Register("Blend", typeof(BlendOperation), typeof(ParticleStormModel3D),
            new PropertyMetadata(BlendOperation.Add, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as ParticleStormNode).Blend = (BlendOperation)e.NewValue;
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
                ((d as Element3DCore).SceneNode as ParticleStormNode).AlphaBlend = (BlendOperation)e.NewValue;
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
                ((d as Element3DCore).SceneNode as ParticleStormNode).SourceBlend = (BlendOption)e.NewValue;
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
                ((d as Element3DCore).SceneNode as ParticleStormNode).DestBlend = (BlendOption)e.NewValue;
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
                ((d as Element3DCore).SceneNode as ParticleStormNode).SourceAlphaBlend = (BlendOption)e.NewValue;
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
                ((d as Element3DCore).SceneNode as ParticleStormNode).DestAlphaBlend = (BlendOption)e.NewValue;
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
        /// Gets or sets the blend factor for blending
        /// </summary>
        /// <value>
        /// The blend factor.
        /// </value>
        public Media.Color BlendFactor
        {
            get
            {
                return (Media.Color)GetValue(BlendFactorProperty);
            }
            set
            {
                SetValue(BlendFactorProperty, value);
            }
        }

        /// <summary>
        /// The blend factor property
        /// </summary>
        public static readonly DependencyProperty BlendFactorProperty =
            DependencyProperty.Register("BlendFactor", typeof(Media.Color), typeof(ParticleStormModel3D),
#if WINUI
                new PropertyMetadata(Microsoft.UI.Colors.White, (d, e) =>
#else
                new PropertyMetadata(Media.Colors.White, (d, e) =>
#endif
                {
                    ((d as Element3DCore).SceneNode as ParticleStormNode).BlendFactor = ((Media.Color)e.NewValue).ToColor4();
                }));


        /// <summary>
        /// Gets or sets the sample mask used during blending
        /// </summary>
        /// <value>
        /// The sample mask.
        /// </value>
        public int SampleMask
        {
            get
            {
                return (int)GetValue(SampleMaskProperty);
            }
            set
            {
                SetValue(SampleMaskProperty, value);
            }
        }

        /// <summary>
        /// The sample mask property
        /// </summary>
        public static readonly DependencyProperty SampleMaskProperty =
            DependencyProperty.Register("SampleMask", typeof(int), typeof(ParticleStormModel3D), new PropertyMetadata(-1, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as ParticleStormNode).SampleMask = (int)e.NewValue;
            }));


        /// <summary>
        /// List of instance matrix. 
        /// </summary>
        public IList<Matrix> Instances
        {
            get
            {
                return (IList<Matrix>)this.GetValue(InstancesProperty);
            }
            set
            {
                this.SetValue(InstancesProperty, value);
            }
        }

        /// <summary>
        /// List of instance matrix.
        /// </summary>
        public static readonly DependencyProperty InstancesProperty =
            DependencyProperty.Register("Instances", typeof(IList<Matrix>), typeof(ParticleStormModel3D), new PropertyMetadata(null, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as ParticleStormNode).Instances = e.NewValue as IList<Matrix>;
            }));

        /// <summary>
        /// The enable view frustum check property
        /// </summary>
        public static readonly DependencyProperty EnableViewFrustumCheckProperty =
            DependencyProperty.Register("EnableViewFrustumCheck", typeof(bool), typeof(ParticleStormModel3D), new PropertyMetadata(true,
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as ParticleStormNode).EnableViewFrustumCheck = (bool)e.NewValue;
                }));
        #endregion


        protected override SceneNode OnCreateSceneNode()
        {
            return new ParticleStormNode();
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode node)
        {
            base.AssignDefaultValuesToSceneNode(node);
            if (node is ParticleStormNode c)
            {
                c.ParticleCount = ParticleCount;

                c.EmitterRadius = (float)EmitterRadius;
                c.ConsumerGravity = (float)ConsumerGravity;

                c.ConsumerRadius = (float)ConsumerRadius;
                c.InitialEnergy = (float)InitialEnergy;
                c.EnergyDissipationRate = (float)EnergyDissipationRate;
                c.RandomVectorGenerator = RandomVectorGenerator;
                c.ParticleTexture = ParticleTexture;
                c.NumTextureColumn = (uint)NumTextureColumn;
                c.NumTextureRow = (uint)NumTextureRow;
                c.ParticleSize = new Vector2((float)ParticleSize.Width, (float)ParticleSize.Height);
                c.InitialVelocity = (float)InitialVelocity;


                c.CumulateAtBound = CumulateAtBound;
                c.BlendColor = BlendColor.ToColor4();
                c.AnimateSpriteByEnergy = AnimateSpriteByEnergy;
                c.Turbulance = (float)Turbulance;
                c.Blend = Blend;
                c.AlphaBlend = AlphaBlend;
                c.SourceBlend = SourceBlend;
                c.DestBlend = DestBlend;
                c.SourceAlphaBlend = SourceAlphaBlend;
                c.DestAlphaBlend = DestAlphaBlend;
                c.SampleMask = SampleMask;
                c.BlendColor = BlendColor.ToColor4();
#if NETFX_CORE || WINUI
                c.EmitterLocation = EmitterLocation;
                c.ConsumerLocation = ConsumerLocation;
                c.InitAcceleration = Acceleration;
                c.DomainBoundMax = ParticleBounds.Maximum;
                c.DomainBoundMin = ParticleBounds.Minimum;
#else
                c.EmitterLocation = EmitterLocation.ToVector3();
                c.ConsumerLocation = ConsumerLocation.ToVector3();
                c.InitAcceleration = Acceleration.ToVector3();
                c.DomainBoundMax = new Vector3((float)(ParticleBounds.SizeX / 2 + ParticleBounds.Location.X), (float)(ParticleBounds.SizeY / 2 + ParticleBounds.Location.Y), (float)(ParticleBounds.SizeZ / 2 + ParticleBounds.Location.Z));
                c.DomainBoundMin = new Vector3((float)(ParticleBounds.Location.X - ParticleBounds.SizeX / 2), (float)(ParticleBounds.Location.Y - ParticleBounds.SizeY / 2), (float)(ParticleBounds.Location.Z - ParticleBounds.SizeZ / 2));
#endif
            }
        }
    }
}
