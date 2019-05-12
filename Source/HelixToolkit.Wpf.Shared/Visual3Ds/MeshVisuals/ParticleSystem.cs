// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParticleSystem.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Renders a simple particle system within the limitations of WPF.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Data;
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
        /// <summary>
        /// Identifies the <see cref="Texture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextureProperty = DependencyPropertyEx.Register<Brush, ParticleSystem>("Texture", null, (s, e) => s.TextureChanged());


        /// <summary>
        /// Identifies the <see cref="LifeTime"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LifeTimeProperty = DependencyPropertyEx.Register<double, ParticleSystem>("LifeTime", 20d);

        /// <summary>
        /// Identifies the <see cref="FadeOutTime"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FadeOutTimeProperty = DependencyPropertyEx.Register<double, ParticleSystem>("FadeOutTime", 0.5d);

        /// <summary>
        /// Identifies the <see cref="AngularVelocity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AngularVelocityProperty = DependencyPropertyEx.Register<double, ParticleSystem>("AngularVelocity", 20d);

        /// <summary>
        /// Identifies the <see cref="SizeRate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SizeRateProperty = DependencyPropertyEx.Register<double, ParticleSystem>("SizeRate", 2d);

        /// <summary>
        /// Identifies the <see cref="VelocityDamping"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VelocityDampingProperty = DependencyPropertyEx.Register<double, ParticleSystem>("VelocityDamping", 1d);

        /// <summary>
        /// Identifies the <see cref="Acceleration"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AccelerationProperty = DependencyPropertyEx.Register<double, ParticleSystem>("Acceleration", 4d);

        /// <summary>
        /// Identifies the <see cref="AccelerationDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AccelerationDirectionProperty = DependencyPropertyEx.Register<Vector3D, ParticleSystem>("AccelerationDirection", new Vector3D(3, 0, 1));

        /// <summary>
        /// Identifies the <see cref="AccelerationSpreading"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AccelerationSpreadingProperty = DependencyPropertyEx.Register<double, ParticleSystem>("AccelerationSpreading", 10d);


        /// <summary>
        /// Identifies the <see cref="EmitRate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EmitRateProperty = DependencyPropertyEx.Register<double, ParticleSystem>("EmitRate", 40d);

        /// <summary>
        /// Identifies the <see cref="Position"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PositionProperty = DependencyPropertyEx.Register<Point3D, ParticleSystem>("Position", new Point3D(0, 0, 0));

        /// <summary>
        /// Identifies the <see cref="StartRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StartRadiusProperty = DependencyPropertyEx.Register<double, ParticleSystem>("StartRadius", 1d);

        /// <summary>
        /// Identifies the <see cref="StartSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StartSizeProperty = DependencyPropertyEx.Register<double, ParticleSystem>("StartSize", 0.5d);

        /// <summary>
        /// Identifies the <see cref="StartDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StartDirectionProperty = DependencyPropertyEx.Register<Vector3D, ParticleSystem>("StartDirection", new Vector3D(0, 0, 1));

        /// <summary>
        /// Identifies the <see cref="StartVelocity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StartVelocityProperty = DependencyPropertyEx.Register<double, ParticleSystem>("StartVelocity", 2d);

        /// <summary>
        /// Identifies the <see cref="StartVelocityRandomness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StartVelocityRandomnessProperty = DependencyPropertyEx.Register<double, ParticleSystem>("StartVelocityRandomness", 1d);

        /// <summary>
        /// Identifies the <see cref="StartSpreading"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StartSpreadingProperty = DependencyPropertyEx.Register<double, ParticleSystem>("StartSpreading", 40d);

        /// <summary>
        /// Identifies the <see cref="AliveParticles"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AliveParticlesProperty = DependencyPropertyEx.Register<int, ParticleSystem>("AliveParticles", 0);

        /// <summary>
        /// The degrees to radians conversion factor.
        /// </summary>
        private const double DegToRad = Math.PI / 180;

        /// <summary>
        /// Two PI
        /// </summary>
        private const double TwoPi = Math.PI * 2;

        /// <summary>
        /// The random number generator.
        /// </summary>
        private static readonly Random r = new Random();

        /// <summary>
        /// The number of opacity levels
        /// </summary>
        private readonly int opacityLevels = 10;

        /// <summary>
        /// The stopwatch that measures the time.
        /// </summary>
        private readonly Stopwatch watch = Stopwatch.StartNew();

        /// <summary>
        /// The mesh containing the particles quads.
        /// </summary>
        private readonly MeshGeometry3D mesh;

        /// <summary>
        /// The model containing the particle mesh
        /// </summary>
        private readonly GeometryModel3D model;

        /// <summary>
        /// The particles
        /// </summary>
        private readonly List<Particle> particles = new List<Particle>(1000);

        /// <summary>
        /// The accumulated number of particles to emit. A particle is emitted when this number is greater than 1.
        /// </summary>
        private double particlesToEmit;

        /// <summary>
        /// The previous time.
        /// </summary>
        private double previousTime = double.NaN;

        /// <summary>
        /// The camera.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the number of alive particles.
        /// </summary>
        /// <value>
        /// The alive particles.
        /// </value>
        public int AliveParticles
        {
            get { return (int)this.GetValue(AliveParticlesProperty); }
            set { this.SetValue(AliveParticlesProperty, value); }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public Point3D Position
        {
            get { return (Point3D)this.GetValue(PositionProperty); }
            set { this.SetValue(PositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the start direction.
        /// </summary>
        /// <value>
        /// The start direction.
        /// </value>
        public Vector3D StartDirection
        {
            get { return (Vector3D)this.GetValue(StartDirectionProperty); }
            set { this.SetValue(StartDirectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the acceleration.
        /// </summary>
        /// <value>
        /// The acceleration.
        /// </value>
        public double Acceleration
        {
            get { return (double)this.GetValue(AccelerationProperty); }
            set { this.SetValue(AccelerationProperty, value); }
        }

        /// <summary>
        /// Gets or sets the acceleration direction.
        /// </summary>
        /// <value>
        /// The acceleration direction.
        /// </value>
        public Vector3D AccelerationDirection
        {
            get { return (Vector3D)this.GetValue(AccelerationDirectionProperty); }
            set { this.SetValue(AccelerationDirectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the acceleration spreading angle.
        /// </summary>
        /// <value>
        /// The acceleration spreading.
        /// </value>
        public double AccelerationSpreading
        {
            get { return (double)this.GetValue(AccelerationSpreadingProperty); }
            set { this.SetValue(AccelerationSpreadingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the start radius.
        /// </summary>
        /// <value>
        /// The start radius.
        /// </value>
        public double StartRadius
        {
            get { return (double)this.GetValue(StartRadiusProperty); }
            set { this.SetValue(StartRadiusProperty, value); }
        }

        /// <summary>
        /// Gets or sets the start size.
        /// </summary>
        /// <value>
        /// The start size.
        /// </value>
        public double StartSize
        {
            get { return (double)this.GetValue(StartSizeProperty); }
            set { this.SetValue(StartSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the start velocity.
        /// </summary>
        /// <value>
        /// The start velocity.
        /// </value>
        public double StartVelocity
        {
            get { return (double)this.GetValue(StartVelocityProperty); }
            set { this.SetValue(StartVelocityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the velocity damping factor.
        /// </summary>
        /// <value>
        /// The velocity damping.
        /// </value>
        public double VelocityDamping
        {
            get { return (double)this.GetValue(VelocityDampingProperty); }
            set { this.SetValue(VelocityDampingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the start velocity randomness.
        /// </summary>
        /// <value>
        /// The start velocity randomness.
        /// </value>
        public double StartVelocityRandomness
        {
            get { return (double)this.GetValue(StartVelocityRandomnessProperty); }
            set { this.SetValue(StartVelocityRandomnessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the start spreading.
        /// </summary>
        /// <value>
        /// The start spreading.
        /// </value>
        public double StartSpreading
        {
            get { return (double)this.GetValue(StartSpreadingProperty); }
            set { this.SetValue(StartSpreadingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the life time.
        /// </summary>
        /// <value>
        /// The life time.
        /// </value>
        public double LifeTime
        {
            get { return (double)this.GetValue(LifeTimeProperty); }
            set { this.SetValue(LifeTimeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the angular velocity.
        /// </summary>
        /// <value>
        /// The angular velocity.
        /// </value>
        public double AngularVelocity
        {
            get { return (double)this.GetValue(AngularVelocityProperty); }
            set { this.SetValue(AngularVelocityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the size rate.
        /// </summary>
        /// <value>
        /// The size rate.
        /// </value>
        public double SizeRate
        {
            get { return (double)this.GetValue(SizeRateProperty); }
            set { this.SetValue(SizeRateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the fade out time.
        /// </summary>
        /// <value>
        /// The fade out time.
        /// </value>
        public double FadeOutTime
        {
            get { return (double)this.GetValue(FadeOutTimeProperty); }
            set { this.SetValue(FadeOutTimeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the emit rate.
        /// </summary>
        /// <value>
        /// The emit rate.
        /// </value>
        public double EmitRate
        {
            get { return (double)this.GetValue(EmitRateProperty); }
            set { this.SetValue(EmitRateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the texture.
        /// </summary>
        /// <value>
        /// The texture.
        /// </value>
        public Brush Texture
        {
            get { return (Brush)this.GetValue(TextureProperty); }
            set { this.SetValue(TextureProperty, value); }
        }

        /// <summary>
        /// Updates the material when the texture changes.
        /// </summary>
        protected void TextureChanged()
        {
            var w = 256;
            var h = 256;
            var bitmap = new RenderTargetBitmap(opacityLevels * w, h, 96, 96, PixelFormats.Pbgra32);
            for (int i = 0; i < opacityLevels; i++)
            {
                var rect = new Rectangle { Opacity = 1 - ((double)i / opacityLevels), Fill = this.Texture, Width = w, Height = h };
                rect.Arrange(new Rect(w * i, 0, w, h));
                bitmap.Render(rect);
            }

            var brush = new ImageBrush(bitmap) { ViewportUnits = BrushMappingMode.Absolute };
            brush.Freeze();
            var material = new DiffuseMaterial(brush) { AmbientColor = Colors.White };
            material.Freeze();
            this.model.Material = material;
        }

        /// <summary>
        /// Called when the parent of the 3-D visual object is changed.
        /// </summary>
        /// <param name="oldParent">A value of type <see cref="T:System.Windows.DependencyObject" /> that represents the previous parent of the <see cref="T:System.Windows.Media.Media3D.Visual3D" /> object. If the <see cref="T:System.Windows.Media.Media3D.Visual3D" /> object did not have a previous parent, the value of the parameter is null.</param>
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

        /// <summary>
        /// Emits one particle.
        /// </summary>
        protected void EmitOne()
        {
            // calculate start direction with random spreading
            var startDirection = CreateRandomVector(this.StartDirection, this.StartSpreading);

            // find start position
            var position = this.Position;
            var z = this.StartDirection;
            z.Normalize();
            var x = z.FindAnyPerpendicular();
            var y = Vector3D.CrossProduct(z, x);
            if (this.StartRadius > 0)
            {
                var psi = TwoPi * r.NextDouble();
                position += this.StartRadius * ((x * Math.Cos(psi)) + (y * Math.Sin(psi)));
            }

            var speed = this.StartVelocity + (this.StartVelocityRandomness * (r.NextDouble() - 0.5));

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

        /// <summary>
        /// Updates the system.
        /// </summary>
        /// <param name="time">The time.</param>
        protected void Update(double time)
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

            var angularVelocity = this.AngularVelocity * DegToRad;
            var velocityDamping = this.VelocityDamping;
            var accelerationDirection = this.AccelerationDirection;
            accelerationDirection.Normalize();
            var acceleration = this.Acceleration;
            var accelerationSpreading = this.AccelerationSpreading;
            var sizeRate = this.SizeRate;
            var fadeOutTime = this.FadeOutTime;
            var lifeTime = this.LifeTime;
            for (int i = 0; i < this.particles.Count; i++)
            {
                var p = this.particles[i];

                p.Age += deltaTime;
                if (p.Age > lifeTime)
                {
                    this.particles.RemoveAt(i);
                    i--;
                    continue;
                }

                var a = accelerationSpreading > 0 ? CreateRandomVector(accelerationDirection, accelerationSpreading) : accelerationDirection;
                p.Position += p.Velocity * deltaTime;
                p.Rotation += angularVelocity * deltaTime;
                p.Size += sizeRate * deltaTime;
                p.Velocity = (p.Velocity * velocityDamping) + (a * acceleration * deltaTime);
            }

            var alive = this.particles.Count;

            var positions = this.mesh.Positions;
            var textureCoordinates = this.mesh.TextureCoordinates;
            var triangleIndices = this.mesh.TriangleIndices;
            this.mesh.Positions = null;
            this.mesh.TextureCoordinates = null;
            this.mesh.TriangleIndices = null;

            this.AliveParticles = alive;

            if (positions == null)
            {
                positions = new Point3DCollection(alive * 4);
                textureCoordinates = new PointCollection(alive * 4);
                triangleIndices = new Int32Collection(alive * 6);
            }

            if (positions.Count != alive * 4)
            {
                int previousAliveParticles = positions.Count / 4;

                // allocate positions, texture coordinates and triangle indices (to make the updating code simpler)
                AdjustListLength(positions, alive * 4);
                AdjustListLength(textureCoordinates, alive * 4);
                AdjustListLength(triangleIndices, alive * 6);

                for (int i = previousAliveParticles; i < alive; i++)
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
            var sortedParticles = this.particles.OrderBy(p => -Vector3D.DotProduct(p.Position - cameraPosition, this.camera.LookDirection));

            int j = 0;
            foreach (var p in sortedParticles)
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
                var pos = p.Position;
                positions[j4] = pos + (x * p0.X) + (y * p0.Y);
                positions[j4 + 1] = pos + (x * p1.X) + (y * p1.Y);
                positions[j4 + 2] = pos + (x * p2.X) + (y * p2.Y);
                positions[j4 + 3] = pos + (x * p3.X) + (y * p3.Y);

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

        /// <summary>
        /// Adjusts the length of the specified list.
        /// </summary>
        /// <typeparam name="T">The type of the list elements.</typeparam>
        /// <param name="list">The list to change.</param>
        /// <param name="targetLength">Target length of the list.</param>
        protected static void AdjustListLength<T>(IList<T> list, int targetLength)
        {
            int n = list.Count;
            for (int i = n - 1; i >= targetLength; i--)
            {
                list.RemoveAt(i);
            }

            for (int i = 0; i < targetLength - n; i++)
            {
                list.Add(default(T));
            }
        }

        /// <summary>
        /// Creates a random vector.
        /// </summary>
        /// <param name="z">The direction.</param>
        /// <param name="spreading">The spreading.</param>
        /// <returns>The random vector.</returns>
        protected static Vector3D CreateRandomVector(Vector3D z, double spreading)
        {
            var theta = spreading * DegToRad * r.NextDouble();
            var phi = TwoPi * r.NextDouble();

            // find basis vectors
            var x = z.FindAnyPerpendicular();
            var y = Vector3D.CrossProduct(z, x);

            var sintheta = Math.Sin(theta);

            // calculate actual direction by spherical coordinates
            return (x * sintheta * Math.Cos(phi)) + (y * sintheta * Math.Sin(phi)) + (z * Math.Cos(theta));
        }

        /// <summary>
        /// Handles the CompositionTarget.Rendering event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="System.Windows.Media.RenderingEventArgs" /> instance containing the event data.</param>
        protected override void OnCompositionTargetRendering(object sender, RenderingEventArgs eventArgs)
        {
            this.Update(this.watch.ElapsedMilliseconds * 0.001);
        }

        /// <summary>
        /// Represents a particle.
        /// </summary>
        internal class Particle
        {
            /// <summary>
            /// Gets or sets the position of the particle.
            /// </summary>
            /// <value>
            /// The position.
            /// </value>
            internal Point3D Position;

            /// <summary>
            /// Gets or sets the velocity of the particle.
            /// </summary>
            /// <value>
            /// The velocity.
            /// </value>
            internal Vector3D Velocity;

            /// <summary>
            /// Gets or sets the 2D rotation of the rendered particle texture.
            /// </summary>
            /// <value>
            /// The rotation.
            /// </value>
            internal double Rotation;

            /// <summary>
            /// Gets or sets the size of the particle.
            /// </summary>
            /// <value>
            /// The size.
            /// </value>
            internal double Size;

            /// <summary>
            /// Gets or sets the age of the particle.
            /// </summary>
            /// <value>
            /// The age.
            /// </value>
            internal double Age;
        }
    }
}