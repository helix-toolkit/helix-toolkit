using HelixToolkit.SharpDX.Core.Animations;
using HelixToolkit.SharpDX.Core.Assimp;
using HelixToolkit.SharpDX.Core.Model.Scene;
using ImGuiNET;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using HelixToolkit.SharpDX.Core.Model;
using HelixToolkit.SharpDX.Core;

namespace CoreTest
{
    public static class SceneUI
    {
        private static bool showImGuiDemo = false;
        private static string exception = "";
        private static bool loading = false;
        private static string modelName = "";
        private static long currentTime = 0;
        public static string SomeTextFromOutside = "";

        public static HelixToolkitScene scene;

        private static bool[] animationSelection;
        private static string[] animationNames;
        private static int currentSelectedAnimation = -1;
        private static float[] fps = new float[128];
        private static float[] frustumTest = new float[128];
        private static int currFPSIndex = 0;

        public static void DrawUI(int width, int height, ref ViewportOptions options, GroupNode rootNode)
        {
            ImGui.SetNextWindowPos(System.Numerics.Vector2.Zero);
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(250, 350));
            bool opened = true;
            ImGui.Begin("Model Loader Window", ref opened, ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse);
            if (ImGui.BeginMenuBar())
            {
                if (ImGui.BeginMenu("Load Model", !loading))
                {
                    if (ImGui.MenuItem("Open"))
                    {
                        LoadModel(rootNode, options.ShowEnvironmentMap);
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Options"))
                {
                    ImGui.Checkbox("Dir Light Follow Camera", ref options.DirectionalLightFollowCamera);
                    ImGui.SliderFloat("Dir Light Intensity", ref options.DirectionLightIntensity, 0, 1, "", 1);
                    ImGui.SliderFloat("Ambient Light Intensity", ref options.AmbientLightIntensity, 0, 1, "", 1);
                    ImGui.Separator();
                    ImGui.Checkbox("Show EnvironmentMap", ref options.ShowEnvironmentMap);
                    ImGui.Checkbox("Enable SSAO", ref options.EnableSSAO);
                    ImGui.Checkbox("Enable FXAA", ref options.EnableFXAA);
                    ImGui.Checkbox("Enable Frustum", ref options.EnableFrustum);
                    if (ImGui.Checkbox("Show Wireframe", ref options.ShowWireframe))
                    {
                        options.ShowWireframeChanged = true;
                    }
                    ImGui.Separator();
                    ImGui.ColorPicker3("Background Color", ref options.BackgroundColor);
                    ImGui.EndMenu();
                }
                if (!showImGuiDemo && ImGui.BeginMenu("ImGui Demo"))
                {
                    if (ImGui.MenuItem("Show"))
                    {
                        showImGuiDemo = true;
                    }
                    ImGui.EndMenu();
                }
                ImGui.EndMenuBar();
            }
            if (ImGui.CollapsingHeader("Mouse Gestures", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.Text("Mouse Right: Rotate");
                ImGui.Text("Mouse Middle: Pan");
                ImGui.Separator();
                if (!string.IsNullOrEmpty(SomeTextFromOutside))
                {
                    ImGui.Text(SomeTextFromOutside);
                }
            }
            ImGui.Separator();
            ImGui.Text("FPS");
            ImGui.PlotLines("", ref fps[0], fps.Length, 0, $"{fps[currFPSIndex]}", 30, 70, new System.Numerics.Vector2(200, 50));
            ImGui.Text("Frustum Test Time");
            ImGui.PlotLines("", ref frustumTest[0], frustumTest.Length, 0, $"{frustumTest[currFPSIndex]}ms", 0, 5, new System.Numerics.Vector2(200, 50));
            fps[currFPSIndex] = 1000f / (float)options.Viewport.RenderHost.RenderStatistics.LatencyStatistics.AverageValue;
            frustumTest[currFPSIndex++] = (float)options.Viewport.RenderHost.RenderStatistics.FrustumTestTime * 1000;
            currFPSIndex %= 128;

            if (!loading && ImGui.CollapsingHeader("Scene Graph", ImGuiTreeNodeFlags.DefaultOpen))
            {
                DrawSceneGraph(rootNode);
            }

            if (!loading && scene != null && scene.Animations != null)
            {
                DrawAnimations(scene.Animations, ref options);
            }

            if (!loading && !string.IsNullOrEmpty(exception))
            {
                ImGui.Separator();
                ImGui.Text(exception);
            }

            if (loading)
            {
                ImGui.Text($"Loading: {modelName}");
                var progress = ((float)(Stopwatch.GetTimestamp() - currentTime) / Stopwatch.Frequency) * 100 % 100;
                ImGui.ProgressBar(progress / 100, new System.Numerics.Vector2(width, 20), "");
            }
            ImGui.End();
            if (showImGuiDemo)
            {
                opened = false;
                ImGui.ShowDemoWindow(ref showImGuiDemo);
            }
        }

        private static void LoadModel(GroupNode node, bool renderEnvironmentMap)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = HelixToolkit.SharpDX.Core.Assimp.Importer.SupportedFormatsString;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var path = dialog.FileName;             
                exception = "";
                currentTime = Stopwatch.GetTimestamp();
                loading = true;
                modelName = Path.GetFileName(path);
                Task.Run(() =>
                {
                    var importer = new Importer();
                    return importer.Load(path);
                }).ContinueWith((x) =>
                {
                    loading = false;
                    if (x.IsCompleted && x.Result != null)
                    {
                        node.Clear();
                        foreach (var model in x.Result.Root.Traverse())
                        {
                            if (model is MeshNode mesh)
                            {
                                if (mesh.Material is PBRMaterialCore pbr)
                                {
                                    pbr.RenderEnvironmentMap = renderEnvironmentMap;
                                }
                                else if (mesh.Material is PhongMaterialCore phong)
                                {
                                    phong.RenderEnvironmentMap = renderEnvironmentMap;
                                }
                            }
                        }
                        node.AddChildNode(x.Result.Root);
                        scene = x.Result;
                        if(scene.Animations != null && scene.Animations.Count > 0)
                        {
                            animationSelection = new bool[scene.Animations.Count];
                            animationNames = scene.Animations.Select((ani) => ani.Name).ToArray();
                            currentSelectedAnimation = -1;
                        }
                    }
                    else if (x.Exception != null)
                    {
                        exception = x.Exception.Message;
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private static void DrawSceneGraph(SceneNode node)
        {
            if (node.Name == null)
            {
                return;
            }
            if (node.Items.Count > 0)
            {
                if (node.IsAnimationNode)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(0, 1, 1, 1));
                }
                if (ImGui.TreeNode(node.Name))
                {
                    if (node.IsAnimationNode)
                    {
                        ImGui.PopStyleColor();
                    }
                    foreach (var n in node.Items)
                    {
                        DrawSceneGraph(n);
                    }
                    ImGui.TreePop();
                }
                else if (node.IsAnimationNode)
                {
                    ImGui.PopStyleColor();
                }
            }
            else
            {
                ImGui.Text(node.Name);
            }
        }

        private static void DrawAnimations(IList<Animation> animations, ref ViewportOptions options)
        {
            if (animations.Count > 0)
            {
                ImGui.Text($"Animations: {animations.Count}");
                if(ImGui.Combo(" ", ref currentSelectedAnimation, animationNames, animations.Count))
                {
                    if(currentSelectedAnimation>=0 && currentSelectedAnimation < animationNames.Length)
                    {
                        options.PlayAnimation = true;
                        options.AnimationUpdater = new NodeAnimationUpdater(scene.Animations[currentSelectedAnimation]);
                    }
                    else
                    {
                        options.PlayAnimation = false;
                        options.AnimationUpdater = null;
                    }
                }
            }
        }
    }
}
