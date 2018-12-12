using HelixToolkit.SharpDX.Core.Model.Scene;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoreTest
{
    public static class SceneUI
    {
        private static bool showImGuiDemo = false;
        private static string exception = "";
        private static bool loading = false;
        private static string modelName = "";
        private static long currentTime = 0;

        public static void DrawUI(int width, int height, ref ViewportOptions options, GroupNode rootNode)
        {
            ImGui.SetNextWindowPos(System.Numerics.Vector2.Zero, Condition.Always, System.Numerics.Vector2.Zero);
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(width, height), Condition.Always);
            bool opened = false;
            if (ImGui.BeginWindow("Model Loader Window", ref opened, 0.8f,
                WindowFlags.MenuBar | WindowFlags.NoResize | WindowFlags.NoMove | WindowFlags.NoCollapse))
            {
                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.BeginMenu("Load Model", !loading))
                    {
                        if (ImGui.MenuItem("Open"))
                        {
                            LoadModel(rootNode);
                        }
                        ImGui.EndMenu();
                    }
                    if (ImGui.BeginMenu("Options"))
                    {
                        ImGui.Checkbox("Dir Light Follow Camera", ref options.DirectionalLightFollowCamera);
                        ImGui.SliderFloat("Dir Light Intensity", ref options.DirectionLightIntensity, 0, 1, "", 1);
                        ImGui.SliderFloat("Ambient Light Intensity", ref options.AmbientLightIntensity, 0, 1, "", 1);
                        ImGui.Separator();
                        ImGui.Checkbox("Enable SSAO", ref options.EnableSSAO);
                        ImGui.Checkbox("Enable FXAA", ref options.EnableFXAA);
                        ImGui.Checkbox("Enable Frustum", ref options.EnableFrustum);
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
                if (ImGui.CollapsingHeader("Mouse Gestures", TreeNodeFlags.DefaultOpen))
                {
                    ImGui.Text("Mouse Right: Rotate");
                    ImGui.Text("Mouse Middle: Pan");
                }
                ImGui.Separator();
                if (!loading && ImGui.CollapsingHeader("Scene Graph", TreeNodeFlags.DefaultOpen))
                {
                    DrawSceneGraph(rootNode);
                }
                
                if (!loading && !string.IsNullOrEmpty(exception))
                {
                    ImGui.Separator();
                    ImGui.Text(exception, new System.Numerics.Vector4(1, 0, 0, 1));
                }

                if (loading)
                {
                    ImGui.Text($"Loading: {modelName}");
                    var progress = ((float)(Stopwatch.GetTimestamp() - currentTime) / Stopwatch.Frequency) * 100 % 100;
                    ImGui.ProgressBar(progress/100, new System.Numerics.Vector2(width, 20), "");
                }
                ImGui.EndWindow();
            }
            if (showImGuiDemo)
            {
                opened = false;
                ImGuiNative.igShowDemoWindow(ref showImGuiDemo);
            }
        }

        private static void LoadModel(GroupNode node)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            //dialog.Filter = "3D model files (*.obj;*.3ds;*.stl|*.obj;*.3ds;*.stl;*.ply;";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var path = dialog.FileName;
                node.Clear();
                exception = "";
                currentTime = Stopwatch.GetTimestamp();
                loading = true;
                modelName = Path.GetFileName(path);
                Task.Run(() =>
                {
                    var importer = new HelixToolkit.SharpDX.Core.Assimp.Importer();
                    return importer.Load(path);
                }).ContinueWith((x) =>
                {
                    loading = false;
                    if (x.IsCompleted)
                    {
                        node.AddChildNode(x.Result);
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
                if (ImGui.TreeNode(node.Name))
                {
                    foreach (var n in node.Items)
                    {
                        DrawSceneGraph(n);
                    }
                    ImGui.TreePop();
                }
            }
            else
            {
                ImGui.Text(node.Name);
            }
        }
    }
}
