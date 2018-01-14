using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using SharpDX;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Media = System.Windows.Media;
using Media3D = System.Windows.Media.Media3D;

namespace ParticleSystemDemo
{
    public class MainViewModel : BaseViewModel
    {
        public MeshGeometry3D Model { private set; get; }

        private Media3D.Transform3D emitterTransform = new Media3D.MatrixTransform3D(new Media3D.Matrix3D(0.5, 0, 0, 0, 0, 0.5, 0, 0, 0, 0, 0.5, 0, 0, -4, 0, 1));
        public Media3D.Transform3D EmitterTransform
        {
            get
            {
                return emitterTransform;
            }
            set
            {
                SetValue(ref emitterTransform, value);
                EmitterLocation = new Media3D.Point3D(value.Value.OffsetX, value.Value.OffsetY, value.Value.OffsetZ);
            }
        }

        private double emitterRadius = 0.5;
        public double EmitterRadius
        {
            set
            {
                if (SetValue(ref emitterRadius, value))
                {
                    var matrix = EmitterTransform.Value;
                    matrix.M11 = matrix.M22 = matrix.M33 = value;
                    EmitterTransform = new Media3D.MatrixTransform3D(matrix);
                }
            }
            get
            {
                return emitterRadius;
            }
        }

        private Media3D.Point3D emitterLocation = new Media3D.Point3D(0, -4, 0);
        public Media3D.Point3D EmitterLocation
        {
            set
            {
                SetValue(ref emitterLocation, value);
            }
            get
            {
                return emitterLocation;
            }
        }


        private Media3D.Transform3D consumerTransform = new Media3D.MatrixTransform3D(new Media3D.Matrix3D(0.5, 0, 0, 0, 0, 0.5, 0, 0, 0, 0, 0.5, 0, 0, 4, 0, 1));
        public Media3D.Transform3D ConsumerTransform
        {
            get
            {
                return consumerTransform;
            }
            set
            {
                SetValue(ref consumerTransform, value);
                ConsumerLocation = new Media3D.Point3D(value.Value.OffsetX, value.Value.OffsetY, value.Value.OffsetZ);
            }
        }

        private Media3D.Point3D consumerLocation = new Media3D.Point3D(0, 4, 0);
        public Media3D.Point3D ConsumerLocation
        {
            set
            {
                SetValue(ref consumerLocation, value);
            }
            get
            {
                return consumerLocation;
            }
        }

        private double consumerRadius = 0.5;
        public double ConsumerRadius
        {
            set
            {
                if (SetValue(ref consumerRadius, value))
                {
                    var matrix = ConsumerTransform.Value;
                    matrix.M11 = matrix.M22 = matrix.M33 = value;
                    ConsumerTransform = new Media3D.MatrixTransform3D(matrix);
                }
            }
            get
            {
                return consumerRadius;
            }
        }

        public Material EmitterMaterial { get; } = new PhongMaterial() { DiffuseColor = new SharpDX.Color4(1, 0, 1, 1) };

        public Material ConsumerMaterial { get; } = new PhongMaterial() { DiffuseColor = new SharpDX.Color4(0.5f, 1f, 0.5f, 1) };

        private Stream particleTexture;
        public Stream ParticleTexture
        {
            set
            {
                SetValue(ref particleTexture, value);
            }
            get
            {
                return particleTexture;
            }
        }

        private Media3D.Vector3D acceleration = new Media3D.Vector3D(0, 1, 0);
        public Media3D.Vector3D Acceleration
        {
            set
            {
                SetValue(ref acceleration, value);
            }
            get
            {
                return acceleration;
            }
        }

        private int accelerationX = 0;
        public int AccelerationX
        {
            set
            {
                if (SetValue(ref accelerationX, value))
                {
                    UpdateAcceleration();
                }
            }
            get
            {
                return accelerationX;
            }
        }

        private Size particlesize = new Size(0.1, 0.1);
        public Size ParticleSize
        {
            set
            {
                SetValue(ref particlesize, value);
            }
            get
            {
                return particlesize;
            }
        }

