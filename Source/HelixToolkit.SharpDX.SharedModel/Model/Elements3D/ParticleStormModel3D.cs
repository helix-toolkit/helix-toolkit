using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene;
using HelixToolkit.SharpDX.Utilities;
using static HelixToolkit.SharpDX.Core.ParticleRenderCore;
using SharpDX;
using SharpDX.Direct3D11;

#if WINUI
using HelixToolkit.WinUI.SharpDX.Model;
#else
using HelixToolkit.Wpf.SharpDX.Model;
using System.Windows;
using Rect3D = System.Windows.Media.Media3D.Rect3D;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

public class ParticleStormModel3D : Element3D
{
    #region Dependency Properties
    public static readonly DependencyProperty ParticleCountProperty = DependencyProperty.Register("ParticleCount", typeof(int), typeof(ParticleStormModel3D),
        new PropertyMetadata(DefaultParticleCount,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.ParticleCount = Math.Max(8, (int)e.NewValue);
                }
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

    public static readonly DependencyProperty EmitterLocationProperty = DependencyProperty.Register("EmitterLocation", typeof(Point3D), typeof(ParticleStormModel3D),
        new PropertyMetadata(DefaultEmitterLocation.ToPoint3D(),
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.EmitterLocation = (((Point3D)e.NewValue).ToVector3());
                }
            }
            ));

    public Point3D EmitterLocation
    {
        set
        {
            SetValue(EmitterLocationProperty, value);
        }
        get
        {
            return (Point3D)GetValue(EmitterLocationProperty);
        }
    }

    public static readonly DependencyProperty ConsumerLocationProperty = DependencyProperty.Register("ConsumerLocation", typeof(Point3D), typeof(ParticleStormModel3D),
        new PropertyMetadata(DefaultConsumerLocation.ToPoint3D(),
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.ConsumerLocation = (((Point3D)e.NewValue).ToVector3());
                }
            }
            ));

    public Point3D ConsumerLocation
    {
        set
        {
            SetValue(ConsumerLocationProperty, value);
        }
        get
        {
            return (Point3D)GetValue(ConsumerLocationProperty);
        }
    }

