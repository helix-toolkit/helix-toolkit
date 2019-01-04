//#define TESTADDREMOVE

using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Cameras;
using HelixToolkit.SharpDX.Core.Controls;
using HelixToolkit.SharpDX.Core.Model;
using HelixToolkit.SharpDX.Core.Model.Scene;
using ImGuiNET;
using SharpDX;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CoreTest
{
    public class CoreTestApp
    {
        public ViewportCore Viewport { get => viewport; }
        private readonly ViewportCore viewport;
        private readonly Form window;
        private readonly EffectsManager effectsManager;
        private CameraCore camera;
        private Geometry3D box, sphere, points, lines;
        private GroupNode groupSphere, groupBox, groupPoints, groupLines, groupModel, groupEffects;
        private EnvironmentMapNode environmentMap;
        private DirectionalLightNode directionalLight;
        private AmbientLightNode ambientLight;
        private const int NumItems = 400;
        private Random rnd = new Random((int)Stopwatch.GetTimestamp());
        private Dictionary<string, MaterialCore> materials = new Dictionary<string, MaterialCore>();
        private MaterialCore[] materialList;
        private long previousTime;
        private bool resizeRequested = false;
        private CameraController cameraController;
        private Stack<IEnumerator<SceneNode>> stackCache = new Stack<IEnumerator<SceneNode>>();
        private IApplyPostEffect currentHighlight = null;

        private ViewportOptions options = new ViewportOptions()
        {
            AmbientLightIntensity = 0.2f,
            BackgroundColor = new System.Numerics.Vector3(0.4f, 0.4f, 0.4f),
            DirectionalLightFollowCamera = true,
            DirectionLightIntensity = 0.8f,
            EnableFrustum = true,
            EnableFXAA = true,
            EnableSSAO = true,
            WalkAround = false,
            ShowRenderDetail = false,
            ShowEnvironmentMap = false,
        };

        public CoreTestApp(Form window)
        {
            viewport = new ViewportCore(window.Handle);
            cameraController = new CameraController(viewport);
            cameraController.CameraMode = CameraMode.Inspect;
            cameraController.CameraRotationMode = CameraRotationMode.Trackball;
            this.window = window;
            window.ResizeEnd += Window_ResizeEnd;
            window.Load += Window_Load;
            window.FormClosing += Window_FormClosing;
            window.MouseMove += Window_MouseMove;
            window.MouseDown += Window_MouseDown;
            window.MouseUp += Window_MouseUp;
            window.MouseWheel += Window_MouseWheel;
            window.KeyDown += Window_KeyDown;
            window.KeyUp += Window_KeyUp;
            window.KeyPress += Window_KeyPress;
            effectsManager = new DefaultEffectsManager();
            effectsManager.AddTechnique(ImGuiNode.RenderTechnique);
            viewport.EffectsManager = effectsManager;           
            viewport.StartRendering += Viewport_OnStartRendering;
            viewport.StopRendering += Viewport_OnStopRendering;
            viewport.ErrorOccurred += Viewport_OnErrorOccurred;
            viewport.CoordinateSystemLabelColor = new Color4(1, 1, 0, 1);
            options.Viewport = viewport;
            AssignViewportOption();
            InitializeScene();
        }

        private void AssignViewportOption()
        {
            viewport.FXAALevel = options.EnableFXAA ? FXAALevel.Low : FXAALevel.None;
            viewport.EnableRenderFrustum = options.EnableFrustum;
            viewport.BackgroundColor = new Color4(options.BackgroundColor.X, options.BackgroundColor.Y, options.BackgroundColor.Z, 1);
            viewport.EnableSSAO = options.EnableSSAO;
            viewport.ShowRenderDetail = options.ShowRenderDetail;
            if (options.ShowWireframeChanged)
            {
                options.ShowWireframeChanged = false;
                foreach(var node in groupModel.Items.Traverse(true, stackCache))
                {
                    if(node is MeshNode m)
                    {
                        m.RenderWireframe = options.ShowWireframe;
                    }
                }
            }
        }


        private void InitializeScene()
        {
            camera = new PerspectiveCameraCore()
            {
                LookDirection = new Vector3(0, 0, 50),
                Position = new Vector3(0, 0, -50),
                FarPlaneDistance = 5000,
                NearPlaneDistance = 1f,
                FieldOfView = 45,
                UpDirection = new Vector3(0, 1, 0)
            };
            viewport.CameraCore = camera;
            directionalLight = new DirectionalLightNode()
            {
                Direction = new Vector3(0, -1, 1),
                Color = Color.White.ToColor4().ChangeIntensity(options.DirectionLightIntensity)
            };
            viewport.Items.AddChildNode(directionalLight);

            ambientLight = new AmbientLightNode()
            {
                Color = Color.White.ToColor4().ChangeIntensity(options.AmbientLightIntensity)
            };
            viewport.Items.AddChildNode(ambientLight);

            groupModel = new GroupNode();
            viewport.Items.AddChildNode(groupModel);
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
            groupEffects = new GroupNode();
            InitializeMaterials();
            materialList = materials.Values.ToArray();
            var materialCount = materialList.Length;

            for (int i = 0; i < NumItems; ++i)
            {
                var transform = Matrix.Translation(new Vector3(rnd.NextFloat(-20, 20), rnd.NextFloat(-20, 20), rnd.NextFloat(-20, 20)));
                groupSphere.AddChildNode(new MeshNode() { Geometry = sphere, Material = materialList[i % materialCount], ModelMatrix = transform, CullMode = SharpDX.Direct3D11.CullMode.Back });
            }

            for (int i = 0; i < NumItems; ++i)
            {
                var transform = Matrix.Translation(new Vector3(rnd.NextFloat(-50, 50), rnd.NextFloat(-50, 50), rnd.NextFloat(-50, 50)));
                groupBox.AddChildNode(new MeshNode() { Geometry = box, Material = materialList[i % materialCount], ModelMatrix = transform, CullMode = SharpDX.Direct3D11.CullMode.Back });
            }

            //for(int i=0; i< NumItems; ++i)
            //{
            //    var transform = Matrix.Translation(new Vector3(rnd.NextFloat(-50, 50), rnd.NextFloat(-50, 50), rnd.NextFloat(-50, 50)));
            //    groupPoints.AddChildNode(new PointNode() { Geometry = points, ModelMatrix = transform, Material = new PointMaterialCore() { PointColor = Color.Red } });
            //}

            //for (int i = 0; i < NumItems; ++i)
            //{
            //    var transform = Matrix.Translation(new Vector3(rnd.NextFloat(-50, 50), rnd.NextFloat(-50, 50), rnd.NextFloat(-50, 50)));
            //    groupLines.AddChildNode(new LineNode() { Geometry = lines, ModelMatrix = transform, Material = new LineMaterialCore() { LineColor = Color.LightBlue } });
            //}

            groupModel.AddChildNode(groupSphere);
            groupSphere.AddChildNode(groupBox);
            groupSphere.AddChildNode(groupPoints);
            groupSphere.AddChildNode(groupLines);

            var imGui = new ImGuiNode();
            viewport.Items.AddChildNode(imGui);
            imGui.UpdatingImGuiUI += ImGui_UpdatingImGuiUI;
            groupEffects.AddChildNode(new NodePostEffectBorderHighlight() { EffectName = "highlightEffect", Color = Color.Yellow });
            viewport.Items.AddChildNode(groupEffects);
            var environmentTexture = new MemoryStream();
            using (var fs = File.Open("Cubemap_Grandcanyon.dds", FileMode.Open))
            {
                fs.CopyTo(environmentTexture);
            }
            environmentMap = new EnvironmentMapNode() { Texture = environmentTexture };
            viewport.Items.AddChildNode(environmentMap);
            viewport.NodeHitOnMouseDown += Viewport_NodeHitOnMouseDown;
        }

        private void Viewport_NodeHitOnMouseDown(object sender, SceneNodeMouseDownArgs e)
        {
            if(currentHighlight != null)
            {
                currentHighlight.PostEffects = "";
            }
            currentHighlight = null;
            if(e.HitResult.ModelHit is IApplyPostEffect s)
            {
                currentHighlight = s;
                currentHighlight.PostEffects = "highlightEffect";
            }
        }

        private void ImGui_UpdatingImGuiUI(object sender, EventArgs e)
        {
            SceneUI.DrawUI((int)viewport.ActualWidth, (int)viewport.ActualHeight, ref options, groupModel);
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
                    viewport.Resize(window.ClientSize.Width, window.ClientSize.Height);
                    resizeRequested = false;
                    return;
                }

                var pos = camera.Position;
                var t = Stopwatch.GetTimestamp();
                var elapse = t - previousTime;
                previousTime = t;
                cameraController.OnTimeStep();
                if (options.DirectionalLightFollowCamera)
                {
                    directionalLight.Direction = camera.LookDirection.Normalized();
                }
                AssignViewportOption();
                directionalLight.Color = Color.White.ToColor4().ChangeIntensity(options.DirectionLightIntensity);
                ambientLight.Color = Color.White.ToColor4().ChangeIntensity(options.AmbientLightIntensity);
                ChangeEnvironmentMapVisibility(options.ShowEnvironmentMap);
                viewport.Render();

                if (options.PlayAnimation && options.AnimationUpdater != null)
                {
                    options.AnimationUpdater.Update(Stopwatch.GetTimestamp(), Stopwatch.Frequency);
                }
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

        private void ChangeEnvironmentMapVisibility(bool visible)
        {
            if(environmentMap.Visible != visible)
            {
                environmentMap.Visible = visible;
                foreach(var model in groupModel.Traverse())
                {
                    if(model is MeshNode mesh)
                    {
                        if(mesh.Material is PBRMaterialCore pbr)
                        {
                            pbr.RenderEnvironmentMap = visible;
                        }
                        else if(mesh.Material is PhongMaterialCore phong)
                        {
                            phong.RenderEnvironmentMap = visible;
                        }
                    }
                }
            }
        }

        private void Window_ResizeEnd(object sender, EventArgs e)
        {
            resizeRequested = true;
        }

        public void RequestResize()
        {
            resizeRequested = true;
        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            viewport.EndD3D();
        }

        private void Window_Load(object sender, EventArgs e)
        {
            viewport.StartD3D(window.ClientSize.Width, window.ClientSize.Height);
        }
        #region Handle mouse event
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            var io = ImGui.GetIO();
            if (!cameraController.IsMouseCaptured)
            {
                io.MousePos = new System.Numerics.Vector2(e.X, e.Y);
            }
            else if (!io.WantCaptureMouse)
            {
                cameraController.MouseMove(new Vector2(e.X, e.Y));
                viewport.MouseMove(new Vector2(e.X, e.Y));
            }
        }

        private void Window_MouseUp(object sender, MouseEventArgs e)
        {
            var io = ImGui.GetIO();
            switch (e.Button)
            {
                case MouseButtons.Left:
                    io.MouseDown[0] = false;
                    break;
                case MouseButtons.Right:
                    io.MouseDown[1] = false;
                    break;
                case MouseButtons.Middle:
                    io.MouseDown[2] = false;
                    break;
            }
            if (cameraController.IsMouseCaptured)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        viewport.MouseUp(new Vector2(e.X, e.Y));
                        break;
                    case MouseButtons.Right:
                        cameraController.EndRotate(new Vector2(e.X, e.Y));
                        break;
                    case MouseButtons.Middle:
                        cameraController.EndPan(new Vector2(e.X, e.Y));
                        break;
                }
            }
        }

        private void Window_MouseDown(object sender, MouseEventArgs e)
        {
            var io = ImGui.GetIO();
            if (!cameraController.IsMouseCaptured)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        io.MouseDown[0] = true;
                        break;
                    case MouseButtons.Right:
                        io.MouseDown[1] = true;
                        break;
                    case MouseButtons.Middle:
                        io.MouseDown[2] = true;
                        break;
                }
                if (!io.WantCaptureMouse)
                {
                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            viewport.MouseDown(new Vector2(e.X, e.Y));
                            break;
                        case MouseButtons.Right:
                            cameraController.StartRotate(new Vector2(e.X, e.Y));
                            break;
                        case MouseButtons.Middle:
                            cameraController.StartPan(new Vector2(e.X, e.Y));
                            break;
                    }
                }
            }
            else if(!io.WantCaptureMouse)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        break;
                    case MouseButtons.Right:
                        cameraController.StartRotate(new Vector2(e.X, e.Y));
                        break;
                    case MouseButtons.Middle:
                        cameraController.StartPan(new Vector2(e.X, e.Y));
                        break;
                }
            }
        }

        private void Window_MouseWheel(object sender, MouseEventArgs e)
        {
            var io = ImGui.GetIO();
            if (!cameraController.IsMouseCaptured)
            {
                io.MouseWheel = (int)(e.Delta * 0.01f);
            }
            if(!io.WantCaptureMouse)
            {
                cameraController.MouseWheel(e.Delta, new Vector2(e.X, e.Y));
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            var io = ImGui.GetIO();
            io.KeysDown[e.KeyValue] = true;
            io.KeyShift = e.Shift;
            io.KeyCtrl = e.Control;
            io.KeyAlt = e.Alt;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            var io = ImGui.GetIO();
            io.KeysDown[e.KeyValue] = false;
            io.KeyShift = e.Shift;
            io.KeyCtrl = e.Control;
            io.KeyAlt = e.Alt;
        }


        private void Window_KeyPress(object sender, KeyPressEventArgs e)
        {
            var io = ImGui.GetIO();
            io.AddInputCharacter(e.KeyChar);
        }
        #endregion
    }
}