        private int sizeSlider = 10;
        public int SizeSlider
        {
            set
            {
                if (SetValue(ref sizeSlider, value))
                {
                    ParticleSize = new Size(((double)value) / 100, ((double)value) / 100);
                }
            }
            get
            {
                return sizeSlider;
            }
        }

        private int accelerationY = 100;
        public int AccelerationY
        {
            set
            {
                if (SetValue(ref accelerationY, value))
                {
                    UpdateAcceleration();
                }
            }
            get
            {
                return accelerationY;
            }
        }

        private int accelerationZ = 0;
        public int AccelerationZ
        {
            set
            {
                if (SetValue(ref accelerationZ, value))
                {
                    UpdateAcceleration();
                }
            }
            get
            {
                return accelerationZ;
            }
        }

        const int DefaultBoundScale = 10;
        public LineGeometry3D BoundingLines { private set; get; }

        public Media3D.ScaleTransform3D BoundingLineTransform { private set; get; } = new Media3D.ScaleTransform3D(DefaultBoundScale, DefaultBoundScale, DefaultBoundScale);

        private Media3D.Rect3D particleBounds = new Media3D.Rect3D(0, 0, 0, DefaultBoundScale, DefaultBoundScale, DefaultBoundScale);
        public Media3D.Rect3D ParticleBounds
        {
            set
            {
                SetValue(ref particleBounds, value);
            }
            get
            {
                return particleBounds;
            }
        }

        private int boundScale = DefaultBoundScale;
        public int BoundScale
        {
            set
            {
                if (SetValue(ref boundScale, value))
                {
                    ParticleBounds = new Media3D.Rect3D(0, 0, 0, value, value, value);
                    BoundingLineTransform.ScaleX = BoundingLineTransform.ScaleY = BoundingLineTransform.ScaleZ = value;
                }
            }
            get
            {
                return boundScale;
            }
        }

        private Media.Color blendColor = Media.Colors.White;
        public Media.Color BlendColor
        {
            set
            {
                if (SetValue(ref blendColor, value))
                {
                    BlendColorBrush = new Media.SolidColorBrush(value);
                }
            }
            get
            {
                return blendColor;
            }
        }

        private int redValue = 255;
        public int RedValue
        {
            set
            {
                if (SetValue(ref redValue, value))
                {
                    BlendColor = Media.Color.FromRgb((byte)RedValue, (byte)GreenValue, (byte)BlueValue);
                }
            }
            get
            {
                return redValue;
            }
        }

        private int greenValue = 255;
        public int GreenValue
        {
            set
            {
                if (SetValue(ref greenValue, value))
                {
                    BlendColor = Media.Color.FromRgb((byte)RedValue, (byte)GreenValue, (byte)BlueValue);
                }
            }
            get
            {
                return greenValue;
            }
        }

        private int blueValue = 255;
        public int BlueValue
        {
            set
            {
                if (SetValue(ref blueValue, value))
                {
                    BlendColor = Media.Color.FromRgb((byte)RedValue, (byte)GreenValue, (byte)BlueValue);
                }
            }
            get
            {
                return blueValue;
            }
        }

        private Media.SolidColorBrush blendColorBrush = new Media.SolidColorBrush(Media.Colors.White);
        public Media.SolidColorBrush BlendColorBrush
        {
            set
            {
                SetValue(ref blendColorBrush, value);
            }
            get
            {
                return blendColorBrush;
            }
        }
        private int numTextureRows;
        public int NumTextureRows
        {
            set
            {
                SetValue(ref numTextureRows, value);
            }
            get
            {
                return numTextureRows;
            }
        }

        private int numTextureColumns;
        public int NumTextureColumns
        {
            set
            {
                SetValue(ref numTextureColumns, value);
            }
            get
            {
                return numTextureColumns;
            }
        }

        private int selectedTextureIndex = 0;
        public int SelectedTextureIndex
        {
            set
            {
                if (SetValue(ref selectedTextureIndex, value))
                {
                    LoadTexture(value);
                }
            }
            get
            {
                return selectedTextureIndex;
            }
        }

        public Array BlendOperationArray { get; } = Enum.GetValues(typeof(BlendOperation));

        public Array BlendOptionArray { get; } = Enum.GetValues(typeof(BlendOption));

