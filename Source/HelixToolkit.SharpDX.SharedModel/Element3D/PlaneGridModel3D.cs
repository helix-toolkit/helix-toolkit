#if NETFX_CORE
using Windows.UI.Xaml;
using Media = Windows.UI;
using Windows.Foundation;
using Vector3D = SharpDX.Vector3;

namespace HelixToolkit.UWP
#else
using System.Windows;
using Media = System.Windows.Media;
using Media3D = System.Windows.Media.Media3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model.Scene;

    public class PlaneGridModel3D : Element3D
    {
        /// <summary>
        /// Gets or sets a value indicating whether [automatic spacing].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [automatic spacing]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoSpacing
        {
            get { return (bool)GetValue(AutoSpacingProperty); }
            set { SetValue(AutoSpacingProperty, value); }
        }

        /// <summary>
        /// The automatic spacing property
        /// </summary>
        public static readonly DependencyProperty AutoSpacingProperty =
            DependencyProperty.Register("AutoSpacing", typeof(bool), typeof(PlaneGridModel3D), new PropertyMetadata(true,
                (d, e) =>
                {
                    ((d as Element3D).SceneNode as PlaneGridNode).AutoSpacing = (bool)e.NewValue;
                }));


        /// <summary>
        /// Gets or sets the grid spacing.
        /// </summary>
        /// <value>
        /// The grid spacing.
        /// </value>
        public double GridSpacing
        {
            get { return (double)GetValue(GridSpacingProperty); }
            set { SetValue(GridSpacingProperty, value); }
        }

        /// <summary>
        /// The grid spacing property
        /// </summary>
        public static readonly DependencyProperty GridSpacingProperty =
            DependencyProperty.Register("GridSpacing", typeof(double), typeof(PlaneGridModel3D), new PropertyMetadata(10.0,
                (d,e)=> 
                {
                    ((d as Element3D).SceneNode as PlaneGridNode).GridSpacing = (float)(double)e.NewValue;
                }));


        /// <summary>
        /// Gets or sets the grid thickness.
        /// </summary>
        /// <value>
        /// The grid thickness.
        /// </value>
        public double GridThickness
        {
            get { return (double)GetValue(GridThicknessProperty); }
            set { SetValue(GridThicknessProperty, value); }
        }

        /// <summary>
        /// The grid thickness property
        /// </summary>
        public static readonly DependencyProperty GridThicknessProperty =
            DependencyProperty.Register("GridThickness", typeof(double), typeof(PlaneGridModel3D), new PropertyMetadata(0.05,
                (d, e) =>
                {
                    ((d as Element3D).SceneNode as PlaneGridNode).GridThickness = (float)(double)e.NewValue;
                }));


        /// <summary>
        /// Gets or sets the fading factor.
        /// </summary>
        /// <value>
        /// The fading factor.
        /// </value>
        public double FadingFactor
        {
            get { return (double)GetValue(FadingFactorProperty); }
            set { SetValue(FadingFactorProperty, value); }
        }

        /// <summary>
        /// The fading factor property
        /// </summary>
        public static readonly DependencyProperty FadingFactorProperty =
            DependencyProperty.Register("FadingFactor", typeof(double), typeof(PlaneGridModel3D), new PropertyMetadata(0.6, (d, e) =>
            {
                ((d as Element3D).SceneNode as PlaneGridNode).FadingFactor = (float)(double)e.NewValue;
            }));


        /// <summary>
        /// Gets or sets the color of the plane.
        /// </summary>
        /// <value>
        /// The color of the plane.
        /// </value>
        public Media.Color PlaneColor
        {
            get { return (Media.Color)GetValue(PlaneColorProperty); }
            set { SetValue(PlaneColorProperty, value); }
        }
        /// <summary>
        /// The plane color property
        /// </summary>
        public static readonly DependencyProperty PlaneColorProperty =
            DependencyProperty.Register("PlaneColor", typeof(Media.Color), typeof(PlaneGridModel3D), 
                new PropertyMetadata(Media.Colors.Gray,
                (d, e) =>
                {
                    ((d as Element3D).SceneNode as PlaneGridNode).PlaneColor = ((Media.Color)e.NewValue).ToColor4();
                }));


        /// <summary>
        /// Gets or sets the color of the grid.
        /// </summary>
        /// <value>
        /// The color of the grid.
        /// </value>
        public Media.Color GridColor
        {
            get { return (Media.Color)GetValue(GridColorProperty); }
            set { SetValue(GridColorProperty, value); }
        }
        /// <summary>
        /// The grid color property
        /// </summary>
        public static readonly DependencyProperty GridColorProperty =
            DependencyProperty.Register("GridColor", typeof(Media.Color), typeof(PlaneGridModel3D), 
                new PropertyMetadata(Media.Colors.Black,
                (d, e) =>
                {
                    ((d as Element3D).SceneNode as PlaneGridNode).GridColor = ((Media.Color)e.NewValue).ToColor4();
                }));


        /// <summary>
        /// Gets or sets a value indicating whether [render shadow map].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render shadow map]; otherwise, <c>false</c>.
        /// </value>
        public bool RenderShadowMap
        {
            get { return (bool)GetValue(RenderShadowMapProperty); }
            set { SetValue(RenderShadowMapProperty, value); }
        }

        /// <summary>
        /// The render shadow map property
        /// </summary>
        public static readonly DependencyProperty RenderShadowMapProperty =
            DependencyProperty.Register("RenderShadowMap", typeof(bool), typeof(PlaneGridModel3D), new PropertyMetadata(false,
                (d, e) =>
                {
                    ((d as Element3D).SceneNode as PlaneGridNode).RenderShadowMap = (bool)e.NewValue;
                }));

        protected override SceneNode OnCreateSceneNode()
        {
            return new PlaneGridNode();
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode node)
        {
            base.AssignDefaultValuesToSceneNode(node);
            var n = node as PlaneGridNode;
            n.AutoSpacing = AutoSpacing;
            n.GridSpacing = (float)GridSpacing;
            n.GridThickness = (float)GridThickness;
            n.FadingFactor = (float)FadingFactor;
            n.PlaneColor = PlaneColor.ToColor4();
            n.GridColor = GridColor.ToColor4();
            n.RenderShadowMap = RenderShadowMap;
        }
    }
}
