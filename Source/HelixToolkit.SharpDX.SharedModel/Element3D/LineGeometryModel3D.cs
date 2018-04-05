/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if NETFX_CORE
using Windows.UI.Xaml;
using Media = Windows.UI;
namespace HelixToolkit.UWP
#else
using System.Windows;
using Media = System.Windows.Media;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using global::SharpDX;
    using Model;
    using Model.Scene;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="HelixToolkit.UWP.GeometryModel3D" />
    public class LineGeometryModel3D : GeometryModel3D
    {
        #region Dependency Properties
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Media.Color), typeof(LineGeometryModel3D), new PropertyMetadata(Media.Colors.Black, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as LineNode).Color = ((Media.Color)e.NewValue).ToColor4();
            }));

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(double), typeof(LineGeometryModel3D), new PropertyMetadata(1.0, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as LineNode).Thickness = (float)(double)e.NewValue;
            }));

        public static readonly DependencyProperty SmoothnessProperty =
            DependencyProperty.Register("Smoothness", typeof(double), typeof(LineGeometryModel3D), new PropertyMetadata(0.0,
            (d, e) =>
            {
                ((d as Element3DCore).SceneNode as LineNode).Smoothness = (float)(double)e.NewValue;
            }));

        public static readonly DependencyProperty HitTestThicknessProperty =
            DependencyProperty.Register("HitTestThickness", typeof(double), typeof(LineGeometryModel3D), new PropertyMetadata(1.0, (d,e)=> 
            { ((d as Element3DCore).SceneNode as LineNode).HitTestThickness = (double)e.NewValue; }));

        public Media.Color Color
        {
            get { return (Media.Color)this.GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }

        public double Thickness
        {
            get { return (double)this.GetValue(ThicknessProperty); }
            set { this.SetValue(ThicknessProperty, value); }
        }


        public double Smoothness
        {
            get { return (double)this.GetValue(SmoothnessProperty); }
            set { this.SetValue(SmoothnessProperty, value); }
        }

        /// <summary>
        /// Used only for point/line hit test
        /// </summary>
        public double HitTestThickness
        {
            get { return (double)this.GetValue(HitTestThicknessProperty); }
            set { this.SetValue(HitTestThicknessProperty, value); }
        }
        #endregion

        protected override SceneNode OnCreateSceneNode()
        {
            return new LineNode();
        }

        /// <summary>
        /// Assigns the default values to core.
        /// </summary>
        /// <param name="core">The core.</param>
        protected override void AssignDefaultValuesToSceneNode(SceneNode core)
        {
            if(core is LineNode n)
            {
                n.Color = Color.ToColor4();
                n.Thickness = (float)Thickness;
                n.Smoothness = (float)Smoothness;
            }
            base.AssignDefaultValuesToSceneNode(core);
        }
    }
}
