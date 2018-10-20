/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if NETFX_CORE
using Windows.Foundation;
using Windows.UI.Xaml;
using Color = Windows.UI.Color;
using Colors = Windows.UI.Colors;
using Media = Windows.UI;
namespace HelixToolkit.UWP
#else
using System.Windows;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using Media = System.Windows.Media;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
    using Model.Scene;
    /// <summary>
    /// 
    /// </summary>
    public class PointGeometryModel3D : GeometryModel3D
    {
        #region Dependency Properties
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Media.Color), typeof(PointGeometryModel3D),
                new PropertyMetadata(Media.Colors.Black, (d, e) =>
                {
                    (d as PointGeometryModel3D).material.PointColor = ((Media.Color)e.NewValue).ToColor4();
                }));

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(Size), typeof(PointGeometryModel3D), new PropertyMetadata(new Size(1.0, 1.0),
                (d,e)=> 
                {
                    var size = (Size)e.NewValue;
                    (d as PointGeometryModel3D).material.Width = (float)size.Width;
                    (d as PointGeometryModel3D).material.Height = (float)size.Height;
                }));

        public static readonly DependencyProperty FigureProperty =
            DependencyProperty.Register("Figure", typeof(PointFigure), typeof(PointGeometryModel3D), new PropertyMetadata(PointFigure.Rect,
                (d, e)=> 
                {
                    (d as PointGeometryModel3D).material.Figure = (PointFigure)e.NewValue;
                }));

        public static readonly DependencyProperty FigureRatioProperty =
            DependencyProperty.Register("FigureRatio", typeof(double), typeof(PointGeometryModel3D), new PropertyMetadata(0.25,
                (d, e)=> 
                {
                    (d as PointGeometryModel3D).material.FigureRatio = (float)(double)e.NewValue;
                }));

        public static readonly DependencyProperty HitTestThicknessProperty =
            DependencyProperty.Register("HitTestThickness", typeof(double), typeof(PointGeometryModel3D), new PropertyMetadata(4.0, (d, e)=> 
                {
                    ((d as PointGeometryModel3D).SceneNode as PointNode).HitTestThickness = (float)(double)e.NewValue;
                }));

        public Media.Color Color
        {
            get { return (Media.Color)this.GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }

        public Size Size
        {
            get { return (Size)this.GetValue(SizeProperty); }
            set { this.SetValue(SizeProperty, value); }
        }

        public PointFigure Figure
        {
            get { return (PointFigure)this.GetValue(FigureProperty); }
            set { this.SetValue(FigureProperty, value); }
        }

        public double FigureRatio
        {
            get { return (double)this.GetValue(FigureRatioProperty); }
            set { this.SetValue(FigureRatioProperty, value); }
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

        protected readonly PointMaterialCore material = new PointMaterialCore();
        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override SceneNode OnCreateSceneNode()
        {
            return new PointNode() { Material = material };
        }
        /// <summary>
        /// Assigns the default values to core.
        /// </summary>
        /// <param name="core">The core.</param>
        protected override void AssignDefaultValuesToSceneNode(SceneNode core)
        {
            material.Width = (float)Size.Width;
            material.Height = (float)Size.Height;
            material.Figure = Figure;
            material.FigureRatio = (float)FigureRatio;
            material.PointColor = Color.ToColor4();
            base.AssignDefaultValuesToSceneNode(core);
        }
    }

}
