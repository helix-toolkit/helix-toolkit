// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RubikCube.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace RubikDemo
{
    /// <summary>
    /// A rubik's cube - demo of building a geometry with Helix Toolkit's MeshBuilder
    /// and using animated transforms to do the rotations
    /// http://en.wikipedia.org/wiki/Rubik's_Cube
    /// http://www.rubiks.com/
    /// </summary>
    public class RubikCube : ModelVisual3D
    {
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(int), typeof(RubikCube), new UIPropertyMetadata(3, SizeChanged));

        private readonly Color[] faceColors = new[]
                                                  {
                                                      Colors.White, Colors.Red, Colors.Blue,
                                                      Colors.Orange, Colors.Green, Colors.Yellow
                                                  };
        // The indices of the faces are
        // 0:Bottom
        // 1:Front
        // 2:Left
        // 3:Right
        // 4:Down
        // 5:Up

        private double spacing = 0.06;
        private double size = 5.7;

        /// <summary>
        /// This array keeps track of which cubelet is located in each
        /// position (i,j,k) in the cube.
        /// </summary>
        private Model3DGroup[, ,] cubelets;

        /// <summary>
        /// The history of all moves that has been done.
        /// </summary>
        private readonly Stack<Tuple<int, double>> history = new Stack<Tuple<int, double>>();

        public RubikCube()
        {
            CreateCubelets();
        }

        private static Brush CreateFaceBrush(Color c, string text)
        {
            var db = new DrawingBrush
            {
                TileMode = TileMode.None,
                ViewportUnits = BrushMappingMode.Absolute,
                Viewport = new Rect(0, 0, 1, 1),
                Viewbox = new Rect(0, 0, 1, 1),
                ViewboxUnits = BrushMappingMode.Absolute
            };
            var dg = new DrawingGroup();
            dg.Children.Add(new GeometryDrawing { Geometry = new RectangleGeometry(new Rect(0, 0, 1, 1)), Brush = Brushes.Black });
            dg.Children.Add(new GeometryDrawing
                                {
                                    Geometry = new RectangleGeometry(new Rect(0.05, 0.05, 0.9, 0.9)) { RadiusX = 0.05, RadiusY = 0.05 },
                                    Brush = new SolidColorBrush(c)
                                });

            if (text != null)
            {
                var ft = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                                           new Typeface("Segoe UI"), 0.3, Brushes.Black);
                ft.TextAlignment = TextAlignment.Center;
                var geometry = ft.BuildGeometry(new Point(0, -0.2));
                var tg = new TransformGroup();
                tg.Children.Add(new RotateTransform(45));
                tg.Children.Add(new TranslateTransform(0.5, 0.5));
                geometry.Transform = tg;
                dg.Children.Add(new GeometryDrawing
                                    {
                                        Geometry = geometry,
                                        Brush = Brushes.Black

                                    });
            }
            db.Drawing = dg;
            return db;
        }

        public int Size
        {
            get { return (int)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        private static void SizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RubikCube)d).OnSizeChanged();
        }

        private void OnSizeChanged()
        {
            CreateCubelets();
        }

        private void CreateCubelets()
        {
            Children.Clear();
            cubelets = new Model3DGroup[Size, Size, Size];
            double o = -(Size - 1) * 0.5 * size;
            var faceBrushes = new Brush[faceColors.Length];

            for (int i = 0; i < faceColors.Length; i++)
            {
                faceBrushes[i] = CreateFaceBrush(faceColors[i], null);
                // SolidColorBrush is much faster
                // faceBrushes[i] = new SolidColorBrush(faceColors[i]);
            }

            var brush011 = CreateFaceBrush(Colors.White, "RUBIK");
            // var logobrush = new ImageBrush(new BitmapImage(new Uri(@"logo.png")));

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    for (int k = 0; k < Size; k++)
                    {
                        // the center of the cubelet
                        var center = new Point3D(o + i * size, o + j * size, o + k * size);

                        // add the 6 faces of a cubelet
                        var cubelet = new Model3DGroup();
                        for (int face = 0; face < 6; face++)
                        {
                            // find the color of the face
                            var color = IsOutsideFace(face, i, j, k) ? faceBrushes[face] : Brushes.Black;
                            if (face == 0 && i == 0 && j == 1 && k == 1)
                                color = brush011;
                            // and add a cube face
                            cubelet.Children.Add(CreateFace(face, center, size * (1 - spacing), size * (1 - spacing), size * (1 - spacing), color));
                        }
                        cubelets[i, j, k] = cubelet;
                        Children.Add(new ModelVisual3D { Content = cubelet });
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether a face given a position in the cube is on the outside.
        /// If it is not, we will give the face black colour.
        /// </summary>
        private bool IsOutsideFace(int face, int i, int j, int k)
        {
            switch (face)
            {
                case 0:
                    return i == 0;
                case 1:
                    return i == Size - 1;
                case 2:
                    return j == 0;
                case 3:
                    return j == Size - 1;
                case 4:
                    return k == 0;
                case 5:
                    return k == Size - 1;
            }
            return false;
        }

        private static GeometryModel3D CreateFace(int face, Point3D center, double width, double length, double height, Brush brush)
        {
            var m = new GeometryModel3D();
            var b = new MeshBuilder(false,true);
            switch (face)
            {
                case 0:
                    b.AddCubeFace(center, new Vector3D(-1, 0, 0), new Vector3D(0, 0, 1), length, width, height);
                    break;
                case 1:
                    b.AddCubeFace(center, new Vector3D(1, 0, 0), new Vector3D(0, 0, -1), length, width, height);
                    break;
                case 2:
                    b.AddCubeFace(center, new Vector3D(0, -1, 0), new Vector3D(0, 0, 1), width, length, height);
                    break;
                case 3:
                    b.AddCubeFace(center, new Vector3D(0, 1, 0), new Vector3D(0, 0, -1), width, length, height);
                    break;
                case 4:
                    b.AddCubeFace(center, new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), height, length, width);
                    break;
                case 5:
                    b.AddCubeFace(center, new Vector3D(0, 0, 1), new Vector3D(0, -1, 0), height, length, width);
                    break;
            }

            m.Geometry = b.ToMesh();
            m.Material = MaterialHelper.CreateMaterial(brush);
            return m;
        }

        public bool CanUnscramble()
        {
            return history.Count > 0;
        }

        private Random random = new Random();

        public void Scramble()
        {
            int face = random.Next(6);
            int rotation = 90;
            if (random.Next(2) == 0)
                rotation = -90;

            // push the move into the history
            history.Push(new Tuple<int, double>(face, rotation));

            Rotate(face, rotation);
        }

        public void Unscramble()
        {
            // pop the last move
            var tuple = history.Pop();
            int face = tuple.Item1;

            Rotate(face, -tuple.Item2);
        }

        public void Rotate(Key key)
        {
            bool control = (Keyboard.IsKeyDown(Key.LeftCtrl));
            bool shift = (Keyboard.IsKeyDown(Key.LeftShift));

            double angle = 90;
            if (shift) angle = -90;
            if (control) angle *= 2;

            int face;
            if (FaceKey.TryGetValue(key, out face))
            {
                history.Push(new Tuple<int, double>(face, angle));
                Rotate(face, angle);
            }
        }

        private Dictionary<Key, int> FaceKey =
            new Dictionary<Key, int>
                {
                    {Key.B,0},
                    {Key.F,1},
                    {Key.L,2},
                    {Key.R,3},
                    {Key.D,4},
                    {Key.U,5}
                };

        private Dictionary<int, Vector3D> RotationAxis =
            new Dictionary<int, Vector3D>
                {
                    {0, new Vector3D(-1, 0, 0)},
                    {1, new Vector3D(1, 0, 0)},
                    {2, new Vector3D(0,-1, 0)},
                    {3, new Vector3D(0,1, 0)},
                    {4, new Vector3D(0,0,-1)},
                    {5, new Vector3D(0,0,1)}
                };

        private DoubleAnimation Rotate(int face, double angle, double animationTime = 200)
        {
            Vector3D axis = RotationAxis[face];
            DoubleAnimation result = null;

            // we must update the array that contains the position of each cubelet
            var rotatedCubelets = new Model3DGroup[Size, Size, Size];
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    for (int k = 0; k < Size; k++)
                        rotatedCubelets[i, j, k] = cubelets[i, j, k];

            // positive angle is turning clockwise
            // turning face 0: (fix,*,*)

            //  2,0 2,1 2,2      2,2 1,2 0,2
            //  1,0 1,1 1,2  =>  2,1 1,1 0,1
            //  0,0 0,1 0,2      2,0 1,0 0,0

            // if angle is negative we need to rotate
            // the cubelets the other way

            int n = Size - 1;

            // this method only supports rotating the outer sides of the cube

            for (int a = 0; a < Size; a++)
            {
                for (int b = 0; b < Size; b++)
                {
                    int at = b;
                    int bt = n - a;
                    if (angle < 0)
                    {
                        at = n - b;
                        bt = a;
                    }

                    Model3DGroup group = null;
                    switch (face)
                    {
                        case 0:
                            group = rotatedCubelets[0, at, bt] = cubelets[0, a, b];
                            break;
                        case 1:
                            group = rotatedCubelets[n, bt, at] = cubelets[n, b, a];
                            break;
                        case 2:
                            group = rotatedCubelets[bt, 0, at] = cubelets[b, 0, a];
                            break;
                        case 3:
                            group = rotatedCubelets[at, n, bt] = cubelets[a, n, b];
                            break;
                        case 4:
                            group = rotatedCubelets[at, bt, 0] = cubelets[a, b, 0];
                            break;
                        case 5:
                            group = rotatedCubelets[bt, at, n] = cubelets[b, a, n];
                            break;
                        default:
                            continue;
                    }

                    var rot = new AxisAngleRotation3D { Axis = axis };
                    var anim = new DoubleAnimation(angle, new Duration(TimeSpan.FromMilliseconds(animationTime)))
                                   {
                                       AccelerationRatio = 0.3,
                                       DecelerationRatio = 0.5
                                   };

                    rot.BeginAnimation(AxisAngleRotation3D.AngleProperty, anim);
                    if (result == null)
                        result = anim;

                    var rott = new RotateTransform3D(rot);
                    var gt = new Transform3DGroup();
                    gt.Children.Add(group.Transform);
                    gt.Children.Add(rott);
                    group.Transform = gt;
                }
            }
            cubelets = rotatedCubelets;

            // can subscribe to the Completed event on this
            return result;
        }
    }
}