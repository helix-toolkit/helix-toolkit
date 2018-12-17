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
    using HelixToolkit.Wpf.SharpDX.Model.Scene;
    using Microsoft.Win32;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    public class MainViewModel : BaseViewModel
    {
        private string OpenFileFilter = $"3D model files ({HelixToolkit.Wpf.SharpDX.Assimp.Importer.SupportedFormatsString + " | " + HelixToolkit.Wpf.SharpDX.Assimp.Importer.SupportedFormatsString})";

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
                NearPlaneDistance = 1
            };
            ResetCameraCommand = new DelegateCommand(() =>
            {
                (Camera as OrthographicCamera).Reset();
            });
            ExportCommand = new DelegateCommand(() => { ExportFile(); });
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
                        GroupModel.AddNode(scene.Root);
                        if(scene.HasAnimation)
                        {
                            foreach(var ani in scene.Animations)
                            {
                                Animations.Add(ani);
                            }
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
            string path = SaveFileDialog("3D model files (*.obj;|*.obj;");
            if (string.IsNullOrEmpty(path))
            {
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

        private string SaveFileDialog(string filter)
        {
            var d = new SaveFileDialog();
            d.Filter = filter;
            if (d.ShowDialog() == true)
            {
                return d.FileName;
            }
            else { return ""; }
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
    }

}