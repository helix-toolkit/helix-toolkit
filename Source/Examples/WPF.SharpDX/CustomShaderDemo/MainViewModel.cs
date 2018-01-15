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
                    ColorGradient = new Color4Collection(GetGradients(startColor.ToColor4(), endColor.ToColor4(), 100));
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
                    ColorGradient = new Color4Collection(GetGradients(startColor.ToColor4(), endColor.ToColor4(), 100));
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
                    points[i * 50 + j] = new Vector3(i / 10f, 0, j / 10f);
                }
            }
            builder.AddRectangularMesh(points, 50);
            Model = builder.ToMesh();
            StartColor = Colors.Blue;
            EndColor = Colors.Red;
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
    }
}
