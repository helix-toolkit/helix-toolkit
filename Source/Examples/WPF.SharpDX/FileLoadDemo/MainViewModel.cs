// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FileLoadDemo
{

    using DemoCore;
    using HelixToolkit.Wpf.SharpDX;
    using HelixToolkit.Wpf.SharpDX.Animations;
    using HelixToolkit.Wpf.SharpDX.Assimp;
    using HelixToolkit.Wpf.SharpDX.Controls;
    using HelixToolkit.Wpf.SharpDX.Model;
    using HelixToolkit.Wpf.SharpDX.Model.Scene;
    using Microsoft.Win32;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    public class MainViewModel : BaseViewModel
    {
        private string OpenFileFilter = $"{HelixToolkit.Wpf.SharpDX.Assimp.Importer.SupportedFormatsString}";
        private string ExportFileFilter = $"{HelixToolkit.Wpf.SharpDX.Assimp.Exporter.SupportedFormatsString}";
        private bool showWireframe = false;
        public bool ShowWireframe
        {
            set
            {
                if (SetValue(ref showWireframe, value))
                {
                    ShowWireframeFunct(value);
                }
            }
            get
            {
                return showWireframe;
            }
        }

        private bool renderFlat = false;
        public bool RenderFlat
        {
            set
            {
                if(SetValue(ref renderFlat, value))
                {
                    RenderFlatFunct(value);
                }
            }
            get
            {
                return renderFlat;
            }
        }

        private bool renderEnvironmentMap = true;
        public bool RenderEnvironmentMap
        {
            set
            {
                if(SetValue(ref renderEnvironmentMap, value) && scene!=null && scene.Root != null)
                {
                    foreach(var node in scene.Root.Traverse())
                    {
                        if(node is MaterialGeometryNode m && m.Material is PBRMaterialCore material)
                        {
                            material.RenderEnvironmentMap = value;
                        }
                    }
                }
            }
            get => renderEnvironmentMap;
        }

        public ICommand OpenFileCommand
        {
            get; set;
        }

        public ICommand ResetCameraCommand
        {
            set; get;
        }

        public ICommand ExportCommand { private set; get; }

        private bool isLoading = false;
        public bool IsLoading
        {
            private set => SetValue(ref isLoading, value);
            get => isLoading;
        }

        private bool enableAnimation = false;
        public bool EnableAnimation
        {
            set
            {
                if (SetValue(ref enableAnimation, value))
                {
                    if (value)
                    {
                        StartAnimation();
                    }
                    else
                    {
                        StopAnimation();
                    }
                }
            }
            get { return enableAnimation; }
        }

        public ObservableCollection<Animation> Animations { get; } = new ObservableCollection<Animation>();

        public SceneNodeGroupModel3D GroupModel { get; } = new SceneNodeGroupModel3D();

        private Animation selectedAnimation = null;
        public Animation SelectedAnimation
        {
            set
            {
                if(SetValue(ref selectedAnimation, value))
                {
                    StopAnimation();
                    if (value != null)
                    {
                        animationUpdater = new NodeAnimationUpdater(value);
                    }
                    else
                    {
                        animationUpdater = null;
                    }
                    if (enableAnimation)
                    {
                        StartAnimation();
                    }
                }
            }
            get
            {
                return selectedAnimation;
            }
        }

        public TextureModel EnvironmentMap { get; }

        private SynchronizationContext context = SynchronizationContext.Current;
        private HelixToolkitScene scene;
        private NodeAnimationUpdater animationUpdater;
        private List<BoneSkinMeshNode> boneSkinNodes = new List<BoneSkinMeshNode>();
        private List<BoneSkinMeshNode> skeletonNodes = new List<BoneSkinMeshNode>();
        private CompositionTargetEx compositeHelper = new CompositionTargetEx();


        public MainViewModel()
        {
            this.OpenFileCommand = new DelegateCommand(this.OpenFile);
            EffectsManager = new DefaultEffectsManager();
            Camera = new OrthographicCamera()
            {
                LookDirection = new System.Windows.Media.Media3D.Vector3D(0, -10, -10),
                Position = new System.Windows.Media.Media3D.Point3D(0, 10, 10),
                UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0),
                FarPlaneDistance = 5000,
                NearPlaneDistance = 0.1f
            };
            ResetCameraCommand = new DelegateCommand(() =>
            {
                (Camera as OrthographicCamera).Reset();
                (Camera as OrthographicCamera).FarPlaneDistance = 5000;
                (Camera as OrthographicCamera).NearPlaneDistance = 0.1f;
            });
            ExportCommand = new DelegateCommand(() => { ExportFile(); });
            EnvironmentMap = LoadFileToMemory("Cubemap_Grandcanyon.dds");
        }

        private void OpenFile()
        {
            if (isLoading)
            {
                return;
            }
            string path = OpenFileDialog(OpenFileFilter);
            if (path == null)
            {
                return;
            }
            StopAnimation();

            IsLoading = true;
            Task.Run(() =>
            {
                var loader = new Importer();
                return loader.Load(path);
            }).ContinueWith((result) =>
            {
                IsLoading = false;
                if (result.IsCompleted)
                {
                    scene = result.Result;
                    Animations.Clear();
                    GroupModel.Clear();
                    if (scene != null)
                    {
                        if (scene.Root != null)
                        {
                            foreach (var node in scene.Root.Traverse())
                            {
                                if (node is MaterialGeometryNode m)
                                {
                                    if (m.Material is PBRMaterialCore pbr)
                                    {
                                        pbr.RenderEnvironmentMap = RenderEnvironmentMap;
                                    }
                                    else if(m.Material is PhongMaterialCore phong)
                                    {
                                        phong.RenderEnvironmentMap = RenderEnvironmentMap;
                                    }
                                }
                            }
                        }
                        GroupModel.AddNode(scene.Root);
                        if(scene.HasAnimation)
                        {
                            foreach(var ani in scene.Animations)
                            {
                                Animations.Add(ani);
                            }
                        }
                        foreach(var n in scene.Root.Traverse())
                        {
                            n.Tag = new AttachedNodeViewModel(n);
                        }
                    }                  
                }
                else if (result.IsFaulted && result.Exception != null)
                {
                    MessageBox.Show(result.Exception.Message);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void StartAnimation()
        {
            compositeHelper.Rendering += CompositeHelper_Rendering;
        }

        public void StopAnimation()
        {
            compositeHelper.Rendering -= CompositeHelper_Rendering;
        }

        private void CompositeHelper_Rendering(object sender, System.Windows.Media.RenderingEventArgs e)
        {
            if(animationUpdater != null)
            {
                animationUpdater.Update(Stopwatch.GetTimestamp(), Stopwatch.Frequency);
            }
        }

        private void ExportFile()
        {
            var index = SaveFileDialog(ExportFileFilter, out var path);
            if (!string.IsNullOrEmpty(path) && index >= 0)
            {
                var id = HelixToolkit.Wpf.SharpDX.Assimp.Exporter.SupportedFormats[index].FormatId;
                var exporter = new HelixToolkit.Wpf.SharpDX.Assimp.Exporter();
                exporter.ExportToFile(path, scene, id);
                return;
            }
        }


        private string OpenFileDialog(string filter)
        {
            var d = new OpenFileDialog();
            d.CustomPlaces.Clear();

            d.Filter = filter;

            if (!d.ShowDialog().Value)
            {
                return null;
            }

            return d.FileName;
        }

        private int SaveFileDialog(string filter, out string path)
        {
            var d = new SaveFileDialog();
            d.Filter = filter;
            if (d.ShowDialog() == true)
            {
                path = d.FileName;
                return d.FilterIndex - 1;//This is tarting from 1. So must minus 1
            }
            else {
                path = "";
                return -1;
            }
        }

        private void ShowWireframeFunct(bool show)
        {
            foreach(var node in GroupModel.GroupNode.Items.PreorderDFT((node) =>
            {
                return node.IsRenderable;
            }))
            {
                if (node is MeshNode m)
                {
                    m.RenderWireframe = show;
                }
            }
        }

        private void RenderFlatFunct(bool show)
        {
            foreach (var node in GroupModel.GroupNode.Items.PreorderDFT((node) =>
            {
                return node.IsRenderable;
            }))
            {
                if (node is MeshNode m)
                {
                    if (m.Material is PhongMaterialCore phong)
                    {
                        phong.EnableFlatShading = show;
                    }
                    else if (m.Material is PBRMaterialCore pbr)
                    {
                        pbr.EnableFlatShading = show;
                    }
                }
            }
        }
    }

}