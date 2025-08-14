using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene;
using HelixToolkit.SharpDX.Utilities;
using static HelixToolkit.SharpDX.Core.ParticleRenderCore;
using SharpDX;
using SharpDX.Direct3D11;

#if false
#elif WINUI
using HelixToolkit.WinUI.SharpDX.Model;
#elif WPF
using HelixToolkit.Wpf.SharpDX.Model;
using System.Windows;
using Rect3D = System.Windows.Media.Media3D.Rect3D;
#elif AVALONIA
using HelixToolkit.Avalonia.SharpDX.Model;
using Avalonia;
#else
#error Unknown framework
#endif

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#elif AVALONIA
namespace HelixToolkit.Avalonia.SharpDX;
#else
#error Unknown framework
#endif

public class ParticleStormModel3D : Element3D
{
    #region Dependency Properties
    public static readonly DependencyProperty ParticleCountProperty =
        HelixProperty.Register<ParticleStormModel3D, int>("ParticleCount",
            DefaultParticleCount,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.ParticleCount = Math.Max(8, (int)e.NewValue!);
                }
            });

    public int ParticleCount
    {
        set
        {
            SetValue(ParticleCountProperty, value);
        }
        get
        {
            return (int)GetValue(ParticleCountProperty)!;
        }
    }

    public static readonly DependencyProperty EmitterLocationProperty =
        HelixProperty.Register<ParticleStormModel3D, Point3D>("EmitterLocation",
            DefaultEmitterLocation.ToPoint3D(),
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.EmitterLocation = (((Point3D)e.NewValue!).ToVector3());
                }
            });

    public Point3D EmitterLocation
    {
        set
        {
            SetValue(EmitterLocationProperty, value);
        }
        get
        {
            return (Point3D)GetValue(EmitterLocationProperty)!;
        }
    }

    public static readonly DependencyProperty ConsumerLocationProperty =
        HelixProperty.Register<ParticleStormModel3D, Point3D>("ConsumerLocation",
            DefaultConsumerLocation.ToPoint3D(),
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.ConsumerLocation = (((Point3D)e.NewValue!).ToVector3());
                }
            });

    public Point3D ConsumerLocation
    {
        set
        {
            SetValue(ConsumerLocationProperty, value);
        }
        get
        {
            return (Point3D)GetValue(ConsumerLocationProperty)!;
        }
    }

#if false
#elif WINUI
    public static readonly DependencyProperty ParticleBoundsProperty =
        HelixProperty.Register<ParticleStormModel3D, BoundingBox>("ParticleBounds",
        new BoundingBox(new Vector3(-50, -50, -50), new Vector3(50, 50, 50)),
        (d, e) =>
        {
            if (d is Element3DCore { SceneNode: ParticleStormNode node })
            {
                var bound = (BoundingBox)e.NewValue!;
                node.DomainBoundMax = bound.Maximum;
                node.DomainBoundMin = bound.Minimum;
            }
        });

    public BoundingBox ParticleBounds
    {
        set
        {
            SetValue(ParticleBoundsProperty, value);
        }
        get
        {
            return (BoundingBox)GetValue(ParticleBoundsProperty)!;
        }
    }
#elif WPF
    public static readonly DependencyProperty ParticleBoundsProperty =
        HelixProperty.Register<ParticleStormModel3D, Rect3D>("ParticleBounds",
        new Rect3D(0, 0, 0, 100, 100, 100),
        (d, e) =>
        {
            if (d is Element3DCore { SceneNode: ParticleStormNode node })
            {
                var bound = (Rect3D)e.NewValue!;
                node.DomainBoundMax = new Vector3((float)(bound.SizeX / 2 + bound.Location.X), (float)(bound.SizeY / 2 + bound.Location.Y), (float)(bound.SizeZ / 2 + bound.Location.Z));
                node.DomainBoundMin = new Vector3((float)(bound.Location.X - bound.SizeX / 2), (float)(bound.Location.Y - bound.SizeY / 2), (float)(bound.Location.Z - bound.SizeZ / 2));
            }
        });

    public Rect3D ParticleBounds
    {
        set
        {
            SetValue(ParticleBoundsProperty, value);
        }
        get
        {
            return (Rect3D)GetValue(ParticleBoundsProperty);
        }
    }
#elif AVALONIA
    public static readonly DependencyProperty ParticleBoundsProperty =
        HelixProperty.Register<ParticleStormModel3D, BoundingBox>("ParticleBounds",
            new BoundingBox(new Vector3(-50, -50, -50), new Vector3(50, 50, 50)),
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    var bound = (BoundingBox)e.NewValue!;
                    node.DomainBoundMax = bound.Maximum;
                    node.DomainBoundMin = bound.Minimum;
                }
            });

    public BoundingBox ParticleBounds
    {
        set
        {
            SetValue(ParticleBoundsProperty, value);
        }
        get
        {
            return (BoundingBox)GetValue(ParticleBoundsProperty)!;
        }
    }
