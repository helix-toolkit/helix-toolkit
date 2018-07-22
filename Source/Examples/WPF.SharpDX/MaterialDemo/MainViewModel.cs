using DemoCore;
using HelixToolkit.Mathematics;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace MaterialDemo
{
    /// <summary>
    /// Model used in this demo is from: https://github.com/derkreature/IBLBaker
    /// </summary>
    /// <seealso cref="DemoCore.BaseViewModel" />
    public class MainViewModel : BaseViewModel
    {
        public ObservableElement3DCollection Model1 { get; } = new ObservableElement3DCollection();
        public ObservableElement3DCollection Model2 { get; } = new ObservableElement3DCollection();
        public ObservableElement3DCollection Model3 { get; } = new ObservableElement3DCollection();
        public ObservableElement3DCollection Model4 { get; } = new ObservableElement3DCollection();
        public ObservableElement3DCollection Model5 { get; } = new ObservableElement3DCollection();
        public ObservableElement3DCollection Model6 { get; } = new ObservableElement3DCollection();
        public ObservableElement3DCollection ModelNormalVector { get; } = new ObservableElement3DCollection();

        public MeshGeometry3D Floor { get; private set; }

        public BillboardText3D MeshTitles { private set; get; }

        public Material FloorMaterial { get; } = PhongMaterials.Gray;

        public Material NormalMaterial { get; } = new NormalMaterial();

        public Material PositionMaterial { get; } = new PositionColorMaterial();

        public Material VertMaterial { get; } = new VertColorMaterial();

        public Material NormalVectorMaterial { get; } = new NormalVectorMaterial();

        public ColorStripeMaterial ColorStripeMaterial { get; } = new ColorStripeMaterial();

        public Stream EnvironmentMap { private set; get; }

        public Transform3D Transform1 { get; } = new Media3D.TranslateTransform3D(-30, 0, 0);
        public Transform3D Transform2 { get; } = new Media3D.TranslateTransform3D(-15, 0, 0);
        public Transform3D Transform3 { get; } = new Media3D.TranslateTransform3D(0, 0, 0);
        public Transform3D Transform4 { get; } = new Media3D.TranslateTransform3D(15, 0, 0);
        public Transform3D Transform5 { get; } = new Media3D.TranslateTransform3D(30, 0, 0);

        public Transform3D Transform6 { get; } = new Media3D.TranslateTransform3D(45, 0, 0);

        public Transform3D TitleTransform { get; } = new Media3D.TranslateTransform3D(0, 10, 0);

        private Random rnd = new Random();
        private SynchronizationContext context = SynchronizationContext.Current;
        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            Title = "Material Demo";
            this.Camera = new PerspectiveCamera { Position = new Point3D(-30, 30, -30), LookDirection = new Vector3D(30, -30, 30), UpDirection = new Vector3D(0, 1, 0) };

            var builder = new MeshBuilder();
            builder.AddBox(new Vector3(0, -6, 0), 200, 2, 100);

            Floor = builder.ToMesh();

            builder = new MeshBuilder();
            builder.AddSphere(Vector3.Zero, 2);

            LoadObj(@"shaderBall\shaderBall.obj");

            EnvironmentMap = LoadFileToMemory("Cubemap_Grandcanyon.dds");

            ColorStripeMaterial.ColorStripeX = GetGradients(new Color4(1, 0, 0, 1), new Color4(0, 1, 0, 1), new Color4(0, 0, 1, 1), 48).ToList();
            ColorStripeMaterial.ColorStripeY = GetGradients(new Color4(1, 1, 0, 1), new Color4(0, 1, 1, 1), new Color4(1, 0, 1, 1), 48).ToList();

            MeshTitles = new BillboardText3D();
            MeshTitles.TextInfo.Add(new TextInfo("Blinn", Transform1.ToVector3()) { Scale = 0.08f, Background = new Color4(1,1,1,1) });
            MeshTitles.TextInfo.Add(new TextInfo("Normal", Transform2.ToVector3()) { Scale = 0.08f, Background = new Color4(1, 1, 1, 1) });
            MeshTitles.TextInfo.Add(new TextInfo("Diffuse", Transform3.ToVector3()) { Scale = 0.08f, Background = new Color4(1, 1, 1, 1) });
            MeshTitles.TextInfo.Add(new TextInfo("Position", Transform4.ToVector3()) { Scale = 0.08f, Background = new Color4(1, 1, 1, 1) });
            MeshTitles.TextInfo.Add(new TextInfo("VertexColor", Transform5.ToVector3()) { Scale = 0.08f, Background = new Color4(1, 1, 1, 1) });
            MeshTitles.TextInfo.Add(new TextInfo("ColorStripe", Transform6.ToVector3()) { Scale = 0.08f, Background = new Color4(1, 1, 1, 1) });
            (FloorMaterial as PhongMaterial).RenderShadowMap = true;
        }

        public void LoadObj(string path)
        {
            var reader = new ObjReader();
            var objCol = reader.Read(path);
            AttachModelList(objCol);
        }

        public void AttachModelList(List<Object3D> objs)
        {
            for(int i=0; i < objs.Count; ++i)
            {
                var ob = objs[i];
                var vertColor = new Color4((float)i / objs.Count, 0, 1 - (float)i / objs.Count, 1);
                ob.Geometry.Colors = new HelixToolkit.Wpf.SharpDX.Core.Color4Collection(Enumerable.Repeat(vertColor, ob.Geometry.Positions.Count));
                ob.Geometry.UpdateOctree();
                ob.Geometry.UpdateBounds();

                context.Post((o) =>
                {
                    var scaleTransform = new Media3D.ScaleTransform3D(15, 15, 15);
                    var s = new MeshGeometryModel3D
                    {
                        Geometry = ob.Geometry,
                        CullMode = SharpDX.Direct3D11.CullMode.Back,
                        IsThrowingShadow = true,
                        Transform = scaleTransform
                    };

                    var diffuseMaterial = new DiffuseMaterial();
                    if (ob.Material is PhongMaterialCore p)
                    {
                        s.Material = p;
                        (s.Material as PhongMaterial).RenderShadowMap = true;
                        diffuseMaterial.DiffuseColor = p.DiffuseColor;
                        diffuseMaterial.DiffuseMap = p.DiffuseMap;
                    }
                    //if (ob.Transform != null && ob.Transform.Count > 0)
                    //{
                    //    s.Instances = ob.Transform;
                    //}
                    this.Model1.Add(s);

                    Model2.Add(new MeshGeometryModel3D()
                    {
                        Geometry = ob.Geometry,
                        CullMode = SharpDX.Direct3D11.CullMode.Back,
                        IsThrowingShadow = true,
                        Material = NormalMaterial,
                        Transform = scaleTransform
                    });

                    ModelNormalVector.Add(new MeshGeometryModel3D()
                    {
                        Geometry = ob.Geometry,
                        CullMode = SharpDX.Direct3D11.CullMode.Back,
                        IsThrowingShadow = true,
                        Material = NormalVectorMaterial,
                        Transform = scaleTransform
                    });
                    Model3.Add(new MeshGeometryModel3D()
                    {
                        Geometry = ob.Geometry,
                        CullMode = SharpDX.Direct3D11.CullMode.Back,
                        IsThrowingShadow = true,
                        Material = diffuseMaterial,
                        Transform = scaleTransform
                    });

                    Model4.Add(new MeshGeometryModel3D()
                    {
                        Geometry = ob.Geometry,
                        CullMode = SharpDX.Direct3D11.CullMode.Back,
                        IsThrowingShadow = true,
                        Material = PositionMaterial,
                        Transform = scaleTransform
                    });

                    Model5.Add(new MeshGeometryModel3D()
                    {
                        Geometry = ob.Geometry,
                        CullMode = SharpDX.Direct3D11.CullMode.Back,
                        IsThrowingShadow = true,
                        Material = VertMaterial,
                        Transform = scaleTransform
                    });

                    Model6.Add(new MeshGeometryModel3D()
                    {
                        Geometry = ob.Geometry,
                        CullMode = SharpDX.Direct3D11.CullMode.Back,
                        IsThrowingShadow = true,
                        Material = ColorStripeMaterial,
                        Transform = scaleTransform
                    });
                }, null);
            }
        }

        public static IEnumerable<Color4> GetGradients(Color4 start, Color4 mid, Color4 end, int steps)
        {
            return GetGradients(start, mid, steps / 2).Concat(GetGradients(mid, end, steps / 2));
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
