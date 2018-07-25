//#define TESTADDREMOVE

using HelixToolkit.SharpDX.Core.Controls;
using HelixToolkit.UWP;
using HelixToolkit.UWP.Cameras;
using HelixToolkit.UWP.Core;
using HelixToolkit.UWP.Model;
using HelixToolkit.UWP.Model.Scene;
using SharpDX;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Numerics;
using HelixToolkit.Mathematics;
using Matrix = System.Numerics.Matrix4x4;

namespace CoreTest
{
    public class CoreTestApp
    {
        private readonly ViewportCore viewport;
        private readonly Form window;
        private readonly EffectsManager effectsManager;
        private CameraCore camera;
        private Geometry3D box, sphere, points, lines;
        private GroupNode groupSphere, groupBox, groupPoints, groupLines;
        private const int NumItems = 2000;
        private Random rnd = new Random((int)Stopwatch.GetTimestamp());
        private Dictionary<string, MaterialCore> materials = new Dictionary<string, MaterialCore>();
        private MaterialCore[] materialList;
        private long previousTime;
        private bool resizeRequested = false;

        public CoreTestApp(Form window)
        {
            viewport = new ViewportCore(window.Handle);
            this.window = window;
            window.ResizeEnd += Window_ResizeEnd;
            window.Load += Window_Load;
            window.FormClosing += Window_FormClosing;
            effectsManager = new DefaultEffectsManager();
            viewport.EffectsManager = effectsManager;           
            viewport.OnStartRendering += Viewport_OnStartRendering;
            viewport.OnStopRendering += Viewport_OnStopRendering;
            viewport.OnErrorOccurred += Viewport_OnErrorOccurred;
            viewport.FXAALevel = FXAALevel.Low;
            //viewport.RenderHost.EnableRenderFrustum = false;
            viewport.RenderHost.RenderConfiguration.EnableRenderOrder = true;
            InitializeScene();
        }

        private void InitializeScene()
        {
            camera = new PerspectiveCameraCore()
            {
                LookDirection = new Vector3(0, 0, 50),
                Position = new Vector3(0, 0, -50),
                FarPlaneDistance = 1000,
                NearPlaneDistance = 0.1f,
                FieldOfView = 45,
                UpDirection = new Vector3(0, 1, 0)
            };
            viewport.CameraCore = camera;
            viewport.Items.Add(new DirectionalLightNode() { Direction = new Vector3(0, -1, 1), Color = Color.White });
            viewport.Items.Add(new PointLightNode() { Position = new Vector3(0, 0, -20), Color = Color.Yellow, Range = 20, Attenuation = Vector3.One });

            var builder = new MeshBuilder(true, true, true);
            builder.AddSphere(Vector3.Zero, 1, 12, 12);
            sphere = builder.ToMesh();
            builder = new MeshBuilder(true, true, true);
            builder.AddBox(Vector3.Zero, 1, 1, 1);
            box = builder.ToMesh();
            points = new PointGeometry3D() { Positions = sphere.Positions };
            var lineBuilder = new LineBuilder();
            lineBuilder.AddBox(Vector3.Zero, 2, 2, 2);
            lines = lineBuilder.ToLineGeometry3D();
            groupSphere = new GroupNode();
            groupBox = new GroupNode();
            groupLines = new GroupNode();
            groupPoints = new GroupNode();
            InitializeMaterials();
            materialList = materials.Values.ToArray();
            var materialCount = materialList.Length;

            for (int i = 0; i < NumItems; ++i)
            {
                var transform = Matrix.CreateTranslation(new Vector3(rnd.NextFloat(-20, 20), rnd.NextFloat(-20, 20), rnd.NextFloat(-20, 20)));
                groupSphere.AddChildNode(new MeshNode() { Geometry = sphere, Material = materialList[i % materialCount], ModelMatrix = transform, CullMode = SharpDX.Direct3D11.CullMode.Back });
            }

            for (int i = 0; i < NumItems; ++i)
            {
                var transform = Matrix.CreateTranslation(new Vector3(rnd.NextFloat(-50, 50), rnd.NextFloat(-50, 50), rnd.NextFloat(-50, 50)));
                groupBox.AddChildNode(new MeshNode() { Geometry = box, Material = materialList[i % materialCount], ModelMatrix = transform, CullMode = SharpDX.Direct3D11.CullMode.Back });
            }

            for(int i=0; i< NumItems; ++i)
            {
                var transform = Matrix.CreateTranslation(new Vector3(rnd.NextFloat(-50, 50), rnd.NextFloat(-50, 50), rnd.NextFloat(-50, 50)));
                groupPoints.AddChildNode(new PointNode() { Geometry = points, ModelMatrix = transform, Color = Color.Red, Size = new Size2F(0.5f, 0.5f) });
            }

            for (int i = 0; i < NumItems; ++i)
            {
                var transform = Matrix.CreateTranslation(new Vector3(rnd.NextFloat(-50, 50), rnd.NextFloat(-50, 50), rnd.NextFloat(-50, 50)));
                groupLines.AddChildNode(new LineNode() { Geometry = lines, ModelMatrix = transform, Color = Color.LightBlue, Thickness = 0.5f });
            }

            viewport.Items.Add(groupSphere);
            groupSphere.AddChildNode(groupBox);
            groupSphere.AddChildNode(groupPoints);
            groupSphere.AddChildNode(groupLines);

            var viewbox = new ViewBoxNode();
            viewport.Items.Add(viewbox);
        }