#if WINUI
    public static readonly DependencyProperty ParticleBoundsProperty = DependencyProperty.Register("ParticleBounds", typeof(BoundingBox), typeof(ParticleStormModel3D),
        new PropertyMetadata(new BoundingBox(new Vector3(-50, -50, -50), new Vector3(50, 50, 50)),
        (d, e) =>
        {
            if (d is Element3DCore { SceneNode: ParticleStormNode node })
            {
                var bound = (BoundingBox)e.NewValue;
                node.DomainBoundMax = bound.Maximum;
                node.DomainBoundMin = bound.Minimum;
            }
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
    public static readonly DependencyProperty ParticleBoundsProperty = DependencyProperty.Register("ParticleBounds", typeof(Rect3D), typeof(ParticleStormModel3D),
    new PropertyMetadata(new Rect3D(0, 0, 0, 100, 100, 100),
        (d, e) =>
        {
            if (d is Element3DCore { SceneNode: ParticleStormNode node })
            {
                var bound = (Rect3D)e.NewValue;
                node.DomainBoundMax = new Vector3((float)(bound.SizeX / 2 + bound.Location.X), (float)(bound.SizeY / 2 + bound.Location.Y), (float)(bound.SizeZ / 2 + bound.Location.Z));
                node.DomainBoundMin = new Vector3((float)(bound.Location.X - bound.SizeX / 2), (float)(bound.Location.Y - bound.SizeY / 2), (float)(bound.Location.Z - bound.SizeZ / 2));
            }
        }));

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
#endif

    public static readonly DependencyProperty EmitterRadiusProperty = DependencyProperty.Register("EmitterRadius", typeof(double), typeof(ParticleStormModel3D),
        new PropertyMetadata(0.0,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.EmitterRadius = (float)(double)e.NewValue;
                }
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

    public static readonly DependencyProperty ConsumerGravityProperty = DependencyProperty.Register("ConsumerGravity", typeof(double), typeof(ParticleStormModel3D),
        new PropertyMetadata(0.0,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.ConsumerGravity = ((float)(double)e.NewValue);
                }
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

    public static readonly DependencyProperty ConsumerRadiusProperty = DependencyProperty.Register("ConsumerRadius", typeof(double), typeof(ParticleStormModel3D),
        new PropertyMetadata(0.0,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.ConsumerRadius = (float)(double)e.NewValue;
                }
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

    public static readonly DependencyProperty InitialEnergyProperty = DependencyProperty.Register("InitialEnergy", typeof(double), typeof(ParticleStormModel3D),
        new PropertyMetadata(5.0,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.InitialEnergy = Math.Max(1f, (float)(double)e.NewValue);
                }
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

    public static readonly DependencyProperty EnergyDissipationRateProperty = DependencyProperty.Register("EnergyDissipationRate", typeof(double), typeof(ParticleStormModel3D),
        new PropertyMetadata(1.0,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.EnergyDissipationRate = Math.Max(1f, (float)(double)e.NewValue);
                }
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

    public static readonly DependencyProperty RandomVectorGeneratorProperty = DependencyProperty.Register("RandomVectorGenerator", typeof(IRandomVector), typeof(ParticleStormModel3D),
        new PropertyMetadata(new UniformRandomVectorGenerator(),
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.RandomVectorGenerator = (IRandomVector)e.NewValue;
                }
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

    public static readonly DependencyProperty ParticleTextureProperty = DependencyProperty.Register("ParticleTexture", typeof(TextureModel), typeof(ParticleStormModel3D),
        new PropertyMetadata(null,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.ParticleTexture = (TextureModel)e.NewValue;
                }
            }
            ));

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

    public static readonly DependencyProperty NumTextureColumnProperty = DependencyProperty.Register("NumTextureColumn", typeof(int), typeof(ParticleStormModel3D),
        new PropertyMetadata(1,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.NumTextureColumn = (uint)System.Math.Max(1, (int)e.NewValue);
                }
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

    public static readonly DependencyProperty NumTextureRowProperty = DependencyProperty.Register("NumTextureRow", typeof(int), typeof(ParticleStormModel3D),
        new PropertyMetadata(1,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.NumTextureRow = (uint)System.Math.Max(1, (int)e.NewValue);
                }
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

    public static readonly DependencyProperty ParticleSizeProperty = DependencyProperty.Register("ParticleSize", typeof(Size), typeof(ParticleStormModel3D),
        new PropertyMetadata(new Size(1, 1),
            (d, e) =>
            {
                var size = (Size)e.NewValue;
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.ParticleSize = new Vector2((float)size.Width, (float)size.Height);
                }
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


    public static readonly DependencyProperty InitialVelocityProperty = DependencyProperty.Register("InitialVelocity", typeof(double), typeof(ParticleStormModel3D),
        new PropertyMetadata(1.0,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.InitialVelocity = (float)(double)e.NewValue;
                }
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

    public static readonly DependencyProperty AccelerationProperty = DependencyProperty.Register("Acceleration", typeof(Vector3D), typeof(ParticleStormModel3D),
        new PropertyMetadata(DefaultAcceleration.ToVector3D(),
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.InitAcceleration = ((Vector3D)e.NewValue).ToVector3();
                }
            }
            ));

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

    public static readonly DependencyProperty CumulateAtBoundProperty = DependencyProperty.Register("CumulateAtBound", typeof(bool), typeof(ParticleStormModel3D),
        new PropertyMetadata(false,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.CumulateAtBound = (bool)e.NewValue;
                }
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

    public static readonly DependencyProperty BlendColorProperty = DependencyProperty.Register("BlendColor", typeof(UIColor), typeof(ParticleStormModel3D),
        new PropertyMetadata(UIColors.White,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.BlendColor = ((UIColor)e.NewValue).ToColor4();
                }
            }));

    public UIColor BlendColor
    {
        set
        {
            SetValue(BlendColorProperty, value);
        }
        get
        {
            return (UIColor)GetValue(BlendColorProperty);
        }
    }

    public static readonly DependencyProperty AnimateSpriteByEnergyBoundProperty = DependencyProperty.Register("AnimateSpriteByEnergy", typeof(bool), typeof(ParticleStormModel3D),
        new PropertyMetadata(false,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.AnimateSpriteByEnergy = (bool)e.NewValue;
                }
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
        DependencyProperty.Register("Turbulance", typeof(double), typeof(ParticleStormModel3D),
            new PropertyMetadata(0.0,
                (d, e) =>
                {
                    if (d is Element3DCore { SceneNode: ParticleStormNode node })
                    {
                        node.Turbulance = (float)(double)e.NewValue;
                    }
                }));

    public static readonly DependencyProperty BlendProperty = DependencyProperty.Register("Blend", typeof(BlendOperation), typeof(ParticleStormModel3D),
        new PropertyMetadata(BlendOperation.Add,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.Blend = (BlendOperation)e.NewValue;
                }
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

    public static readonly DependencyProperty AlphaBlendProperty = DependencyProperty.Register("AlphaBlend", typeof(BlendOperation), typeof(ParticleStormModel3D),
        new PropertyMetadata(BlendOperation.Add,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.AlphaBlend = (BlendOperation)e.NewValue;
                }
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

    public static readonly DependencyProperty SourceBlendProperty = DependencyProperty.Register("SourceBlend", typeof(BlendOption), typeof(ParticleStormModel3D),
        new PropertyMetadata(BlendOption.One,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.SourceBlend = (BlendOption)e.NewValue;
                }
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

    public static readonly DependencyProperty DestBlendProperty = DependencyProperty.Register("DestBlend", typeof(BlendOption), typeof(ParticleStormModel3D),
        new PropertyMetadata(BlendOption.One,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.DestBlend = (BlendOption)e.NewValue;
                }
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

    public static readonly DependencyProperty SourceAlphaBlendProperty = DependencyProperty.Register("SourceAlphaBlend", typeof(BlendOption), typeof(ParticleStormModel3D),
        new PropertyMetadata(BlendOption.One,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.SourceAlphaBlend = (BlendOption)e.NewValue;
                }
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

    public static readonly DependencyProperty DestAlphaBlendProperty = DependencyProperty.Register("DestAlphaBlend", typeof(BlendOption), typeof(ParticleStormModel3D),
        new PropertyMetadata(BlendOption.Zero,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.DestAlphaBlend = (BlendOption)e.NewValue;
                }
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
    public UIColor BlendFactor
    {
        get
        {
            return (UIColor)GetValue(BlendFactorProperty);
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
        DependencyProperty.Register("BlendFactor", typeof(UIColor), typeof(ParticleStormModel3D),
        new PropertyMetadata(UIColors.White,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.BlendFactor = ((UIColor)e.NewValue).ToColor4();
                }
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
        DependencyProperty.Register("SampleMask", typeof(int), typeof(ParticleStormModel3D),
            new PropertyMetadata(-1,
                (d, e) =>
                {
                    if (d is Element3DCore { SceneNode: ParticleStormNode node })
                    {
                        node.SampleMask = (int)e.NewValue;
                    }
                }));

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
        DependencyProperty.Register("Instances", typeof(IList<Matrix>), typeof(ParticleStormModel3D), new PropertyMetadata(null, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: ParticleStormNode node })
            {
                node.Instances = e.NewValue as IList<Matrix>;
            }
        }));

    /// <summary>
    /// The enable view frustum check property
    /// </summary>
    public static readonly DependencyProperty EnableViewFrustumCheckProperty =
        DependencyProperty.Register("EnableViewFrustumCheck", typeof(bool), typeof(ParticleStormModel3D), new PropertyMetadata(true,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ParticleStormNode node })
                {
                    node.EnableViewFrustumCheck = (bool)e.NewValue;
                }
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
#if WINUI
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
