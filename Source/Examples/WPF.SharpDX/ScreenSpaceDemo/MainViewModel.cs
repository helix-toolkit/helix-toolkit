// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ScreenSpaceDemo
{
    using DemoCore;
    using HelixToolkit.Wpf.SharpDX;
    using HelixToolkit.Wpf.SharpDX.Model;
    using HelixToolkit.Wpf.SharpDX.Extensions;

    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using Media3D = System.Windows.Media.Media3D;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using RotateTransform3D = System.Windows.Media.Media3D.RotateTransform3D;
    using ScaleTransform3D = System.Windows.Media.Media3D.ScaleTransform3D;
    using Transform3D = System.Windows.Media.Media3D.Transform3D;
    using Transform3DGroup = System.Windows.Media.Media3D.Transform3DGroup;
    using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;


    public class MainViewModel : BaseViewModel
    {
        public ObservableElement3DCollection ModelGeometry { get; private set; }
        public MeshGeometry3D Model { get; private set; }
        public LineGeometry3D Lines { get; private set; }
        public LineGeometry3D Grid { get; private set; }

        public PhongMaterial RedMaterial { get; private set; }
        public PhongMaterial DefaultMaterial { get; private set; }
        public Color GridColor { get; private set; }

        public Transform3D ModelTransform { get; private set; }

        public Vector3 DirectionalLightDirection1 { get; private set; }
        public Vector3 DirectionalLightDirection2 { get; private set; }
        public Color4 DirectionalLightColor { get; private set; }
        public Color4 AmbientLightColor { get; private set; }
        public Color4 BackgroundColor { get; private set; }

        public MainViewModel()
        {
            // ----------------------------------------------
            // titles
            this.Title = "Screen Space Ambient Occlusion Demo";
            this.SubTitle = "WPF & SharpDX";

            // camera setup
            this.Camera = new PerspectiveCamera { Position = new Point3D(1.5, 2.5, 2.5), LookDirection = new Vector3D(-1.5, -2.5, -2.5), UpDirection = new Vector3D(0, 1, 0) };

            // default render technique
            EffectsManager = new DefaultEffectsManager();
            RenderTechnique = EffectsManager[DefaultRenderTechniqueNames.Blinn];

            // background
            this.BackgroundColor = (Color4)Color.White;
            
            // setup lighting            
            this.AmbientLightColor = new Color4(0.1f, 0.1f, 0.1f, 1.0f);
            this.DirectionalLightColor = Color.White;
            this.DirectionalLightDirection1 = new Vector3(-2, -5, -2);
            this.DirectionalLightDirection2 = new Vector3(+2, +5, +5);
     
            // model materials            
            this.DefaultMaterial = PhongMaterials.DefaultVRML;            

            //load model
            var reader = new ObjReader();
            var objModel = reader.Read(@"./Media/CornellBox-Glossy.obj");                              
                        
            this.ModelGeometry = new ObservableElement3DCollection();
            foreach(var model in objModel.Select(x => new MeshGeometryModel3D() { Geometry = x.Geometry as MeshGeometry3D, Material = GetMaterialFromMaterialCore(x.Material as PhongMaterialCore), }))
            {
              this.ModelGeometry.Add(model);
            }
                          
            // model trafos
            this.ModelTransform = new Media3D.TranslateTransform3D(0, 0, 0);            
        }

        private static Material GetMaterialFromMaterialCore(PhongMaterialCore material)
        {
            if (material == null)
            {
                return null;
            }

            var mat = new PhongMaterial
            {
                Name = material.Name,
                SpecularColor = material.SpecularColor,
                SpecularShininess = material.SpecularShininess,
                AmbientColor = material.AmbientColor,
                DiffuseColor = material.DiffuseColor,
                DiffuseMap = material.DiffuseMap,
                DiffuseMapSampler = material.DiffuseMapSampler,
                DiffuseAlphaMap = material.DiffuseAlphaMap,
                DiffuseAlphaMapSampler = material.DiffuseAlphaMapSampler,
                DisplacementMap = material.DisplacementMap,
                DisplacementMapSampler = material.DisplacementMapSampler,
                DisplacementMapScaleMask = material.DisplacementMapScaleMask,
                EmissiveColor = material.EmissiveColor,
                EnableTessellation = material.EnableTessellation,
                MaxDistanceTessellationFactor = material.MaxDistanceTessellationFactor,
                MaxTessellationDistance = material.MaxTessellationDistance,
                MinDistanceTessellationFactor = material.MinDistanceTessellationFactor,
                MinTessellationDistance = material.MinTessellationDistance,
                NormalMap = material.NormalMap,
                NormalMapSampler = material.NormalMapSampler,
                ReflectiveColor = material.ReflectiveColor,
                RenderDiffuseAlphaMap = material.RenderDiffuseAlphaMap,
                RenderDiffuseMap = material.RenderDiffuseMap,
                RenderDisplacementMap = material.RenderDisplacementMap,
                RenderNormalMap = material.RenderNormalMap,
            };

            return mat;
        }
    }
}
