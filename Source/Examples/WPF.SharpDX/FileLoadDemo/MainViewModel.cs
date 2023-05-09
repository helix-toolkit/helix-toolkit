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
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Linq;
    using SharpDX;
    using Point3D = System.Windows.Media.Media3D.Point3D;

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
                if (SetValue(ref renderFlat, value))
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
                if (SetValue(ref renderEnvironmentMap, value) && scene != null && scene.Root != null)
                {
                    foreach (var node in scene.Root.Traverse())
                    {
                        if (node is MaterialGeometryNode m && m.Material is PBRMaterialCore material)
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

        public ICommand CopyAsBitmapCommand { private set; get; }

        public ICommand CopyAsHiresBitmapCommand { private set; get; }

        private bool isLoading = false;
        public bool IsLoading
        {
            private set => SetValue(ref isLoading, value);
            get => isLoading;
        }

        private bool isPlaying = false;
        public bool IsPlaying
        {
            private set => SetValue(ref isPlaying, value);
            get => isPlaying;
        }

        private float startTime;
        public float StartTime
        {
            private set => SetValue(ref startTime, value);
            get => startTime;
        }

        private float endTime;
        public float EndTime
        {
            private set => SetValue(ref endTime, value);
            get => endTime;
        }

        private float currAnimationTime = 0;
        public float CurrAnimationTime
        {
            set
            {
                if (EndTime == 0)
                { return; }
                if (SetValue(ref currAnimationTime, value % EndTime + StartTime))
                {
                    animationUpdater?.Update(value, 1);
                }
            }
            get => currAnimationTime;
        }

        public ObservableCollection<IAnimationUpdater> Animations { get; } = new ObservableCollection<IAnimationUpdater>();

        public SceneNodeGroupModel3D GroupModel { get; } = new SceneNodeGroupModel3D();

        private IAnimationUpdater selectedAnimation = null;
        public IAnimationUpdater SelectedAnimation
        {
            set
            {
                if (SetValue(ref selectedAnimation, value))
                {
                    StopAnimation();
                    CurrAnimationTime = 0;
                    if (value != null)
                    {
                        animationUpdater = value;
                        animationUpdater.Reset();
                        animationUpdater.RepeatMode = AnimationRepeatMode.Loop;
                        StartTime = value.StartTime;
                        EndTime = value.EndTime;
                    }
                    else
                    {
                        animationUpdater = null;
                        StartTime = EndTime = 0;
                    }
                }
            }
            get
            {
                return selectedAnimation;
            }
        }

        private float speed = 1.0f;
        public float Speed
        {
            set
            {
                SetValue(ref speed, value);
            }
            get => speed;
        }

        private Point3D modelCentroid = default;
        public Point3D ModelCentroid
        {
            private set => SetValue(ref modelCentroid, value);
            get => modelCentroid;
        }
        private BoundingBox modelBound = new BoundingBox();
        public BoundingBox ModelBound
        {
            private set => SetValue(ref modelBound, value);
            get => modelBound;
        }
        public TextureModel EnvironmentMap { get; }

        public ICommand PlayCommand { get; }

        private SynchronizationContext context = SynchronizationContext.Current;
        private HelixToolkitScene scene;
        private IAnimationUpdater animationUpdater;
        private List<BoneSkinMeshNode> boneSkinNodes = new List<BoneSkinMeshNode>();
        private List<BoneSkinMeshNode> skeletonNodes = new List<BoneSkinMeshNode>();
        private CompositionTargetEx compositeHelper = new CompositionTargetEx();
        private long initTimeStamp = 0;

        private MainWindow mainWindow = null;

        public MainViewModel(MainWindow window)
        {
            mainWindow = window;

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

            CopyAsBitmapCommand = new DelegateCommand(() => { CopyAsBitmapToClipBoard(mainWindow.view); });
            CopyAsHiresBitmapCommand = new DelegateCommand(() => { CopyAsHiResBitmapToClipBoard(mainWindow.view); });

            EnvironmentMap = TextureModel.Create("Cubemap_Grandcanyon.dds");

            PlayCommand = new DelegateCommand(() =>
            {
                if (!IsPlaying && SelectedAnimation != null)
                {
                    StartAnimation();
                }
                else
                {
                    StopAnimation();
                }
            });
        }

        private void CopyAsBitmapToClipBoard(Viewport3DX viewport)
        {
            var bitmap = ViewportExtensions.RenderBitmap(viewport);
            try
            {
                Clipboard.Clear();
                Clipboard.SetImage(bitmap);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private void CopyAsHiResBitmapToClipBoard(Viewport3DX viewport)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var bitmap = ViewportExtensions.RenderBitmap(viewport, 1920, 1080);
            try
            {
                Clipboard.Clear();
                Clipboard.SetImage(bitmap);
                stopwatch.Stop();
                Debug.WriteLine($"creating bitmap needs {stopwatch.ElapsedMilliseconds} ms");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
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
            var syncContext = SynchronizationContext.Current;
            IsLoading = true;
            Task.Run(() =>
            {
                var loader = new Importer();
                var scene = loader.Load(path);
                scene.Root.Attach(EffectsManager); // Pre attach scene graph
                scene.Root.UpdateAllTransformMatrix();
                if (scene.Root.TryGetBound(out var bound))
                {
                    /// Must use UI thread to set value back.
                    syncContext.Post((o) => { ModelBound = bound; }, null);
                }
                if (scene.Root.TryGetCentroid(out var centroid))
                {
                    /// Must use UI thread to set value back.
                    syncContext.Post((o) => { ModelCentroid = centroid.ToPoint3D(); }, null);
                }
                return scene;
            }).ContinueWith((result) =>
            {
                IsLoading = false;
                if (result.IsCompleted)
                {
                    scene = result.Result;
                    Animations.Clear();
                    var oldNode = GroupModel.SceneNode.Items.ToArray();
                    GroupModel.Clear(false);
                    Task.Run(() =>
                    {
                        foreach (var node in oldNode)
                        { node.Dispose(); }
                    });
                    if (scene != null)
                    {
                        if (scene.Root != null)
                        {
                            foreach (var node in scene.Root.Traverse())
                            {
                                if (node is MaterialGeometryNode m)
                                {
                                    //m.Geometry.SetAsTransient();
                                    if (m.Material is PBRMaterialCore pbr)
                                    {
                                        pbr.RenderEnvironmentMap = RenderEnvironmentMap;
                                    }
                                    else if (m.Material is PhongMaterialCore phong)
                                    {
                                        phong.RenderEnvironmentMap = RenderEnvironmentMap;
                                    }
                                }
                            }
                        }
                        GroupModel.AddNode(scene.Root);
                        if (scene.HasAnimation)
                        {
                            var dict = scene.Animations.CreateAnimationUpdaters();
                            foreach (var ani in dict.Values)
                            {
                                Animations.Add(ani);
                            }
                        }
                        foreach (var n in scene.Root.Traverse())
                        {
                            n.Tag = new AttachedNodeViewModel(n);
                        }
                        FocusCameraToScene();
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
            initTimeStamp = Stopwatch.GetTimestamp();
            compositeHelper.Rendering += CompositeHelper_Rendering;
            IsPlaying = true;
        }

        public void StopAnimation()
        {
            IsPlaying = false;
            compositeHelper.Rendering -= CompositeHelper_Rendering;
        }

        private void CompositeHelper_Rendering(object sender, System.Windows.Media.RenderingEventArgs e)
        {
            if (animationUpdater != null)
            {
                var elapsed = (Stopwatch.GetTimestamp() - initTimeStamp) * speed;
                CurrAnimationTime = elapsed / Stopwatch.Frequency;
            }
        }

        private void FocusCameraToScene()
        {
            var maxWidth = Math.Max(Math.Max(modelBound.Width, modelBound.Height), modelBound.Depth);
            var pos = modelBound.Center + new Vector3(0, 0, maxWidth);
            Camera.Position = pos.ToPoint3D();
            Camera.LookDirection = (modelBound.Center - pos).ToVector3D();
            Camera.UpDirection = Vector3.UnitY.ToVector3D();
            if (Camera is OrthographicCamera orthCam)
            {
                orthCam.Width = maxWidth;
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
            else
            {
                path = "";
                return -1;
            }
        }

        private void ShowWireframeFunct(bool show)
        {
            foreach (var node in GroupModel.GroupNode.Items.PreorderDFT((node) =>
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