        private BlendOption sourceBlendOption = BlendOption.One;
        public BlendOption SourceBlendOption
        {
            set
            {
                SetValue(ref sourceBlendOption, value);
            }
            get
            {
                return sourceBlendOption;
            }
        }

        private BlendOption sourceAlphaBlendOption = BlendOption.One;
        public BlendOption SourceAlphaBlendOption
        {
            set
            {
                SetValue(ref sourceAlphaBlendOption, value);
            }
            get
            {
                return sourceAlphaBlendOption;
            }
        }

        private BlendOption destBlendOption = BlendOption.One;
        public BlendOption DestBlendOption
        {
            set
            {
                SetValue(ref destBlendOption, value);
            }
            get
            {
                return destBlendOption;
            }
        }

        private BlendOption destAlphaBlendOption = BlendOption.Zero;
        public BlendOption DestAlphaBlendOption
        {
            set
            {
                SetValue(ref destAlphaBlendOption, value);
            }
            get
            {
                return destAlphaBlendOption;
            }
        }

        public IList<Matrix> Instances { private set; get; }

        public readonly Tuple<int, int>[] TextureColumnsRows = new Tuple<int, int>[] { new Tuple<int, int>(1, 1), new Tuple<int, int>(4, 4), new Tuple<int, int>(4, 4), new Tuple<int, int>(6, 5) };
        public readonly string[] Textures = new string[] { @"Snowflake.png", @"FXT_Explosion_Fireball_Atlas_d.png", @"FXT_Sparks_01_Atlas_d.png", @"Smoke30Frames_0.png" };
        public readonly int[] DefaultParticleSizes = new int[] { 20, 90, 40, 90 };

        public Media.Color Light1Color { get; set; } = Media.Colors.White;
        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            var lineBuilder = new LineBuilder();
            lineBuilder.AddBox(new SharpDX.Vector3(), 1, 1, 1);
            BoundingLines = lineBuilder.ToLineGeometry3D();
            LoadTexture(SelectedTextureIndex);
            var meshBuilder = new MeshBuilder();
            meshBuilder.AddSphere(new SharpDX.Vector3(0, 0, 0), 0.5, 16, 16);
            Model = meshBuilder.ToMesh();
            Camera = new PerspectiveCamera() { Position = new Media3D.Point3D(0, 0, 20), UpDirection = new Media3D.Vector3D(0, 1, 0), LookDirection = new Media3D.Vector3D(0, 0, -20) };
            Instances = new Matrix[] {
                Matrix.Identity, Matrix.Scaling(1,-1, 1) * Matrix.Translation(10, 0, 10), Matrix.Translation(-10, 0, 10), Matrix.Translation(10, 0, -10),
                Matrix.RotationAxis(new Vector3(1,0,0), 90) *  Matrix.Translation(-10, 0, -10), };
        }

        private void LoadTexture(int index)
        {
            using (var file = new FileStream(new System.Uri(Textures[index], System.UriKind.RelativeOrAbsolute).ToString(), FileMode.Open))
            {
                var mem = new MemoryStream();
                file.CopyTo(mem);
                ParticleTexture = mem;
            }

            NumTextureColumns = TextureColumnsRows[index].Item1;
            NumTextureRows = TextureColumnsRows[index].Item2;
            SizeSlider = DefaultParticleSizes[index];
            switch (index)
            {
                case 3:
                    SourceBlendOption = BlendOption.SourceAlpha;
                    SourceAlphaBlendOption = BlendOption.Zero;
                    DestBlendOption = BlendOption.InverseSourceAlpha;
                    DestAlphaBlendOption = BlendOption.Zero;
                    break;
                default:
                    SourceBlendOption = BlendOption.One;
                    SourceAlphaBlendOption = BlendOption.One;
                    DestBlendOption = BlendOption.One;
                    DestAlphaBlendOption = BlendOption.Zero;
                    break;
            }
        }

        private void UpdateAcceleration()
        {
            Acceleration = new Media3D.Vector3D((double)AccelerationX / 100, (double)AccelerationY / 100, (double)AccelerationZ / 100);
        }
    }
}
