using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using Color = System.Windows.Media.Color;
using Plane = SharpDX.Plane;
using Vector3 = SharpDX.Vector3;
using Colors = System.Windows.Media.Colors;
using Color4 = SharpDX.Color4;
using System.IO;

namespace MaterialDemo
{
    public class PBRViewModel : BaseViewModel
    {
        public Geometry3D SphereModel { get; }
        private const int Row = 5;
        private const int Col = 5;
        private const int Size = 5 * 5;
        public Stream EnvironmentMap { set; get; }
        public ObservableElement3DCollection Models { get; } = new ObservableElement3DCollection();
        private List<PBRMaterial> materials = new List<PBRMaterial>();
        public Geometry3D Model { get; }
        public Geometry3D FloorModel { get; }
        public Transform3D ModelTransform { get; }
        public PBRMaterial Material { get; }
        public PBRMaterial FloorMaterial { get; }
        public Transform3D FloorModelTransform { get; }
        private Color albedoColor = Colors.Gold;
        public Color AlbedoColor
        {
            set
            {
                if(SetValue(ref albedoColor, value))
                {
                    foreach(var m in materials)
                    {
                        m.AlbedoColor = value.ToColor4();
                    }

                    Material.AlbedoColor = value.ToColor4();
                }
            }
            get { return albedoColor; }
        }

        private bool renderEnvironment = true;
        public bool RenderEnvironment
        {
            set
            {
                if(SetValue(ref renderEnvironment, value))
                {
                    foreach (var m in materials)
                    {
                        m.RenderEnvironmentMap = value;
                    }                   
                }
            }
            get { return renderEnvironment; }
        }
        private bool renderNormalMap = true;
        public bool RenderNormalMap
        {
            set
            {
                if (SetValue(ref renderNormalMap, value))
                {
                    foreach (var m in materials)
                    {
                        m.RenderNormalMap = value;
                    }
                }
            }
            get { return renderNormalMap; }
        }
        public PBRViewModel(IEffectsManager manager)
        {
            EffectsManager = manager;
            this.Camera = new PerspectiveCamera { Position = new Point3D(0, 60, 60), LookDirection = new Vector3D(0, -60, -60), UpDirection = new Vector3D(0, 1, 0) };
            var builder = new MeshBuilder();
            builder.AddSphere(Vector3.Zero, 2);
            SphereModel = builder.ToMesh();
            var normalMap = LoadFileToMemory(new System.Uri("TextureNoise1_dot3.dds", System.UriKind.RelativeOrAbsolute).ToString());
            for (int i = -Row; i < Row; ++i)
            {

                for(int j=-Col; j<Col; ++j)
                {
                    var m = new PBRMaterial()
                    {
                        AlbedoColor = albedoColor.ToColor4(),
                        RoughnessFactor = 1.0 / (2 * Row) * Math.Abs(i + Row),
                        MetallicFactor = 1.0 / (2 * Col) * Math.Abs(j + Col),
                        RenderEnvironmentMap = true,
                        EnableAutoTangent = true,
                        NormalMap = normalMap,
                        RenderShadowMap = true
                    };
                    materials.Add(m);
                    Models.Add(new MeshGeometryModel3D()
                    {
                        CullMode = SharpDX.Direct3D11.CullMode.Back,
                        Geometry = SphereModel,
                        Material = m,
                        IsThrowingShadow = true,
                        Transform = new Media3D.TranslateTransform3D(new Vector3D(i * 6, 0, j * 6))
                    });
                }
            }
            builder = new MeshBuilder();
            builder.AddSphere(Vector3.Zero, 8, 64, 64);
            Model = builder.ToMesh();
            Material = new PBRMaterial()
            {
                AlbedoColor = albedoColor.ToColor4(),
                RenderEnvironmentMap = true,
                AlbedoMap = LoadFileToMemory("Engraved_Metal_COLOR.jpg"),
                NormalMap = LoadFileToMemory("Engraved_Metal_NORM.jpg"),
                DisplacementMap = LoadFileToMemory("Engraved_Metal_DISP.png"),
                RMAMap = LoadFileToMemory("Engraved_Metal_RMC.png"),
                DisplacementMapScaleMask = new Vector4(0.1f, 0.1f, 0.1f, 0),
                EnableAutoTangent =true,
            };
            ModelTransform = new Media3D.MatrixTransform3D(Matrix.Translation(0, 30, 0).ToMatrix3D());

            builder = new MeshBuilder();
            builder.AddBox(Vector3.Zero, 100, 0.5, 100);
            var floorGeo = builder.ToMesh();
            for (int i = 0; i < floorGeo.TextureCoordinates.Count; ++i)
            {
                floorGeo.TextureCoordinates[i] *= 5;
            }
            FloorModel = floorGeo;
            FloorMaterial = new PBRMaterial()
            {
                AlbedoMap = LoadFileToMemory("Wood_Planks_COLOR.jpg"),
                NormalMap = LoadFileToMemory("Wood_Planks_NORM.jpg"),
                DisplacementMap = LoadFileToMemory("Wood_Planks_DISP.png"),
                RMAMap = LoadFileToMemory("Wood_Planks_RMA.png"),
                DisplacementMapScaleMask = new Vector4(1f, 1f, 1f, 0),
                RoughnessFactor = 0.8,
                MetallicFactor = 0.2,                
                RenderShadowMap = true,
                EnableAutoTangent = true,
            };
            FloorModelTransform = new Media3D.MatrixTransform3D(Matrix.Translation(0, -5, 0).ToMatrix3D());
        }
    }
}
