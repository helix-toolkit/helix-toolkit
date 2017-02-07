using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HelixToolkit.Wpf.SharpDX
{
    public class OctreeLineGeometryModel3D : GroupModel3D
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
            = DependencyProperty.Register("LineColor", typeof(Color), typeof(OctreeLineGeometryModel3D), new PropertyMetadata(Color.Green));

        public static readonly DependencyProperty HitLineColorProperty
            = DependencyProperty.Register("HitLineColor", typeof(Color), typeof(OctreeLineGeometryModel3D), new PropertyMetadata(Color.Red));

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

        public Color LineColor
        {
            set
            {
                SetValue(LineColorProperty, value);
            }
            get
            {
                return (Color)GetValue(LineColorProperty);
            }
        }
        public Color HitLineColor
        {
            set
            {
                SetValue(HitLineColorProperty, value);
            }
            get
            {
                return (Color)GetValue(HitLineColorProperty);
            }
        }

        private readonly LineGeometryModel3D OctreeVisual = new LineGeometryModel3D();
        private readonly LineGeometryModel3D HitVisual = new LineGeometryModel3D();

        public OctreeLineGeometryModel3D()
        {
            IsHitTestVisible = false;
            Children.Add(OctreeVisual);
            Children.Add(HitVisual);
            OctreeVisual.Color = LineColor;
            HitVisual.Color = HitLineColor;
            OctreeVisual.Thickness = 0;
            OctreeVisual.FillMode = global::SharpDX.Direct3D11.FillMode.Wireframe;
            HitVisual.Thickness = 1.5;
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

        private void OctreeLineGeometryModel3D_OnHit(object sender, OnHitEventArgs args)
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
