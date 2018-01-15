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

    using HelixToolkit.Wpf;
    using HelixToolkit.Wpf.SharpDX;
    using HelixToolkit.Wpf.SharpDX.Core;

    using SharpDX;
    using Media3D = System.Windows.Media.Media3D;
    using Color = System.Windows.Media.Color;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;
    using System.Collections.Generic;

    public class MainViewModel : BaseViewModel
    {
        public MeshGeometry3D Model { get; private set; }
        public LineGeometry3D Lines { get; private set; }
        public LineGeometry3D Grid { get; private set; }
        public PointGeometry3D Points { get; private set; }
        public BillboardText3D Text { get; private set; }

        public PhongMaterial ModelMaterial { get; private set; } = PhongMaterials.White;


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
                    ColorGradient = new Color4Collection(GetGradients(startColor, endColor, 100).Select(x=>x.ToColor4()));
                }
            }
            get { return startColor; }
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
                    ColorGradient = new Color4Collection(GetGradients(startColor, endColor, 100).Select(x => x.ToColor4()));
                }
            }
            get { return endColor; }
        }

        public Color4Collection ColorGradient { private set; get; }

        public MainViewModel()
        {
            // titles
            Title = "Simple Demo";
            SubTitle = "WPF & SharpDX";

            // camera setup
            Camera = new PerspectiveCamera { 
                Position = new Point3D(3, 3, 5), 
                LookDirection = new Vector3D(-3, -3, -5), 
                UpDirection = new Vector3D(0, 1, 0),
                FarPlaneDistance = 5000
            };

            EffectsManager = new CustomEffectsManager();
            RenderTechnique = EffectsManager[CustomShaderNames.DataSampling];

            var builder = new MeshBuilder(true);
            Vector3[] points = new Vector3[2500];
            for(int i=0; i<50; ++i)
            {
                for(int j=0; j<50; ++j)
                {
                    points[i * 50 + j] = new Vector3(i / 10, 0, j / 10);
                }
            }
            builder.AddRectangularMesh(points, 50);
            Model = builder.ToMesh();
            StartColor = Colors.Blue;
            EndColor = Colors.Red;
        }

        public static IEnumerable<Color> GetGradients(Color start, Color end, int steps)
        {
            int stepA = ((end.A - start.A) / (steps - 1));
            int stepR = ((end.R - start.R) / (steps - 1));
            int stepG = ((end.G - start.G) / (steps - 1));
            int stepB = ((end.B - start.B) / (steps - 1));

            for (int i = 0; i < steps; i++)
            {
                yield return Color.FromArgb((byte)(start.A + (stepA * i)),
                                            (byte)(start.R + (stepR * i)),
                                            (byte)(start.G + (stepG * i)),
                                            (byte)(start.B + (stepB * i)));
            }
        }
    }
}
