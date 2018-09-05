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
        public Transform3D ModelTransform { get; }
        public PBRMaterial Material { get; }
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

                    Material.RenderEnvironmentMap = value;
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

                    Material.RenderNormalMap = value;
                }
            }
            get { return renderNormalMap; }
        }
        public PBRViewModel(IEffectsManager manager)
        {
            EffectsManager = manager;
            this.Camera = new PerspectiveCamera { Position = new Point3D(0, 0, 30), LookDirection = new Vector3D(0, 0, -30), UpDirection = new Vector3D(0, 1, 0) };
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
                        NormalMap = normalMap
                    };
                    materials.Add(m);
                    Models.Add(new MeshGeometryModel3D()
                    {
                        CullMode = SharpDX.Direct3D11.CullMode.Back,
                        Geometry = SphereModel,
                        Material = m,
                        Transform = new Media3D.TranslateTransform3D(new Vector3D(i * 6, j * 6, 0))
                    });
                }
            }
            Model = SphereModel;
            Material = new PBRMaterial()
            {               
                AlbedoColor = albedoColor.ToColor4(),
                RenderEnvironmentMap=true,
                NormalMap = normalMap
            };
            ModelTransform = new Media3D.MatrixTransform3D((Matrix.Scaling(4) * Matrix.Translation(0, 0, 10)).ToMatrix3D());
        }
    }
}
