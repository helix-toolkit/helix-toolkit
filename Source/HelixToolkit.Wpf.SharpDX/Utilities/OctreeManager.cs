// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OctreeManager.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows;

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Windows.Threading;
    using Utilities;

    public interface IOctreeManagerWrapper
    {
        IOctree Octree { get; }
        IOctreeManager Manager { get; }
    }

    public abstract class OctreeManagerBaseWrapper : FrameworkContentElement, IOctreeManagerWrapper
    {
        public static readonly DependencyProperty OctreeProperty
            = DependencyProperty.Register("Octree", typeof(IOctree), typeof(OctreeManagerBaseWrapper),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty EnableOctreeOutputProperty
            = DependencyProperty.Register("EnableOctreeOutput", typeof(bool), typeof(OctreeManagerBaseWrapper),
                new PropertyMetadata(false, 
                    (d,e)=> { (d as OctreeManagerBaseWrapper).enableOctreeOutput = (bool)e.NewValue; }));

        public static readonly DependencyProperty MinSizeProperty
            = DependencyProperty.Register("MinSize", typeof(float), typeof(OctreeManagerBaseWrapper),
                new PropertyMetadata(1f, (s, e) => { (s as OctreeManagerBaseWrapper).Manager.Parameter.MinimumOctantSize = (float)e.NewValue; }));

        public static readonly DependencyProperty AutoDeleteIfEmptyProperty
            = DependencyProperty.Register("AutoDeleteIfEmpty", typeof(bool), typeof(OctreeManagerBaseWrapper),
                new PropertyMetadata(true, (s, e) => { (s as OctreeManagerBaseWrapper).Manager.Parameter.AutoDeleteIfEmpty = (bool)e.NewValue; }));

        public static readonly DependencyProperty CubifyPropertyProperty
            = DependencyProperty.Register("Cubify", typeof(bool), typeof(OctreeManagerBaseWrapper),
                new PropertyMetadata(false, (s, e) => { (s as OctreeManagerBaseWrapper).Manager.Parameter.Cubify = (bool)e.NewValue; }));

        public static readonly DependencyProperty RecordHitPathBoundingBoxesProperty
            = DependencyProperty.Register("RecordHitPathBoundingBoxes", typeof(bool), typeof(OctreeManagerBaseWrapper),
                new PropertyMetadata(false, (s, e) => { (s as OctreeManagerBaseWrapper).Manager.Parameter.RecordHitPathBoundingBoxes = (bool)e.NewValue; }));

        public static readonly DependencyProperty MinObjectSizeToSplitProperty
            = DependencyProperty.Register("MinObjectSizeToSplit", typeof(int), typeof(OctreeManagerBaseWrapper),
                new PropertyMetadata(0, (s, e) => { (s as OctreeManagerBaseWrapper).Manager.Parameter.MinObjectSizeToSplit = (int)e.NewValue; }));

        public IOctree Octree
        {
            set
            {
                SetValue(OctreeProperty, value);
            }
            get
            {
                return (IOctree)GetValue(OctreeProperty);
            }
        }

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

        private DispatcherOperation octreeOpt;
        private bool enableOctreeOutput = false;
        private IOctreeManager manager;
        public IOctreeManager Manager
        {
            get
            {
                if(manager == null)
                {
                    manager = OnCreateManager();
                    manager.OnOctreeCreated += (s, e) =>
                    {
                        if (octreeOpt != null && octreeOpt.Status == DispatcherOperationStatus.Pending)
                        {
                            octreeOpt.Abort();
                        }
                        if (enableOctreeOutput)
                        {
                            octreeOpt = Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                new Action(() =>
                                {
                                    this.Octree = e;
                                }));
                        }
                    };
                }
                return manager;
            }
        }

        private bool mEnabled = true;
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

        protected abstract IOctreeManager OnCreateManager();
    }



    /// <summary>
    /// Use to create geometryModel3D octree for groups. Each ItemsModel3D must has its own manager, do not share between two ItemsModel3D
    /// </summary>
    public sealed class GeometryModel3DOctreeManager : OctreeManagerBaseWrapper
    {
        protected override IOctreeManager OnCreateManager()
        {
            return new GroupRenderableBoundOctreeManager();
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
