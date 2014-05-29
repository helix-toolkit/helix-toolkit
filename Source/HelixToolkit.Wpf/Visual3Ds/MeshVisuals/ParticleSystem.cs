namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;
    using System.Windows.Shapes;

    /// <summary>
    /// Renders a simple particle system within the limitations of WPF.
    /// </summary>
    public class ParticleSystem : RenderingModelVisual3D
    {
        public static readonly DependencyProperty TextureProperty = DependencyPropertyEx.Register<Brush, ParticleSystem>("Texture", null, (s, e) => s.TextureChanged());

        public static readonly DependencyProperty LifeTimeProperty = DependencyPropertyEx.Register<double, ParticleSystem>("LifeTime", 20d);
        public static readonly DependencyProperty FadeOutTimeProperty = DependencyPropertyEx.Register<double, ParticleSystem>("FadeOutTime", 0.5d);

        public static readonly DependencyProperty AngularVelocityProperty = DependencyPropertyEx.Register<double, ParticleSystem>("AngularVelocity", 20d);
        public static readonly DependencyProperty SizeRateProperty = DependencyPropertyEx.Register<double, ParticleSystem>("SizeRate", 2d);
        public static readonly DependencyProperty VelocityDampingProperty = DependencyPropertyEx.Register<double, ParticleSystem>("VelocityDamping", 1d);
        public static readonly DependencyProperty AccelerationProperty = DependencyPropertyEx.Register<double, ParticleSystem>("Acceleration", 4d);
        public static readonly DependencyProperty AccelerationDirectionProperty = DependencyPropertyEx.Register<Vector3D, ParticleSystem>("AccelerationDirection", new Vector3D(3, 0, 1));
        public static readonly DependencyProperty AccelerationSpreadingProperty = DependencyPropertyEx.Register<double, ParticleSystem>("AccelerationSpreading", 10d);

        public static readonly DependencyProperty EmitRateProperty = DependencyPropertyEx.Register<double, ParticleSystem>("EmitRate", 40d);
        public static readonly DependencyProperty PositionProperty = DependencyPropertyEx.Register<Point3D, ParticleSystem>("Position", new Point3D(0, 0, 0));
        public static readonly DependencyProperty StartRadiusProperty = DependencyPropertyEx.Register<double, ParticleSystem>("StartRadius", 1d);
        public static readonly DependencyProperty StartSizeProperty = DependencyPropertyEx.Register<double, ParticleSystem>("StartSize", 0.5d);
        public static readonly DependencyProperty StartDirectionProperty = DependencyPropertyEx.Register<Vector3D, ParticleSystem>("StartDirection", new Vector3D(0, 0, 1));
        public static readonly DependencyProperty StartVelocityProperty = DependencyPropertyEx.Register<double, ParticleSystem>("StartVelocity", 2d);
        public static readonly DependencyProperty StartVelocityRandomnessProperty = DependencyPropertyEx.Register<double, ParticleSystem>("StartVelocityRandomness", 1d);
        public static readonly DependencyProperty StartSpreadingProperty = DependencyPropertyEx.Register<double, ParticleSystem>("StartSpreading", 40d);
        public static readonly DependencyProperty AliveParticlesProperty = DependencyPropertyEx.Register<int, ParticleSystem>("AliveParticles", 0);

        private readonly Stopwatch watch = Stopwatch.StartNew();
        private readonly int opacityLevels = 10;
        private readonly MeshGeometry3D mesh;
        private readonly GeometryModel3D model;
        private readonly List<Particle> particles = new List<Particle>(1000);
        private readonly Random r = new Random();

        private double particlesToEmit;

        private double previousTime = double.NaN;

        private ProjectionCamera camera;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParticleSystem"/> class.
        /// </summary>
        public ParticleSystem()
        {
            this.mesh = new MeshGeometry3D();
            this.model = new GeometryModel3D { Geometry = this.mesh };
            this.Content = this.model;
            this.EmitOne();
        }

        public int AliveParticles
        {
            get { return (int)this.GetValue(AliveParticlesProperty); }
            set { this.SetValue(AliveParticlesProperty, value); }
        }

        public Point3D Position
        {
            get { return (Point3D)this.GetValue(PositionProperty); }
            set { this.SetValue(PositionProperty, value); }
        }

        public Vector3D StartDirection
        {
            get { return (Vector3D)this.GetValue(StartDirectionProperty); }
            set { this.SetValue(StartDirectionProperty, value); }
        }

        public double Acceleration
        {
            get { return (double)this.GetValue(AccelerationProperty); }
            set { this.SetValue(AccelerationProperty, value); }
        }

        public Vector3D AccelerationDirection
        {
            get { return (Vector3D)this.GetValue(AccelerationDirectionProperty); }
            set { this.SetValue(AccelerationDirectionProperty, value); }
        }

        public double AccelerationSpreading
        {
            get { return (double)this.GetValue(AccelerationSpreadingProperty); }
            set { this.SetValue(AccelerationSpreadingProperty, value); }
        }

        public double StartRadius
        {
            get { return (double)this.GetValue(StartRadiusProperty); }
            set { this.SetValue(StartRadiusProperty, value); }
        }

        public double StartSize
        {
            get { return (double)this.GetValue(StartSizeProperty); }
            set { this.SetValue(StartSizeProperty, value); }
        }

        public double StartVelocity
        {
            get { return (double)this.GetValue(StartVelocityProperty); }
            set { this.SetValue(StartVelocityProperty, value); }
        }

        public double VelocityDamping
        {
            get { return (double)this.GetValue(VelocityDampingProperty); }
            set { this.SetValue(VelocityDampingProperty, value); }
        }

        public double StartVelocityRandomness
        {
            get { return (double)this.GetValue(StartVelocityRandomnessProperty); }
            set { this.SetValue(StartVelocityRandomnessProperty, value); }
        }

        public double StartSpreading
        {
            get { return (double)this.GetValue(StartSpreadingProperty); }
            set { this.SetValue(StartSpreadingProperty, value); }
        }

        public double LifeTime
        {
            get { return (double)this.GetValue(LifeTimeProperty); }
            set { this.SetValue(LifeTimeProperty, value); }
        }

        public double AngularVelocity
        {
            get { return (double)this.GetValue(AngularVelocityProperty); }
            set { this.SetValue(AngularVelocityProperty, value); }
        }

        public double SizeRate
        {
            get { return (double)this.GetValue(SizeRateProperty); }
            set { this.SetValue(SizeRateProperty, value); }
        }

        public double FadeOutTime
        {
            get { return (double)this.GetValue(FadeOutTimeProperty); }
            set { this.SetValue(FadeOutTimeProperty, value); }
        }

        public double EmitRate
        {
            get { return (double)this.GetValue(EmitRateProperty); }
            set { this.SetValue(EmitRateProperty, value); }
        }

        public Brush Texture
        {
            get { return (Brush)this.GetValue(TextureProperty); }
            set { this.SetValue(TextureProperty, value); }
        }

        private void TextureChanged()
        {
            var w = 256;
            var h = 256;
            var bitmap = new RenderTargetBitmap(this.opacityLevels * w, h, 96, 96, PixelFormats.Pbgra32);
            for (int i = 0; i < this.opacityLevels; i++)
            {
                var rect = new Rectangle { Opacity = 1 - (double)i / this.opacityLevels, Fill = this.Texture, Width = w, Height = h };
                rect.Arrange(new Rect(w * i, 0, w, h));
                bitmap.Render(rect);
            }

            var brush = new ImageBrush(bitmap) { ViewportUnits = BrushMappingMode.Absolute };
            brush.Freeze();
            var material = new DiffuseMaterial(brush) { AmbientColor = Colors.White };
            material.Freeze();
            this.model.Material = material;
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            if (oldParent == null)
            {
                this.SubscribeToRenderingEvent();
            }
            else
            {
                this.UnsubscribeRenderingEvent();
            }
        }

        public void EmitOne()
        {
            // calculate start direction with random spreading
            var startDirection = this.CreateRandomVector(this.StartDirection, this.StartSpreading);

            // find start position
            var position = this.Position;
            var z = this.StartDirection;
            z.Normalize();
            var x = z.FindAnyPerpendicular();
            var y = Vector3D.CrossProduct(z, x);
            if (this.StartRadius > 0)
            {
                var psi = Math.PI * 2 * this.r.NextDouble();
                position += this.StartRadius * ((x * Math.Cos(psi)) + (y * Math.Sin(psi)));
            }

            var speed = this.StartVelocity + (this.StartVelocityRandomness * (this.r.NextDouble() - 0.5));

            var particle = new Particle
                        {
                            Position = position,
                            Size = this.StartSize,
                            Age = 0,
                            Rotation = 0,
                            Velocity = speed * startDirection
                        };

            // Replace or add to particle list
            int index = this.particles.FindIndex(p => p == null);
            if (index >= 0)
            {
                this.particles[index] = particle;
            }
            else
            {
                this.particles.Add(particle);
            }
        }

        public void Update(double time)
        {
            if (double.IsNaN(this.previousTime))
            {
                this.previousTime = time;
                return;
            }

            var deltaTime = time - this.previousTime;
            this.previousTime = time;

            this.particlesToEmit += deltaTime * this.EmitRate;
            while (this.particlesToEmit > 1)
            {
                this.EmitOne();
                this.particlesToEmit--;
            }

            var angularVelocity = this.AngularVelocity / 180 * Math.PI;
            var velocityDamping = this.VelocityDamping;
            var accelerationDirection = this.AccelerationDirection;
            accelerationDirection.Normalize();
            var acceleration = this.Acceleration;
            var accelerationSpreading = this.AccelerationSpreading;
            var sizeRate = this.SizeRate;
            int alive = 0;
            for (int i = 0; i < this.particles.Count; i++)
            {
                var p = this.particles[i];
                if (p == null)
                {
                    continue;
                }

                p.Age += deltaTime;
                if (p.Age > this.LifeTime)
                {
                    this.particles[i] = null;
                    continue;
                }

                p.Position += p.Velocity * deltaTime;
                p.Rotation += angularVelocity * deltaTime;
                p.Size += sizeRate * deltaTime;
                var a = accelerationSpreading > 0 ? this.CreateRandomVector(accelerationDirection, accelerationSpreading) : accelerationDirection;
                p.Velocity = (p.Velocity * velocityDamping) + (a * acceleration * deltaTime);
                alive++;
            }

            var positions = this.mesh.Positions;
            var textureCoordinates = this.mesh.TextureCoordinates;
            var triangleIndices = this.mesh.TriangleIndices;
            this.mesh.Positions = null;
            this.mesh.TextureCoordinates = null;
            this.mesh.TriangleIndices = null;

            this.AliveParticles = alive;

            if (positions == null || positions.Count != alive)
            {
                Debug.WriteLine("Adjust to " + alive);
                positions = new Point3DCollection(alive * 4);
                textureCoordinates = new PointCollection(alive * 4);
                triangleIndices = new Int32Collection(alive * 6);

                // allocate positions, texture coordinates and triangle indices (to make the updating code simpler)
                while (positions.Count < alive * 4)
                {
                    positions.Add(new Point3D());
                }

                while (textureCoordinates.Count < alive * 4)
                {
                    textureCoordinates.Add(new Point());
                }

                while (triangleIndices.Count < alive * 6)
                {
                    triangleIndices.Add(0);
                }

                for (int i = 0; i < alive; i++)
                {
                    var i4 = i * 4;
                    var i6 = i * 6;

                    triangleIndices[i6] = i4;
                    triangleIndices[i6 + 1] = i4 + 1;
                    triangleIndices[i6 + 2] = i4 + 2;
                    triangleIndices[i6 + 3] = i4 + 2;
                    triangleIndices[i6 + 4] = i4 + 3;
                    triangleIndices[i6 + 5] = i4;
                }
            }

            if (this.camera == null)
            {
                var viewport = this.GetViewport3D();
                this.camera = (ProjectionCamera)viewport.Camera;
            }

            // calculate coordinate system for particle planes
            // all particles are rendered on parallel planes to avoid transparency conflicts
            var cameraPosition = this.camera.Position;
            var upVector = this.camera.UpDirection;
            var z = this.camera.LookDirection;

            var x = Vector3D.CrossProduct(z, upVector);
            var y = Vector3D.CrossProduct(x, z);
            x.Normalize();
            y.Normalize();

            // find alive particles and sort by distance from camera (projected to look direction, nearest particles last)
            var aliveParticles = this.particles.Where(p => p != null).OrderBy(p => -Vector3D.DotProduct(p.Position - cameraPosition, this.camera.LookDirection));
            var fadeOutTime = this.FadeOutTime;
            var lifeTime = this.LifeTime;

            int j = 0;
            foreach (var p in aliveParticles)
            {
                var halfSize = p.Size * 0.5;
                var j4 = j * 4;
                j++;

                var cos = Math.Cos(p.Rotation);
                var sin = Math.Sin(p.Rotation);

                var p0 = new Point(halfSize * (cos + sin), halfSize * (sin - cos));
                var p1 = new Point(halfSize * (cos - sin), halfSize * (cos + sin));
                var p2 = new Point(-halfSize * (cos + sin), halfSize * (cos - sin));
                var p3 = new Point(halfSize * (sin - cos), -halfSize * (cos + sin));

                positions[j4 + 0] = p.Position + (x * p0.X) + (y * p0.Y);
                positions[j4 + 1] = p.Position + (x * p1.X) + (y * p1.Y);
                positions[j4 + 2] = p.Position + (x * p2.X) + (y * p2.Y);
                positions[j4 + 3] = p.Position + (x * p3.X) + (y * p3.Y);

                var opacity = 1d;
                if (fadeOutTime < 1 && p.Age > lifeTime * fadeOutTime)
                {
                    opacity = 1 - (((p.Age / lifeTime) - fadeOutTime) / (1 - fadeOutTime));
                }

                // update the texture coordinates with the current opacity of the particle
                int transparency = (int)((1 - opacity) * this.opacityLevels);
                var u0 = (double)transparency / this.opacityLevels;
                var u1 = (transparency + 1d) / this.opacityLevels;
                textureCoordinates[j4] = new Point(u1, 1);
                textureCoordinates[j4 + 1] = new Point(u1, 0);
                textureCoordinates[j4 + 2] = new Point(u0, 0);
                textureCoordinates[j4 + 3] = new Point(u0, 1);
            }

            this.mesh.Positions = positions;
            this.mesh.TextureCoordinates = textureCoordinates;
            this.mesh.TriangleIndices = triangleIndices;
        }

        private Vector3D CreateRandomVector(Vector3D dir, double spreading)
        {
            var theta = spreading / 180 * Math.PI * this.r.NextDouble();
            var phi = Math.PI * 2 * this.r.NextDouble();

            // find basis vectors
            var z = dir;
            var x = z.FindAnyPerpendicular();
            var y = Vector3D.CrossProduct(z, x);

            // calculate actual direction by spherical coordinates
            return (x * Math.Sin(theta) * Math.Cos(phi)) + (y * Math.Sin(theta) * Math.Sin(phi)) + (z * Math.Cos(theta));
        }

        protected override void OnCompositionTargetRendering(object sender, RenderingEventArgs eventArgs)
        {
            this.Update(this.watch.ElapsedMilliseconds * 0.001);
        }

        /// <summary>
        /// Represents a particle.
        /// </summary>
        public class Particle
        {
            /// <summary>
            /// Gets or sets the position of the particle.
            /// </summary>
            /// <value>
            /// The position.
            /// </value>
            public Point3D Position { get; set; }

            /// <summary>
            /// Gets or sets the velocity of the particle.
            /// </summary>
            /// <value>
            /// The velocity.
            /// </value>
            public Vector3D Velocity { get; set; }

            /// <summary>
            /// Gets or sets the 2D rotation of the rendered particle texture.
            /// </summary>
            /// <value>
            /// The rotation.
            /// </value>
            public double Rotation { get; set; }

            /// <summary>
            /// Gets or sets the size of the particle.
            /// </summary>
            /// <value>
            /// The size.
            /// </value>
            public double Size { get; set; }

            /// <summary>
            /// Gets or sets the age of the particle.
            /// </summary>
            /// <value>
            /// The age.
            /// </value>
            public double Age { get; set; }
        }
    }
}