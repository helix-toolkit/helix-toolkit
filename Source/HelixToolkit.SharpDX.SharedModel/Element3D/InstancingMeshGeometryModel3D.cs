using System.Collections.Generic;

#if NETFX_CORE
using Windows.UI.Xaml;
namespace HelixToolkit.UWP
#else
using System.Windows;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
    using Model.Scene;

    public class InstancingMeshGeometryModel3D : MeshGeometryModel3D
    {
        #region DependencyProperties
        /// <summary>
        /// If bind to identifiers, hit test returns identifier as Tag in HitTestResult.
        /// </summary>
        public static readonly DependencyProperty InstanceIdentifiersProperty = DependencyProperty.Register("InstanceIdentifiers", typeof(IList<System.Guid>),
            typeof(InstancingMeshGeometryModel3D), new PropertyMetadata(null, (d,e)=>
            { ((d as Element3DCore).SceneNode as InstancingMeshNode).InstanceIdentifiers = e.NewValue as IList<System.Guid>; }));

        /// <summary>
        /// Add octree manager to use octree hit test.
        /// </summary>
        public static readonly DependencyProperty OctreeManagerProperty = DependencyProperty.Register("OctreeManager",
            typeof(IOctreeManagerWrapper), typeof(InstancingMeshGeometryModel3D), new PropertyMetadata(null, (s, e) =>
            {
                var d = s as InstancingMeshGeometryModel3D;
#if NETFX_CORE
                if(e.OldValue != null)
                {
                    d.Items.Remove(e.OldValue);
                }
                if(e.NewValue != null)
                {
                    d.Items.Add(e.NewValue);
                }
#else
                if (e.OldValue != null)
                {
                    d.RemoveLogicalChild(e.OldValue);
                }

                if (e.NewValue != null)
                {
                    d.AddLogicalChild(e.NewValue);
                }
#endif
                (d.SceneNode as InstancingMeshNode).OctreeManager = e.NewValue == null ? null : (e.NewValue as IOctreeManagerWrapper).Manager;
            }));

        /// <summary>
        /// List of instance parameter. 
        /// </summary>
        public static readonly DependencyProperty InstanceAdvArrayProperty =
            DependencyProperty.Register("InstanceParamArray", typeof(IList<InstanceParameter>), typeof(InstancingMeshGeometryModel3D), 
                new PropertyMetadata(null, (d, e) =>
                { ((d as Element3DCore).SceneNode as InstancingMeshNode).InstanceParamArray = e.NewValue as IList<InstanceParameter>; }));

        /// <summary>
        /// If bind to identifiers, hit test returns identifier as Tag in HitTestResult.
        /// </summary>        
        public IList<System.Guid> InstanceIdentifiers
        {
            set
            {
                SetValue(InstanceIdentifiersProperty, value);
            }
            get
            {
                return (IList<System.Guid>)GetValue(InstanceIdentifiersProperty);
            }
        }

        public IOctreeManagerWrapper OctreeManager
        {
            set
            {
                SetValue(OctreeManagerProperty, value);
            }
            get
            {
                return (IOctreeManagerWrapper)GetValue(OctreeManagerProperty);
            }
        }

        /// <summary>
        /// List of instance parameters. 
        /// </summary>
        public IList<InstanceParameter> InstanceParamArray
        {
            get { return (IList<InstanceParameter>)this.GetValue(InstanceAdvArrayProperty); }
            set { this.SetValue(InstanceAdvArrayProperty, value); }
        }

        #endregion

        protected override SceneNode OnCreateSceneNode()
        {
            return new InstancingMeshNode();
        }
#if NETFX_CORE
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if(OctreeManager != null && !Items.Contains(OctreeManager))
            {
                Items.Add(OctreeManager);
            }
        }
#endif
    }
}
