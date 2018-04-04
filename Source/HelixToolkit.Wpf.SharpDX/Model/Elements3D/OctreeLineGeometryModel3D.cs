using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Media = System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX
{
    public class OctreeLineGeometryModel3D : CompositeModel3D
    {
        public static readonly DependencyProperty OctreeProperty
            = DependencyProperty.Register("Octree", typeof(IOctree), typeof(OctreeLineGeometryModel3D),
                new PropertyMetadata(null, (s, e) =>
                {
                    var d = (s as OctreeLineGeometryModel3D);
                    if (e.OldValue != null)
                    {
                        ((IOctree)e.OldValue).OnHit -= d.OctreeLineGeometryModel3D_OnHit;
                    }
                    if (e.NewValue != null)
                    {
                        ((IOctree)e.NewValue).OnHit += d.OctreeLineGeometryModel3D_OnHit;
                    }
                    d.CreateOctreeLines();
                }));

        public static readonly DependencyProperty LineColorProperty
            = DependencyProperty.Register("LineColor", typeof(Media.Color), typeof(OctreeLineGeometryModel3D), new PropertyMetadata(Media.Colors.Green));

        public static readonly DependencyProperty HitLineColorProperty
            = DependencyProperty.Register("HitLineColor", typeof(Media.Color), typeof(OctreeLineGeometryModel3D), new PropertyMetadata(Media.Colors.Red));

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

        public Media.Color LineColor
        {
            set
            {
                SetValue(LineColorProperty, value);
            }
            get
            {
                return (Media.Color)GetValue(LineColorProperty);
            }
        }
        public Media.Color HitLineColor
        {
            set
            {
                SetValue(HitLineColorProperty, value);
            }
            get
            {
                return (Media.Color)GetValue(HitLineColorProperty);
            }
        }

        private readonly LineGeometryModel3D OctreeVisual = new LineGeometryModel3D();
        private readonly LineGeometryModel3D HitVisual = new LineGeometryModel3D();

        public OctreeLineGeometryModel3D()
        {
            IsHitTestVisible = OctreeVisual.IsHitTestVisible = HitVisual.IsHitTestVisible = false;
            Children.Add(OctreeVisual);
            Children.Add(HitVisual);
            OctreeVisual.Color = LineColor;
            HitVisual.Color = HitLineColor;
            OctreeVisual.Thickness = 0;
            OctreeVisual.FillMode = global::SharpDX.Direct3D11.FillMode.Wireframe;
            HitVisual.Thickness = 1.5;
            this.SceneNode.OnVisibleChanged += OctreeLineGeometryModel3D_OnVisibleChanged;
        }

        private void OctreeLineGeometryModel3D_OnVisibleChanged(object sender, BoolArgs e)
        {
            CreateOctreeLines();
        }

        private void CreateOctreeLines()
        {
            if (Octree != null && Visibility == Visibility.Visible && IsRendering)
            {
                OctreeVisual.Geometry = Octree.CreateOctreeLineModel();
                OctreeVisual.Color = LineColor;
            }
            else
            {
                OctreeVisual.Geometry = null;
            }
        }

        private void OctreeLineGeometryModel3D_OnHit(object sender, EventArgs args)
        {
            var node = sender as IOctree;
            if (node.HitPathBoundingBoxes.Count > 0 && Visibility == Visibility.Visible && IsRendering)
            {
                HitVisual.Geometry = node.HitPathBoundingBoxes.CreatePathLines();
                HitVisual.Color = HitLineColor;
            }
            else
            {
                HitVisual.Geometry = null;
            }
        }
    }
}
