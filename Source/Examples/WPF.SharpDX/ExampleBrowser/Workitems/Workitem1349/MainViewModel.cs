// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Workitem1349
{

    using System.Linq;
    using DemoCore;
    using HelixToolkit.Wpf.SharpDX;
    using SharpDX;
    using MeshGeometry3D = HelixToolkit.Wpf.SharpDX.MeshGeometry3D;
    using PerspectiveCamera = HelixToolkit.Wpf.SharpDX.PerspectiveCamera;

    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;
    using Color = System.Windows.Media.Color;
    using Colors = System.Windows.Media.Colors;

    public class MainViewModel : BaseViewModel
    {
        public MeshGeometry3D Model { get; private set; }
        public LineGeometry3D Grid { get; private set; }

        public SharpDX.Color GridColor { get; private set; }

        public Color DirectionalLightColor { get; private set; }
        public Color AmbientLightColor { get; private set; }

        public BillboardText3D Text3D { get; set; } = new BillboardText3D()
        {
            IsDynamic = true
        };

        public MainViewModel()
        {
            // titles
            this.Title = "Bug? BillboardText3D not visible until origin comes in sight.";
            this.SubTitle = "Please move or rotate the view and see how the BillboardText suddenly appears when the origin (right front edge of the grid) comes into view.";

            // camera setup
            this.Camera = new PerspectiveCamera { Position = new Point3D(4.4, 2.2, -4.4), LookDirection = new Vector3D(0, -4, 10), UpDirection = new Vector3D(0, 1, 0) };

            EffectsManager = new DefaultEffectsManager();
            
            // setup lighting            
            this.AmbientLightColor = Colors.Black;
            this.DirectionalLightColor = Colors.White;

            // floor plane grid
            this.Grid = LineBuilder.GenerateGrid();
            this.GridColor = SharpDX.Color.Black;

            // scene model3d
            var b1 = new MeshBuilder();            
            b1.AddSphere(new Vector3(0, 0, 0), 0.05);
           
            var meshGeometry = b1.ToMeshGeometry3D();
            meshGeometry.Colors = new Color4Collection(meshGeometry.TextureCoordinates.Select(x => x.ToColor4()));
            this.Model = meshGeometry;

            // Create Billboard Text
            float offset = 4.5f;
            float scale = 0.8f;
//            Text3D.TextInfo.Add(new TextInfo("Origin", new Vector3(0,0,0)) { Foreground = SharpDX.Color.Red, Scale = scale * 2 });
//            Text3D.TextInfo.Add(new TextInfo("1", new Vector3(1,0,0)) { Foreground = SharpDX.Color.Blue, Scale = scale * 2 });
            Text3D.TextInfo.Add(new TextInfo("2", new Vector3(2,0,0)) { Foreground = SharpDX.Color.Blue, Scale = scale * 2 });
            Text3D.TextInfo.Add(new TextInfo("3", new Vector3(3,0,3)) { Foreground = SharpDX.Color.Blue, Scale = scale * 2 });
            Text3D.TextInfo.Add(new TextInfo("4", new Vector3(4,0,3)) { Foreground = SharpDX.Color.Blue, Scale = scale * 2 });
            Text3D.TextInfo.Add(new TextInfo("5", new Vector3(5,0,3)) { Foreground = SharpDX.Color.Blue, Scale = scale * 2 });

        }
    }
}