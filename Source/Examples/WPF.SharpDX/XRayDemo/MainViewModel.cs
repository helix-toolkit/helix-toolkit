// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XRayDemo
{
    using System;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using DemoCore;
    using HelixToolkit.Wpf.SharpDX;
    using SharpDX;
    using Media3D = System.Windows.Media.Media3D;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;
    using Transform3D = System.Windows.Media.Media3D.Transform3D;
    using Color = System.Windows.Media.Color;
    using Plane = SharpDX.Plane;
    using Vector3 = SharpDX.Vector3;
    using Colors = System.Windows.Media.Colors;
    using Color4 = SharpDX.Color4;
    using HelixToolkit.Wpf;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;
    using SharpDX.Direct3D11;
    using System.Windows;
    using System.Windows.Data;
    using HelixToolkit.Wpf.SharpDX.Extensions;

    public class MainViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public MainViewModel ViewModel { get { return this; } }
        public MeshGeometry3D Model { get; private set; }
        public MeshGeometry3D Floor { get; private set; }

        public MeshGeometry3D CarModel { private set; get; }

        public PhongMaterial ModelMaterial { get; set; }
        public PhongMaterial FloorMaterial { get; set; }
        public PhongMaterial LightModelMaterial { get; set; }

        public Transform3D ModelTransform { private set; get; }

        public Vector3D Light1Direction { get; set; }
        public Color Light1Color { get; set; }
        public Color AmbientLightColor { get; set; }
        private Vector3D camLookDir = new Vector3D(-100, -100, -100);
        public Vector3D CamLookDir
        {
            set
            {
                if (camLookDir != value)
                {
                    camLookDir = value;
                    OnPropertyChanged();
                    Light1Direction = value;
                }
            }
            get
            {
                return camLookDir;
            }
        }

        public Matrix[] Instances { private set; get; }
        public Matrix[] OutlineInstances { private set; get; }
        public BlendStateDescription BlendDescription
        {
            set; get;
        }

        public DepthStencilStateDescription DepthStencilDescription
        {
            set;get;
        }

        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            // ----------------------------------------------
            // titles
            this.Title = "Lighting Demo";
            this.SubTitle = "WPF & SharpDX";

            // ----------------------------------------------
            // camera setup
            this.Camera = new PerspectiveCamera { Position = new Point3D(100, 100, 100), LookDirection = new Vector3D(-100, -100, -100), UpDirection = new Vector3D(0, 1, 0) };
            // ----------------------------------------------
            // setup scene
            this.AmbientLightColor = Colors.DimGray;
            this.Light1Color = Colors.LightGray;


            this.Light1Direction = new Vector3D(-100, -100, -100);
            SetupCameraBindings(Camera);
            // ----------------------------------------------
            // ----------------------------------------------
            // scene model3d
            this.ModelMaterial = PhongMaterials.Silver;

            // ----------------------------------------------
            // floor model3d
            var b2 = new MeshBuilder(true, true, true);
            b2.AddBox(new Vector3(0.0f, 0, 0.0f), 150, 1, 150, BoxFaces.All);
            b2.AddBox(new Vector3(0, 25, 70), 150, 50, 20);
            b2.AddBox(new Vector3(0, 25, -70), 150, 50, 20);
            this.Floor = b2.ToMeshGeometry3D();
            this.FloorMaterial = PhongMaterials.Bisque;
            this.FloorMaterial.DiffuseMap = LoadFileToMemory(new System.Uri(@"TextureCheckerboard2.jpg", System.UriKind.RelativeOrAbsolute).ToString());
            this.FloorMaterial.NormalMap = LoadFileToMemory(new System.Uri(@"TextureCheckerboard2_dot3.jpg", System.UriKind.RelativeOrAbsolute).ToString());

            var caritems = Load3ds("leone.3DBuilder.obj").Select(x => x.Geometry as MeshGeometry3D).ToArray();
            var scale = new Vector3(1f);

            foreach (var item in caritems)
            {
                for (int i = 0; i < item.Positions.Count; ++i)
                {
                    item.Positions[i] = item.Positions[i] * scale;
                }
                
            }
            Model = MeshGeometry3D.Merge(caritems);

            ModelTransform = new Media3D.RotateTransform3D() { Rotation = new Media3D.AxisAngleRotation3D(new Vector3D(1, 0, 0), -90) };

            Instances = new Matrix[6];
            for(int i=0; i<Instances.Length; ++i)
            {
                Instances[i] = Matrix.Translation(new Vector3(15 * i - 30, 15 * (i % 2) - 30, 0));
            }

            OutlineInstances = new Matrix[6];
            for (int i = 0; i < Instances.Length; ++i)
            {
                OutlineInstances[i] = Matrix.Translation(new Vector3(15 * i - 30, 15 * (i % 2), 0));
            }

            var blendDesc = new BlendStateDescription();
            blendDesc.RenderTarget[0] = new RenderTargetBlendDescription
            {
                IsBlendEnabled = true,
                BlendOperation = BlendOperation.Add,
                AlphaBlendOperation = BlendOperation.Add,
                SourceBlend = BlendOption.One,
                DestinationBlend = BlendOption.One,
                SourceAlphaBlend = BlendOption.Zero,
                DestinationAlphaBlend = BlendOption.One,
                RenderTargetWriteMask = ColorWriteMaskFlags.All
            };
            BlendDescription = blendDesc;
            DepthStencilDescription = new DepthStencilStateDescription()
            {
                IsDepthEnabled = true,
                DepthComparison = Comparison.LessEqual,
                DepthWriteMask = DepthWriteMask.Zero
            };
        }

        public List<Object3D> Load3ds(string path)
        {
            var reader = new ObjReader();
            var list = reader.Read(path);
            return list;
        }

        public void SetupCameraBindings(Camera camera)
        {
            if (camera is ProjectionCamera)
            {
                SetBinding("CamLookDir", camera, ProjectionCamera.LookDirectionProperty, this);
            }
        }

        private static void SetBinding(string path, DependencyObject dobj, DependencyProperty property, object viewModel, BindingMode mode = BindingMode.TwoWay)
        {
            var binding = new Binding(path);
            binding.Source = viewModel;
            binding.Mode = mode;
            BindingOperations.SetBinding(dobj, property, binding);
        }
    }
}