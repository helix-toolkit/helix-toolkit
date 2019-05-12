// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LegoVisual3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Traditional Lego bricks.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using HelixToolkit.Wpf;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Media3D;

namespace CoreWpfDemo
{
    /// <summary>
    /// Traditional Lego bricks.
    /// </summary>
    public class LegoVisual3D : MeshElement3D
    {
        private const double grid = 0.008;
        private const double margin = 0.0001;
        private const double wallThickness = 0.001;
        private const double plateThickness = 0.0032;
        private const double brickThickness = 0.0096;
        private const double knobHeight = 0.0018;
        private const double knobDiameter = 0.0048;
        private const double outerDiameter = 0.00651;
        private const double axleDiameter = 0.00475;
        private const double holeDiameter = 0.00485;

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof (int), typeof (LegoVisual3D),
                                        new UIPropertyMetadata(3, GeometryChanged));

        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register("Rows", typeof (int), typeof (LegoVisual3D),
                                        new UIPropertyMetadata(2, GeometryChanged));

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof (int), typeof (LegoVisual3D),
                                        new UIPropertyMetadata(6, GeometryChanged));
        public int Divisions
        {
            get { return (int)GetValue(DivisionsProperty); }
            set { SetValue(DivisionsProperty, value); }
        }

        public static readonly DependencyProperty DivisionsProperty =
            DependencyProperty.Register("Divisions", typeof(int), typeof(LegoVisual3D), new UIPropertyMetadata(12));

        [Category("Lego attributes")]
        public int Height
        {
            get { return (int) GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        public int Rows
        {
            get { return (int) GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        public int Columns
        {
            get { return (int) GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        // http://www.robertcailliau.eu/Lego/Dimensions/zMeasurements-en.xhtml
        public static double GridUnit
        {
            get { return grid; }
        }
        public static double HeightUnit
        {
            get { return plateThickness; }
        }

        protected override MeshGeometry3D Tessellate()
        {
            double width = Columns*grid - margin*2;
            double length = Rows*grid - margin*2;
            double height = Height*plateThickness;
            var builder = new MeshBuilder(true, true);

            for (int i = 0; i < Columns; i++)
                for (int j = 0; j < Rows; j++)
                {
                    var o = new Point3D((i + 0.5)*grid, (j + 0.5)*grid, height);
                    builder.AddCone(o, new Vector3D(0, 0, 1), knobDiameter/2, knobDiameter/2, knobHeight, false, true,
                                    Divisions);
                    builder.AddPipe(new Point3D(o.X, o.Y, o.Z - wallThickness), new Point3D(o.X, o.Y, wallThickness),
                                    knobDiameter, outerDiameter, Divisions);
                }

            builder.AddBox(new Point3D(Columns * 0.5 * grid, Rows * 0.5 * grid, height - wallThickness / 2), width, length,
                          wallThickness,
                          BoxFaces.All);
            builder.AddBox(new Point3D(margin + wallThickness / 2, Rows * 0.5 * grid, height / 2 - wallThickness / 2),
                           wallThickness, length, height - wallThickness,
                           BoxFaces.All ^ BoxFaces.Top);
            builder.AddBox(
                new Point3D(Columns * grid - margin - wallThickness / 2, Rows * 0.5 * grid, height / 2 - wallThickness / 2),
                wallThickness, length, height - wallThickness,
                BoxFaces.All ^ BoxFaces.Top);
            builder.AddBox(new Point3D(Columns * 0.5 * grid, margin + wallThickness / 2, height / 2 - wallThickness / 2),
                           width, wallThickness, height - wallThickness,
                           BoxFaces.All ^ BoxFaces.Top);
            builder.AddBox(
                new Point3D(Columns * 0.5 * grid, Rows * grid - margin - wallThickness / 2, height / 2 - wallThickness / 2),
                width, wallThickness, height - wallThickness,
                BoxFaces.All ^ BoxFaces.Top);

            return builder.ToMesh();
        }
    }
}