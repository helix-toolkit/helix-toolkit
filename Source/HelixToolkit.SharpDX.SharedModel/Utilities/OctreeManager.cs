// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OctreeManager.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;

#if NETFX_CORE
using  Windows.UI.Xaml;
using System.ServiceModel.Dispatcher;
using FrameworkContentElement = Windows.UI.Xaml.FrameworkElement;    
using Windows.Foundation;
using Windows.UI.Core;

namespace HelixToolkit.UWP
#elif WINUI 
using Microsoft.UI.Xaml;
// using System.ServiceModel.Dispatcher;
using FrameworkContentElement = Microsoft.UI.Xaml.FrameworkElement;    
using Windows.Foundation;
using Windows.UI.Core;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Utilities;
namespace HelixToolkit.WinUI
#else
using System.Windows;
using System.Windows.Threading;
#if COREWPF
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Utilities;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
#if !COREWPF && !WINUI
    using Utilities;
#endif
    /// <summary>
    /// 
    /// </summary>
    public abstract class OctreeManagerBaseWrapper : FrameworkContentElement, IOctreeManagerWrapper
    {
        /// <summary>
        /// The octree property
        /// </summary>
        public static readonly DependencyProperty OctreeProperty
            = DependencyProperty.Register("Octree", typeof(IOctreeBasic), typeof(OctreeManagerBaseWrapper),
                new PropertyMetadata(null));
        /// <summary>
        /// The enable octree output property
        /// </summary>
        public static readonly DependencyProperty EnableOctreeOutputProperty
            = DependencyProperty.Register("EnableOctreeOutput", typeof(bool), typeof(OctreeManagerBaseWrapper),
                new PropertyMetadata(false,
                    (d, e) => { (d as OctreeManagerBaseWrapper).enableOctreeOutput = (bool)e.NewValue; }));
        /// <summary>
        /// The minimum size property
        /// </summary>
        public static readonly DependencyProperty MinSizeProperty
            = DependencyProperty.Register("MinSize", typeof(float), typeof(OctreeManagerBaseWrapper),
                new PropertyMetadata(1f, (s, e) => { (s as OctreeManagerBaseWrapper).Manager.Parameter.MinimumOctantSize = (float)e.NewValue; }));
        /// <summary>
        /// The automatic delete if empty property
        /// </summary>
        public static readonly DependencyProperty AutoDeleteIfEmptyProperty
            = DependencyProperty.Register("AutoDeleteIfEmpty", typeof(bool), typeof(OctreeManagerBaseWrapper),
                new PropertyMetadata(true, (s, e) => { (s as OctreeManagerBaseWrapper).Manager.Parameter.AutoDeleteIfEmpty = (bool)e.NewValue; }));
        /// <summary>
        /// The cubify property property
        /// </summary>
        public static readonly DependencyProperty CubifyPropertyProperty
            = DependencyProperty.Register("Cubify", typeof(bool), typeof(OctreeManagerBaseWrapper),
                new PropertyMetadata(false, (s, e) => { (s as OctreeManagerBaseWrapper).Manager.Parameter.Cubify = (bool)e.NewValue; }));
        /// <summary>
        /// The record hit path bounding boxes property
        /// </summary>
        public static readonly DependencyProperty RecordHitPathBoundingBoxesProperty
            = DependencyProperty.Register("RecordHitPathBoundingBoxes", typeof(bool), typeof(OctreeManagerBaseWrapper),
                new PropertyMetadata(false, (s, e) => { (s as OctreeManagerBaseWrapper).Manager.Parameter.RecordHitPathBoundingBoxes = (bool)e.NewValue; }));
        /// <summary>
        /// The minimum object size to split property
        /// </summary>
        public static readonly DependencyProperty MinObjectSizeToSplitProperty
            = DependencyProperty.Register("MinObjectSizeToSplit", typeof(int), typeof(OctreeManagerBaseWrapper),
                new PropertyMetadata(0, (s, e) => { (s as OctreeManagerBaseWrapper).Manager.Parameter.MinObjectSizeToSplit = (int)e.NewValue; }));
        /// <summary>
        /// Gets or sets the octree.
        /// </summary>
        /// <value>
        /// The octree.
        /// </value>
        public IOctreeBasic Octree
        {
            set
            {
                SetValue(OctreeProperty, value);
            }
            get
            {
                return (IOctreeBasic)GetValue(OctreeProperty);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enable octree output].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable octree output]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableOctreeOutput
        {
            set
            {
                SetValue(EnableOctreeOutputProperty, value);
            }
            get
            {
                return (bool)GetValue(EnableOctreeOutputProperty);
            }
        }

        /// <summary>
        /// Minimum octant size
        /// </summary>
        public float MinSize
        {
            set
            {
                SetValue(MinSizeProperty, value);
            }
            get
            {
                return (float)GetValue(MinSizeProperty);
            }
        }
        /// <summary>
        /// Delete octant node if its empty
        /// </summary>
        public bool AutoDeleteIfEmpty
        {
            set
            {
                SetValue(AutoDeleteIfEmptyProperty, value);
            }
            get
            {
                return (bool)GetValue(AutoDeleteIfEmptyProperty);
            }
        }
        /// <summary>
        /// Create cube octree
        /// </summary>
        public bool Cubify
        {
            set
            {
                SetValue(CubifyPropertyProperty, value);
            }
            get
            {
                return (bool)GetValue(CubifyPropertyProperty);
            }
        }
        /// <summary>
        /// Record the hit path bounding box for debugging
        /// </summary>
        public bool RecordHitPathBoundingBoxes
        {
            set
            {
                SetValue(RecordHitPathBoundingBoxesProperty, value);
            }
            get
            {
                return (bool)GetValue(RecordHitPathBoundingBoxesProperty);
            }
        }
        /// <summary>
        /// Minimum object in each octant to start splitting into smaller octant during build
        /// </summary>
        public int MinObjectSizeToSplit
        {
            set
            {
                SetValue(MinObjectSizeToSplitProperty, value);
            }
            get
            {
                return (int)GetValue(MinObjectSizeToSplitProperty);
            }
        }
#if NETFX_CORE || WINUI
        private IAsyncAction octreeOpt;
#else
        private DispatcherOperation octreeOpt;
#endif
        private bool enableOctreeOutput = false;
        private IOctreeManager manager;

        /// <summary>
        /// Gets the manager.
        /// </summary>
        /// <value>
        /// The manager.
        /// </value>
        public IOctreeManager Manager
        {
            get
            {
                if (manager == null)
                {
                    manager = OnCreateManager();
                    manager.OnOctreeCreated += (s, e) =>
                    {
#if !NETFX_CORE && !WINUI
                        if (octreeOpt != null && octreeOpt.Status == DispatcherOperationStatus.Pending)
                        {
                            octreeOpt.Abort();
                        }
                        if (enableOctreeOutput)
                        {
                            octreeOpt = Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                new Action(() =>
                                {
                                    this.Octree = null;
                                    this.Octree = e.Octree;
                                }));
                        }
#else
                        if (octreeOpt != null && octreeOpt.Status != AsyncStatus.Completed)
                        {
                            octreeOpt?.Cancel();
                        }
                        if (enableOctreeOutput)
                        {
                            octreeOpt = Dispatcher.RunAsync(CoreDispatcherPriority.Low, ()=> {
                                this.Octree = null;
                                this.Octree = e.Octree;
                            });
                        }
#endif
                    };
                }
                return manager;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OctreeManagerBaseWrapper"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled
        {
            set
            {
                manager.Enabled = value;
            }
            get
            {
                return manager.Enabled;
            }
        }
        /// <summary>
        /// Gets or sets the parameter.
        /// </summary>
        /// <value>
        /// The parameter.
        /// </value>
        public OctreeBuildParameter Parameter
        {
            set
            {
                Manager.Parameter = value;
            }
            get
            {
                return Manager.Parameter;
            }
        }
        /// <summary>
        /// Called when [create manager].
        /// </summary>
        /// <returns></returns>
        protected abstract IOctreeManager OnCreateManager();
    }



    /// <summary>
    /// Use to create geometryModel3D octree for groups. Each ItemsModel3D must has its own manager, do not share between two ItemsModel3D
    /// </summary>
    public sealed class GeometryModel3DOctreeManager : OctreeManagerBaseWrapper
    {
        protected override IOctreeManager OnCreateManager()
        {
            return new GroupNodeGeometryBoundOctreeManager();
        }
    }

    public sealed class InstancingModel3DOctreeManager : OctreeManagerBaseWrapper
    {
        protected override IOctreeManager OnCreateManager()
        {
            return new InstancingRenderableOctreeManager();
        }
    }
}