        private void InitializeMaterials()
        {
            var diffuse = new MemoryStream();
            using (var fs = File.Open("TextureCheckerboard2.jpg", FileMode.Open))
            {
                fs.CopyTo(diffuse);
            }

            var normal = new MemoryStream();
            using (var fs = File.Open("TextureCheckerboard2_dot3.jpg", FileMode.Open))
            {
                fs.CopyTo(normal);
            }
            materials.Add("red", new DiffuseMaterialCore() { DiffuseColor = Color.Red, DiffuseMap = diffuse });
            materials.Add("green", new DiffuseMaterialCore() { DiffuseColor = Color.Green, DiffuseMap = diffuse });
            materials.Add("blue", new DiffuseMaterialCore() { DiffuseColor = Color.Blue, DiffuseMap = diffuse });
            materials.Add("DodgerBlue", new PhongMaterialCore() { DiffuseColor = Color.DodgerBlue, ReflectiveColor = Color.DarkGray, SpecularShininess = 10, SpecularColor = Color.Red, DiffuseMap = diffuse, NormalMap = normal });
            materials.Add("Orange", new PhongMaterialCore() { DiffuseColor = Color.Orange, ReflectiveColor = Color.DarkGray, SpecularShininess = 10, SpecularColor = Color.Red, DiffuseMap = diffuse, NormalMap = normal });
            materials.Add("PaleGreen", new PhongMaterialCore() { DiffuseColor = Color.PaleGreen, ReflectiveColor = Color.DarkGray, SpecularShininess = 10, SpecularColor = Color.Red, DiffuseMap = diffuse, NormalMap = normal });
            materials.Add("normal", new NormalMaterialCore());
        }

        private void Viewport_OnErrorOccurred(object sender, Exception e)
        {

        }

        private void Viewport_OnStopRendering(object sender, EventArgs e)
        {
            
        }

        private void Viewport_OnStartRendering(object sender, EventArgs e)
        {
            bool isGoingOut = true;
            bool isAddingNode = false;
            RenderLoop.Run(window, () => 
            {
                if (resizeRequested)
                {
                    viewport.Resize(window.Width, window.Height);
                    resizeRequested = false;
                    return;
                }
                var pos = camera.Position;
                var t = Stopwatch.GetTimestamp();
                var elapse = t - previousTime;
                previousTime = t;
                var angle = ((double)elapse / Stopwatch.Frequency) * 0.05;
                var camRotate = Matrix.CreateFromAxisAngle(Vector3.UnitY, (float)(angle * Math.PI));
                camera.Position = Vector3Helper.TransformCoordinate(pos, camRotate);
                camera.LookDirection = -camera.Position;
                if (isGoingOut)
                {
                    camera.Position += 0.05f * Vector3.Normalize(camera.Position);
                    if(camera.Position.LengthSquared() > 10000)
                    {
                        isGoingOut = false;
                    }
                }
                else
                {
                    camera.Position -= 0.05f * Vector3.Normalize(camera.Position);
                    if(camera.Position.LengthSquared() < 2500)
                    {
                        isGoingOut = true;
                    }
                }
                viewport.Render();
                viewport.InvalidateRender();
#if TESTADDREMOVE
                if (groupSphere.Items.Count > 0 && !isAddingNode)
                {
                    groupSphere.RemoveChildNode(groupSphere.Items.First());
                    if (groupSphere.Items.Count == 0)
                    {
                        isAddingNode = true;
                        Console.WriteLine($"{effectsManager.GetResourceCountSummary()}");
                        groupSphere.AddChildNode(groupBox);
                        groupSphere.AddChildNode(groupPoints);
                        groupPoints.AddChildNode(groupLines);
                    }
                }
                else
                {
                    var materialCount = materialList.Length;
                    var transform = Matrix.Translation(new Vector3(rnd.NextFloat(-50, 50), rnd.NextFloat(-50, 50), rnd.NextFloat(-50, 50)));
                    groupSphere.AddChildNode(new MeshNode() { Geometry = box, Material = materialList[groupSphere.Items.Count % materialCount], ModelMatrix = transform, CullMode = SharpDX.Direct3D11.CullMode.Back });
                    transform = Matrix.Translation(new Vector3(rnd.NextFloat(-20, 20), rnd.NextFloat(-20, 20), rnd.NextFloat(-20, 20)));
                    groupSphere.AddChildNode(new MeshNode() { Geometry = sphere, Material = materialList[groupSphere.Items.Count % materialCount], ModelMatrix = transform, CullMode = SharpDX.Direct3D11.CullMode.Back });
                    if (groupSphere.Items.Count > NumItems)
                    {
                        isAddingNode = false;
                    }
                }
#endif
            });
        }

        private void Window_ResizeEnd(object sender, EventArgs e)
        {
            resizeRequested = true;
        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            viewport.EndD3D();
        }

        private void Window_Load(object sender, EventArgs e)
        {
            viewport.StartD3D(window.Width, window.Height);
        }
    }
}
