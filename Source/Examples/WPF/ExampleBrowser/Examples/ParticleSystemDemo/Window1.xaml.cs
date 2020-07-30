// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Window1.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Interaction logic for Window1.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ParticleSystemDemo
{
    using System.Windows;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    using PropertyTools;
    using PropertyTools.DataAnnotations;

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    [Example("ParticleSystemDemo", "A simple particle system")]
    public partial class Window1 : Window
    {
        public Window1()
        {
            this.InitializeComponent();
            this.DataContext = new MainViewModel();
        }
    }

    public class MainViewModel : Observable
    {
        private double lifeTime;

        private double fadeOutTime;
        private double emitRate;

        private Point3D position;

        private Vector3D startDirection;

        private double startSpreading;

        private double startVelocity;

        private double startVelocityRandomness;

        private double startRadius;

        private double velocityDamping;
        private double angularVelocity;
        private double sizeRate;
        private double acceleration;
        private double accelerationSpreading;
        private double startSize;

        private Vector3D accelerationDirection;

        public MainViewModel()
        {
            this.lifeTime = 5;
            this.fadeOutTime = 0;

            this.position = new Point3D(0, 0, 0);
            this.emitRate = 100;
            this.startRadius = 0;
            this.startDirection = new Vector3D(0, 0, 1);
            this.startSpreading = 10;
            this.startVelocity = 4;
            this.startVelocityRandomness = 2;
            this.startSize = 0.5;

            this.velocityDamping = 0.999;
            this.angularVelocity = 10;
            this.sizeRate = 1;
            this.accelerationDirection = new Vector3D(3, 0, -1);
            this.acceleration = 4;
            this.accelerationSpreading = 10;
        }

        [Category("Properties|Life")]
        [Slidable(1, 100, 1, 10)]
        [FormatString("0.0")]
        public double LifeTime
        {
            get
            {
                return this.lifeTime;
            }

            set
            {
                this.SetValue(ref this.lifeTime, value, nameof(this.LifeTime));
            }
        }

        [Category("Properties|Life")]
        [DisplayName("Fade-out time")]
        [Description("The relative lifetime (0-1) when the particles start fading out")]
        [Slidable(0, 1, 0.01, 0.1)]
        [FormatString("0.00")]
        public double FadeOutTime
        {
            get
            {
                return this.fadeOutTime;
            }

            set
            {
                this.SetValue(ref this.fadeOutTime, value, nameof(this.FadeOutTime));
            }
        }

        [Category("Properties|Change")]
        [DisplayName("Velocity damping")]
        [Description("The velocity damping factor")]
        [Slidable(0.9, 1, 0.001, 0.01)]
        [FormatString("0.0000")]
        public double VelocityDamping
        {
            get
            {
                return this.velocityDamping;
            }

            set
            {
                this.SetValue(ref this.velocityDamping, value, nameof(this.VelocityDamping));
            }
        }

        [Category("Properties|Change")]
        [DisplayName("Angular velocity")]
        [Description("The angular velocity (degrees/second)")]
        [Slidable(0, 100, 1, 10)]
        [FormatString("0.0")]
        public double AngularVelocity
        {
            get
            {
                return this.angularVelocity;
            }

            set
            {
                this.SetValue(ref this.angularVelocity, value, nameof(this.AngularVelocity));
            }
        }

        [Category("Properties|Change")]
        [DisplayName("Size change rate")]
        [Description("The size change rate (per second)")]
        [Slidable(0, 5, 0.1, 1)]
        [FormatString("0.0")]
        public double SizeRate
        {
            get
            {
                return this.sizeRate;
            }

            set
            {
                this.SetValue(ref this.sizeRate, value, nameof(this.SizeRate));
            }
        }

        [Category("Properties|Change")]
        [DisplayName("Acceleration direction")]
        [Description("The acceleration direction")]
        [FormatString("0.0")]
        public Vector3D AccelerationDirection
        {
            get
            {
                return this.accelerationDirection;
            }

            set
            {
                this.SetValue(ref this.accelerationDirection, value, nameof(this.AccelerationDirection));
            }
        }

        [Category("Properties|Change")]
        [DisplayName("Acceleration")]
        [Description("The acceleration magnitude")]
        [Slidable(0, 20, 1, 10)]
        [FormatString("0.0")]
        public double Acceleration
        {
            get
            {
                return this.acceleration;
            }

            set
            {
                this.SetValue(ref this.acceleration, value, nameof(this.Acceleration));
            }
        }

        [Category("Properties|Change")]
        [DisplayName("Acceleration spreading")]
        [Description("The spreading (degrees) - this does not seem to work well")]
        [Slidable(0, 180, 1, 10)]
        [FormatString("0.0")]
        public double AccelerationSpreading
        {
            get
            {
                return this.accelerationSpreading;
            }

            set
            {
                this.SetValue(ref this.accelerationSpreading, value, nameof(this.AccelerationSpreading));
            }
        }


        [Category("Properties|Emission")]
        [DisplayName("Rate")]
        [Description("The number of particles emitted per second")]
        [Slidable(0.1, 400, 1, 10)]
        [FormatString("0.0")]
        public double EmitRate
        {
            get
            {
                return this.emitRate;
            }

            set
            {
                this.SetValue(ref this.emitRate, value, nameof(this.EmitRate));
            }
        }

        [Category("Properties|Emission")]
        [DisplayName("Position")]
        [Description("The position of the emitter")]
        public Point3D Position
        {
            get
            {
                return this.position;
            }

            set
            {
                this.SetValue(ref this.position, value, nameof(this.Position));
            }
        }


        [Category("Properties|Emission")]
        [DisplayName("Start radius")]
        [Slidable(0, 1, 0.1, 1)]
        [Description("The radius of the emitter")]
        [FormatString("0.0")]
        public double StartRadius
        {
            get
            {
                return this.startRadius;
            }

            set
            {
                this.SetValue(ref this.startRadius, value, nameof(this.StartRadius));
            }
        }

        [Category("Properties|Emission")]
        [DisplayName("Start size")]
        [Slidable(0, 4, 0.1, 1)]
        [Description("The start size of the particles")]
        [FormatString("0.0")]
        public double StartSize
        {
            get
            {
                return this.startSize;
            }

            set
            {
                this.SetValue(ref this.startSize, value, nameof(this.StartSize));
            }
        }

        [Category("Properties|Emission")]
        [DisplayName("Start direction")]
        [Description("The start direction of the particles")]
        public Vector3D StartDirection
        {
            get
            {
                return this.startDirection;
            }

            set
            {
                this.SetValue(ref this.startDirection, value, nameof(this.StartDirection));
            }
        }

        [Category("Properties|Emission")]
        [DisplayName("Start spreading")]
        [Slidable(0, 90, 1, 10)]
        [Description("The start spreading (degrees) of the particles")]
        [FormatString("0.0")]
        public double StartSpreading
        {
            get
            {
                return this.startSpreading;
            }

            set
            {
                this.SetValue(ref this.startSpreading, value, nameof(StartSpreading));
            }
        }

        [Category("Properties|Emission")]
        [DisplayName("Start velocity")]
        [Slidable(0, 100, 1, 10)]
        [Description("The start velocity of the particles")]
        [FormatString("0.0")]
        public double StartVelocity
        {
            get
            {
                return this.startVelocity;
            }

            set
            {
                this.SetValue(ref this.startVelocity, value, nameof(this.StartVelocity));
            }
        }

        [Category("Properties|Emission")]
        [DisplayName("Start velocity randomness")]
        [Description("The start velocity randomness of the particles")]
        [FormatString("0.0")]
        public double StartVelocityRandomness
        {
            get
            {
                return this.startVelocityRandomness;
            }

            set
            {
                this.SetValue(ref this.startVelocityRandomness, value, nameof(this.StartVelocityRandomness));
            }
        }
    }
}