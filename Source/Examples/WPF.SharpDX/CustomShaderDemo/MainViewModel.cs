// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CustomShaderDemo
{
    using System.Linq;
    using System.Windows.Media;
    using DemoCore;

    using HelixToolkit.Wpf.SharpDX;
    using HelixToolkit.Wpf.SharpDX.Core;

    using System.Numerics;
    using HelixToolkit.Mathematics;
    using Media3D = System.Windows.Media.Media3D;
    using Color = System.Windows.Media.Color;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;
    using System.Collections.Generic;
    using System.Windows.Input;
    using System;
    using SharpDX.Direct3D11;

    public class MainViewModel : BaseViewModel
    {
        public MeshGeometry3D Model { get; private set; }
        public MeshGeometry3D SphereModel { get; private set; }
        public LineGeometry3D AxisModel { get; private set; }
        public BillboardText3D AxisLabel { private set; get; }
        public ColorStripeMaterial ModelMaterial { get; private set; } = new ColorStripeMaterial();
        public PhongMaterial SphereMaterial { private set; get; } = PhongMaterials.Copper;

        private Color startColor;
        /// <summary>
        /// Gets or sets the StartColor.
        /// </summary>
        /// <value>
        /// StartColor
        /// </value>
        public Color StartColor
        {
            set
            {
                if (SetValue(ref startColor, value))
                {
                    ColorGradient = new Color4Collection(GetGradients(startColor.ToColor4(), midColor.ToColor4(), endColor.ToColor4(), 100));
                }
            }
            get { return startColor; }
        }

        private Color midColor;
        /// <summary>
        /// Gets or sets the StartColor.
        /// </summary>
        /// <value>
        /// StartColor
        /// </value>
        public Color MidColor
        {
            set
            {
                if (SetValue(ref midColor, value))
                {
                    ColorGradient = new Color4Collection(GetGradients(startColor.ToColor4(), midColor.ToColor4(), endColor.ToColor4(), 100));
                }
            }
            get { return midColor; }
        }

        private Color endColor;
        /// <summary>
        /// Gets or sets the StartColor.
        /// </summary>
        /// <value>
        /// StartColor
        /// </value>
        public Color EndColor
        {
            set
            {
                if (SetValue(ref endColor, value))
                {
                    ColorGradient = new Color4Collection(GetGradients(startColor.ToColor4(), midColor.ToColor4(), endColor.ToColor4(), 100));
                }
            }
            get { return endColor; }
        }

        private Color4Collection colorGradient;
        public Color4Collection ColorGradient
        {
            private set
            {
                if(SetValue(ref colorGradient, value))
                {
                    ModelMaterial.ColorStripeX = value;
                }
            }
            get { return colorGradient; }
        }

        private FillMode fillMode = FillMode.Solid;
        public FillMode FillMode
        {
            set
            {
                SetValue(ref fillMode, value);
            }
            get { return fillMode; }
        }

        private bool showWireframe = false;
        public bool ShowWireframe
        {
            set
            {
                if(SetValue(ref showWireframe, value))
                {
                    FillMode = value ? FillMode.Wireframe : FillMode.Solid;
                }
            }
            get { return showWireframe; }
        }

        private int Width = 100;
        private int Height = 100;

        public ICommand GenerateNoiseCommand { private set; get; }

        public MainViewModel()
        {
            // titles
            Title = "Simple Demo";
            SubTitle = "WPF & SharpDX";

            // camera setup
            Camera = new PerspectiveCamera { 
                Position = new Point3D(-6, 8, 23), 
                LookDirection = new Vector3D(11, -4, -23), 
                UpDirection = new Vector3D(0, 1, 0),
                FarPlaneDistance = 5000
            };

            EffectsManager = new CustomEffectsManager();
           

            var builder = new MeshBuilder(true);
            Vector3[] points = new Vector3[Width*Height];
            for(int i=0; i<Width; ++i)
            {
                for(int j=0; j<Height; ++j)
                {
                    points[i * Width + j] = new Vector3(i / 10f, 0, j / 10f);
                }
            }
            builder.AddRectangularMesh(points, Width);
            Model = builder.ToMesh();
            for(int i=0; i<Model.Normals.Count; ++i)
            {
                Model.Normals[i] = new Vector3(0, Math.Abs(Model.Normals[i].Y), 0);
            }
            StartColor = Colors.Blue;
            MidColor = Colors.Green;
            EndColor = Colors.Red;

            var lineBuilder = new LineBuilder();
            lineBuilder.AddLine(new Vector3(0, 0, 0), new Vector3(10, 0, 0));
            lineBuilder.AddLine(new Vector3(0, 0, 0), new Vector3(0, 10, 0));
            lineBuilder.AddLine(new Vector3(0, 0, 0), new Vector3(0, 0, 10));

            AxisModel = lineBuilder.ToLineGeometry3D();
            AxisModel.Colors = new Color4Collection(AxisModel.Positions.Count);
            AxisModel.Colors.Add(Colors.Red.ToColor4());
            AxisModel.Colors.Add(Colors.Red.ToColor4());
            AxisModel.Colors.Add(Colors.Green.ToColor4());
            AxisModel.Colors.Add(Colors.Green.ToColor4());
            AxisModel.Colors.Add(Colors.Blue.ToColor4());
            AxisModel.Colors.Add(Colors.Blue.ToColor4());

            AxisLabel = new BillboardText3D();
            AxisLabel.TextInfo.Add(new TextInfo() { Origin = new Vector3(11, 0, 0), Text = "X", Foreground = Colors.Red.ToColor4() });
            AxisLabel.TextInfo.Add(new TextInfo() { Origin = new Vector3(0, 11, 0), Text = "Y", Foreground = Colors.Green.ToColor4() });
            AxisLabel.TextInfo.Add(new TextInfo() { Origin = new Vector3(0, 0, 11), Text = "Z", Foreground = Colors.Blue.ToColor4() });

            builder = new MeshBuilder(true);
            builder.AddSphere(new Vector3(-15, 0, 0), 5);
            SphereModel = builder.ToMesh();

            GenerateNoiseCommand = new RelayCommand((o) => { CreatePerlinNoise(); });
            CreatePerlinNoise();
        }

        public static IEnumerable<Color4> GetGradients(Color4 start, Color4 mid, Color4 end, int steps)
        {
            return GetGradients(start, mid, steps / 2).Concat(GetGradients(mid, end, steps / 2));
        }

        public static IEnumerable<Color4> GetGradients(Color4 start, Color4 end, int steps)
        {
            float stepA = ((end.Alpha - start.Alpha) / (steps - 1));
            float stepR = ((end.Red - start.Red) / (steps - 1));
            float stepG = ((end.Green - start.Green) / (steps - 1));
            float stepB = ((end.Blue - start.Blue) / (steps - 1));

            for (int i = 0; i < steps; i++)
            {
                yield return new Color4((start.Red + (stepR * i)),
                                            (start.Green + (stepG * i)),
                                            (start.Blue + (stepB * i)),
                                            (start.Alpha + (stepA * i)));
            }
        }

        private void CreatePerlinNoise()
        {
            float[] noise;
            MathHelper.GenerateNoiseMap(Width, Height, 8, out noise);
            Vector2Collection collection = new Vector2Collection(Width * Height);
            for(int i=0; i<Width; ++i)
            {
                for(int j=0; j<Height; ++j)
                {
                    collection.Add(new Vector2(Math.Abs(noise[Width * i + j]), 0));
                }
            }
            Model.TextureCoordinates = collection;
        }
    }
}
