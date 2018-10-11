
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace PolygonTriangulationDemo
{
    using DemoCore;
    using HelixToolkit.Wpf.SharpDX;
    using SharpDX;
    using System;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Transform3D = System.Windows.Media.Media3D.Transform3D;
    using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;

    public class MainViewModel : BaseViewModel
    {
        /// <summary>
        /// Thickness of the Lines of the Grid
        /// </summary>
        public double LineThickness { get; set; }
        /// <summary>
        /// Thickness of the Lines of the Triangulated Polygon
        /// </summary>
        public double TriangulationThickness { get; set; }
        /// <summary>
        /// The Grid
        /// </summary>
        public LineGeometry3D Grid { get; private set; }
        /// <summary>
        /// Color of the Gridlines
        /// </summary>
        public SharpDX.Color GridColor { get; private set; }
        /// <summary>
        /// Color of the Triangle Lines
        /// </summary>
        public SharpDX.Color TriangulationColor { get; private set; }
        /// <summary>
        /// Transform of the Grid
        /// </summary>
        public Transform3D GridTransform { get; private set; }
        /// <summary>
        /// Direction of the directional Light
        /// </summary>
        public Vector3 DirectionalLightDirection { get; private set; }
        /// <summary>
        /// Color of the directional Light
        /// </summary>
        public Color4 DirectionalLightColor { get; private set; }
        /// <summary>
        /// Color of the ambient Light
        /// </summary>
        public Color4 AmbientLightColor { get; private set; }
        /// <summary>
        /// The Polygon-Material
        /// </summary>
        private PhongMaterial mMaterial;
        /// <summary>
        /// Accessor to the Polygon-Material
        /// </summary>
        public PhongMaterial Material
        {
            get
            {
                return mMaterial;
            }
            set
            {
                mMaterial = value;
                OnPropertyChanged("Material");
            }
        }
        /// <summary>
        /// Transform of the Polygon
        /// </summary>
        public Transform3D ModelTransform { get; private set; }
        /// <summary>
        /// Transform of the Polygon Triangle Lines
        /// </summary>
        public Transform3D ModelLineTransform { get; private set; }
        /// <summary>
        /// Collection of Materials to chose from
        /// </summary>
        public PhongMaterialCollection Materials
        {
            get
            {
                return HelixToolkit.Wpf.SharpDX.PhongMaterials.Materials;
            }
        }
        /// <summary>
        /// Draw the Triangles or not
        /// </summary>
        private Boolean mShowTriangleLines;
        /// <summary>
        /// Accessor to the Boolean
        /// </summary>
        public Boolean ShowTriangleLines {
            get
            {
                return mShowTriangleLines;
            }
            set
            {
                mShowTriangleLines = value;
                OnPropertyChanged("ShowTriangleLines");
            }
        }
        /// <summary>
        /// The Point Count
        /// </summary>
        private int mPointCount;
        /// <summary>
        /// Access to the Point Count (restricted to the Range 3 - 10.000)
        /// </summary>
        public int PointCount
        {
            get
            {
                return mPointCount;
            }
            set
            {
                if (value < 3)
                    mPointCount = 3;
                else if (value > 10000)
                    mPointCount = 10000;
                else
                    mPointCount = value;
                OnPropertyChanged("PointCountText");
            }
        }
        /// <summary>
        /// Text representing the current PointCount
        /// </summary>
        public string PointCountText
        {
            get { return "Number of Points: " + this.mPointCount; }
        }
        /// <summary>
        /// The Geometry for the Triangle Lines
        /// </summary>
        public HelixToolkit.Wpf.SharpDX.LineGeometry3D LineGeometry;

        /// <summary>
        /// Constructor of the MainViewModel
        /// Sets up allProperties
        /// </summary>
        public MainViewModel()
        {
            // Render Setup
            EffectsManager = new DefaultEffectsManager();
            // Window Setup
            this.Title = "Polygon Triangulation Demo";
            this.SubTitle = null;

            // Camera Setup
            this.Camera = new PerspectiveCamera { Position = new Point3D(0, 5, 9), LookDirection = new Vector3D(0, -5, -4), UpDirection = new Vector3D(0, 1, 0) };
            
            // Lines Setup
            this.LineThickness = 1;
            this.TriangulationThickness = .5;
            this.ShowTriangleLines = true;

            // Count Setup
            this.PointCount = 1000;

            // Lighting Setup
            this.AmbientLightColor = new Color4(.1f, .1f, .1f, 1.0f);
            this.DirectionalLightColor = Color.White;
            this.DirectionalLightDirection = new Vector3(0, -1, 0);
            
            // Model Transformations
            this.ModelTransform = new TranslateTransform3D(0, 0, 0);
            this.ModelLineTransform = new TranslateTransform3D(0, 0.001, 0);
            
            // Model Materials and Colors
            this.Material = PhongMaterials.PolishedBronze;
            this.TriangulationColor = SharpDX.Color.Black;

            // Grid Setup
            this.Grid = LineBuilder.GenerateGrid(Vector3.UnitY, -5, 5, 0, 10);
            this.GridColor = SharpDX.Color.DarkGray;
            this.GridTransform = new TranslateTransform3D(0, -0.01, 0);
        }
    }
}