#else
#error Unknown framework
#endif

    public static readonly DependencyProperty EmitterRadiusProperty =
        HelixProperty.Register<ParticleStormModel3D, double>("EmitterRadius",
            0.0,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.EmitterRadius = (float)(double)e.NewValue!;
                }
            });

    public double EmitterRadius
    {
        set
        {
            SetValue(EmitterRadiusProperty, value);
        }
        get
        {
            return (double)GetValue(EmitterRadiusProperty)!;
        }
    }

    public static readonly DependencyProperty ConsumerGravityProperty =
        HelixProperty.Register<ParticleStormModel3D, double>("ConsumerGravity",
            0.0,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.ConsumerGravity = ((float)(double)e.NewValue!);
                }
            });

    public double ConsumerGravity
    {
        set
        {
            SetValue(ConsumerGravityProperty, value);
        }
        get
        {
            return (double)GetValue(ConsumerGravityProperty)!;
        }
    }

    public static readonly DependencyProperty ConsumerRadiusProperty =
        HelixProperty.Register<ParticleStormModel3D, double>("ConsumerRadius",
            0.0,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.ConsumerRadius = (float)(double)e.NewValue!;
                }
            });

    public double ConsumerRadius
    {
        set
        {
            SetValue(ConsumerRadiusProperty, value);
        }
        get
        {
            return (double)GetValue(ConsumerRadiusProperty)!;
        }
    }

    public static readonly DependencyProperty InitialEnergyProperty =
        HelixProperty.Register<ParticleStormModel3D, double>("InitialEnergy",
            5.0,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.InitialEnergy = Math.Max(1f, (float)(double)e.NewValue!);
                }
            });

    public double InitialEnergy
    {
        set
        {
            SetValue(InitialEnergyProperty, value);
        }
        get
        {
            return (double)GetValue(InitialEnergyProperty)!;
        }
    }

    public static readonly DependencyProperty EnergyDissipationRateProperty =
        HelixProperty.Register<ParticleStormModel3D, double>("EnergyDissipationRate",
            1.0,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.EnergyDissipationRate = Math.Max(1f, (float)(double)e.NewValue!);
                }
            });

    public double EnergyDissipationRate
    {
        set
        {
            SetValue(EnergyDissipationRateProperty, value);
        }
        get
        {
            return (double)GetValue(EnergyDissipationRateProperty)!;
        }
    }

    public static readonly DependencyProperty RandomVectorGeneratorProperty =
        HelixProperty.Register<ParticleStormModel3D, IRandomVector>("RandomVectorGenerator",
            new UniformRandomVectorGenerator(),
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.RandomVectorGenerator = (IRandomVector)e.NewValue!;
                }
            });

    public IRandomVector RandomVectorGenerator
    {
        set
        {
            SetValue(RandomVectorGeneratorProperty, value);
        }
        get
        {
            return (IRandomVector)GetValue(RandomVectorGeneratorProperty)!;
        }
    }

    public static readonly DependencyProperty ParticleTextureProperty =
        HelixProperty.Register<ParticleStormModel3D, TextureModel?>("ParticleTexture",
            null,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.ParticleTexture = (TextureModel?)e.NewValue;
                }
            });

    public TextureModel? ParticleTexture
    {
        set
        {
            SetValue(ParticleTextureProperty, value);
        }
        get
        {
            return (TextureModel?)GetValue(ParticleTextureProperty);
        }
    }

    public static readonly DependencyProperty NumTextureColumnProperty =
        HelixProperty.Register<ParticleStormModel3D, int>("NumTextureColumn",
            1,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.NumTextureColumn = (uint)System.Math.Max(1, (int)e.NewValue!);
                }
            });

    public int NumTextureColumn
    {
        set
        {
            SetValue(NumTextureColumnProperty, value);
        }
        get
        {
            return (int)GetValue(NumTextureColumnProperty)!;
        }
    }

    public static readonly DependencyProperty NumTextureRowProperty =
        HelixProperty.Register<ParticleStormModel3D, int>("NumTextureRow",
            1,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.NumTextureRow = (uint)System.Math.Max(1, (int)e.NewValue!);
                }
            });

    public int NumTextureRow
    {
        set
        {
            SetValue(NumTextureRowProperty, value);
        }
        get
        {
            return (int)GetValue(NumTextureRowProperty)!;
        }
    }

    public static readonly DependencyProperty ParticleSizeProperty =
        HelixProperty.Register<ParticleStormModel3D, Size>("ParticleSize",
            new Size(1, 1),
            (d, e) =>
            {
                var size = (Size)e.NewValue!;
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.ParticleSize = new Vector2((float)size.Width, (float)size.Height);
                }
            });

    public Size ParticleSize
    {
        set
        {
            SetValue(ParticleSizeProperty, value);
        }
        get
        {
            return (Size)GetValue(ParticleSizeProperty)!;
        }
    }


    public static readonly DependencyProperty InitialVelocityProperty =
        HelixProperty.Register<ParticleStormModel3D, double>("InitialVelocity",
            1.0,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.InitialVelocity = (float)(double)e.NewValue!;
                }
            });

    public double InitialVelocity
    {
        set
        {
            SetValue(InitialVelocityProperty, value);
        }
        get
        {
            return (double)GetValue(InitialVelocityProperty)!;
        }
    }

    public static readonly DependencyProperty AccelerationProperty =
        HelixProperty.Register<ParticleStormModel3D, Vector3D>("Acceleration",
            DefaultAcceleration.ToVector3D(),
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.InitAcceleration = ((Vector3D)e.NewValue!).ToVector3();
                }
            });

    public Vector3D Acceleration
    {
        set
        {
            SetValue(AccelerationProperty, value);
        }
        get
        {
            return (Vector3D)GetValue(AccelerationProperty)!;
        }
    }

    public static readonly DependencyProperty CumulateAtBoundProperty =
        HelixProperty.Register<ParticleStormModel3D, bool>("CumulateAtBound",
            false,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.CumulateAtBound = (bool)e.NewValue!;
                }
            });

    public bool CumulateAtBound
    {
        set
        {
            SetValue(CumulateAtBoundProperty, value);
        }
        get
        {
            return (bool)GetValue(CumulateAtBoundProperty)!;
        }
    }

    public static readonly DependencyProperty BlendColorProperty =
        HelixProperty.Register<ParticleStormModel3D, UIColor>("BlendColor",
            UIColors.White,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.BlendColor = ((UIColor)e.NewValue!).ToColor4();
                }
            });

    public UIColor BlendColor
    {
        set
        {
            SetValue(BlendColorProperty, value);
        }
        get
        {
            return (UIColor)GetValue(BlendColorProperty)!;
        }
    }

    public static readonly DependencyProperty AnimateSpriteByEnergyBoundProperty =
        HelixProperty.Register<ParticleStormModel3D, bool>("AnimateSpriteByEnergy",
            false,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.AnimateSpriteByEnergy = (bool)e.NewValue!;
                }
            });

    public bool AnimateSpriteByEnergy
    {
        set
        {
            SetValue(AnimateSpriteByEnergyBoundProperty, value);
        }
        get
        {
            return (bool)GetValue(AnimateSpriteByEnergyBoundProperty)!;
        }
    }

    public double Turbulance
    {
        get
        {
            return (double)GetValue(TurbulanceProperty)!;
        }
        set
        {
            SetValue(TurbulanceProperty, value);
        }
    }


    public static readonly DependencyProperty TurbulanceProperty =
        HelixProperty.Register<ParticleStormModel3D, double>("Turbulance",
            0.0,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.Turbulance = (float)(double)e.NewValue!;
                }
            });

    public static readonly DependencyProperty BlendProperty =
        HelixProperty.Register<ParticleStormModel3D, BlendOperation>("Blend",
            BlendOperation.Add,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.Blend = (BlendOperation)e.NewValue!;
                }
            });

    public BlendOperation Blend
    {
        set
        {
            SetValue(BlendProperty, value);
        }
        get
        {
            return (BlendOperation)GetValue(BlendProperty)!;
        }
    }

    public static readonly DependencyProperty AlphaBlendProperty =
        HelixProperty.Register<ParticleStormModel3D, BlendOperation>("AlphaBlend",
            BlendOperation.Add,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.AlphaBlend = (BlendOperation)e.NewValue!;
                }
            });

    public BlendOperation AlphaBlend
    {
        set
        {
            SetValue(AlphaBlendProperty, value);
        }
        get
        {
            return (BlendOperation)GetValue(AlphaBlendProperty)!;
        }
    }

    public static readonly DependencyProperty SourceBlendProperty =
        HelixProperty.Register<ParticleStormModel3D, BlendOption>("SourceBlend",
            BlendOption.One,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.SourceBlend = (BlendOption)e.NewValue!;
                }
            });

    public BlendOption SourceBlend
    {
        set
        {
            SetValue(SourceBlendProperty, value);
        }
        get
        {
            return (BlendOption)GetValue(SourceBlendProperty)!;
        }
    }

    public static readonly DependencyProperty DestBlendProperty =
        HelixProperty.Register<ParticleStormModel3D, BlendOption>("DestBlend",
            BlendOption.One,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.DestBlend = (BlendOption)e.NewValue!;
                }
            });

    public BlendOption DestBlend
    {
        set
        {
            SetValue(DestBlendProperty, value);
        }
        get
        {
            return (BlendOption)GetValue(DestBlendProperty)!;
        }
    }

    public static readonly DependencyProperty SourceAlphaBlendProperty =
        HelixProperty.Register<ParticleStormModel3D, BlendOption>("SourceAlphaBlend",
            BlendOption.One,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.SourceAlphaBlend = (BlendOption)e.NewValue!;
                }
            });

    public BlendOption SourceAlphaBlend
    {
        set
        {
            SetValue(SourceAlphaBlendProperty, value);
        }
        get
        {
            return (BlendOption)GetValue(SourceAlphaBlendProperty)!;
        }
    }

    public static readonly DependencyProperty DestAlphaBlendProperty =
        HelixProperty.Register<ParticleStormModel3D, BlendOption>("DestAlphaBlend",
            BlendOption.Zero,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.DestAlphaBlend = (BlendOption)e.NewValue!;
                }
            });

    public BlendOption DestAlphaBlend
    {
        set
        {
            SetValue(DestAlphaBlendProperty, value);
        }
        get
        {
            return (BlendOption)GetValue(DestAlphaBlendProperty)!;
        }
    }
    /// <summary>
    /// Gets or sets the blend factor for blending
    /// </summary>
    /// <value>
    /// The blend factor.
    /// </value>
    public UIColor BlendFactor
    {
        get
        {
            return (UIColor)GetValue(BlendFactorProperty)!;
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
        HelixProperty.Register<ParticleStormModel3D, UIColor>("BlendFactor",
            UIColors.White,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.BlendFactor = ((UIColor)e.NewValue!).ToColor4();
                }
            });

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
            return (int)GetValue(SampleMaskProperty)!;
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
        HelixProperty.Register<ParticleStormModel3D, int>("SampleMask",
            -1,
                (d, e) =>
                {
                    if (d is Element3DCore { SceneNode: ParticleStormNode node })
                    {
                        node.SampleMask = (int)e.NewValue!;
                    }
                });

    /// <summary>
    /// List of instance matrix. 
    /// </summary>
    public IList<Matrix>? Instances
    {
        get
        {
            return (IList<Matrix>?)this.GetValue(InstancesProperty);
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
        HelixProperty.Register<ParticleStormModel3D, IList<Matrix>?>("Instances",
            null, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: ParticleStormNode node })
            {
                node.Instances = e.NewValue as IList<Matrix>;
            }
        });

    /// <summary>
    /// The enable view frustum check property
    /// </summary>
    public static readonly DependencyProperty EnableViewFrustumCheckProperty =
        HelixProperty.Register<ParticleStormModel3D, bool>("EnableViewFrustumCheck",
            true,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.EnableViewFrustumCheck = (bool)e.NewValue!;
                }
            });
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
#if false
#elif WINUI
            c.EmitterLocation = EmitterLocation;
            c.ConsumerLocation = ConsumerLocation;
            c.InitAcceleration = Acceleration;
            c.DomainBoundMax = ParticleBounds.Maximum;
            c.DomainBoundMin = ParticleBounds.Minimum;
#elif WPF
            c.EmitterLocation = EmitterLocation.ToVector3();
            c.ConsumerLocation = ConsumerLocation.ToVector3();
            c.InitAcceleration = Acceleration.ToVector3();
            c.DomainBoundMax = new Vector3((float)(ParticleBounds.SizeX / 2 + ParticleBounds.Location.X), (float)(ParticleBounds.SizeY / 2 + ParticleBounds.Location.Y), (float)(ParticleBounds.SizeZ / 2 + ParticleBounds.Location.Z));
            c.DomainBoundMin = new Vector3((float)(ParticleBounds.Location.X - ParticleBounds.SizeX / 2), (float)(ParticleBounds.Location.Y - ParticleBounds.SizeY / 2), (float)(ParticleBounds.Location.Z - ParticleBounds.SizeZ / 2));
#elif AVALONIA
            c.EmitterLocation = EmitterLocation;
            c.ConsumerLocation = ConsumerLocation;
            c.InitAcceleration = Acceleration;
            c.DomainBoundMax = ParticleBounds.Maximum;
            c.DomainBoundMin = ParticleBounds.Minimum;
#else
#error Unknown framework
#endif
        }
    }
}
